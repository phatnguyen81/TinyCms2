using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using TinyCms.Core;
using TinyCms.Core.Domain.Common;
using TinyCms.Core.Domain.Security;
using TinyCms.Services.Posts;
using TinyCms.Services.Topics;

namespace TinyCms.Services.Seo
{
    /// <summary>
    ///     Represents a sitemap generator
    /// </summary>
    public class SitemapGenerator : ISitemapGenerator
    {
        #region Ctor

        public SitemapGenerator(
            CommonSettings commonSettings,
            SecuritySettings securitySettings, ITopicService topicService, IPostService postService,
            ICategoryService categoryService)
        {
            _commonSettings = commonSettings;
            _securitySettings = securitySettings;
            _topicService = topicService;
            _postService = postService;
            _categoryService = categoryService;
        }

        #endregion

        #region Fields

        private readonly IPostService _postService;
        private readonly ITopicService _topicService;
        private readonly ICategoryService _categoryService;
        private readonly CommonSettings _commonSettings;
        private readonly SecuritySettings _securitySettings;

        private const string DateFormat = @"yyyy-MM-dd";
        private XmlTextWriter _writer;

        #endregion

        #region Utilities

        protected virtual string GetHttpProtocol()
        {
            return _securitySettings.ForceSslForAllPages ? "https" : "http";
        }

        /// <summary>
        ///     Writes the url location to the writer.
        /// </summary>
        /// <param name="url">Url of indexed location (don't put root url information in).</param>
        /// <param name="updateFrequency">Update frequency - always, hourly, daily, weekly, yearly, never.</param>
        /// <param name="lastUpdated">Date last updated.</param>
        protected virtual void WriteUrlLocation(string url, UpdateFrequency updateFrequency, DateTime lastUpdated)
        {
            _writer.WriteStartElement("url");
            var loc = XmlHelper.XmlEncode(url);
            _writer.WriteElementString("loc", loc);
            _writer.WriteElementString("changefreq", updateFrequency.ToString().ToLowerInvariant());
            _writer.WriteElementString("lastmod", lastUpdated.ToString(DateFormat));
            _writer.WriteEndElement();
        }

        /// <summary>
        ///     Method that is overridden, that handles creation of child urls.
        ///     Use the method WriteUrlLocation() within this method.
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        protected virtual void GenerateUrlNodes(UrlHelper urlHelper)
        {
            //home page
            var homePageUrl = urlHelper.RouteUrl("HomePage", null, GetHttpProtocol());
            WriteUrlLocation(homePageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);
            //search posts
            var postSearchUrl = urlHelper.RouteUrl("PostSearch", null, GetHttpProtocol());
            WriteUrlLocation(postSearchUrl, UpdateFrequency.Weekly, DateTime.UtcNow);
            //contact us
            //var contactUsUrl = urlHelper.RouteUrl("ContactUs", null, GetHttpProtocol());
            //WriteUrlLocation(contactUsUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

            //categories
            if (_commonSettings.SitemapIncludeCategories)
            {
                WriteCategories(urlHelper, 0);
            }
            //topics
            WriteTopics(urlHelper);
        }

        protected virtual void WriteTopics(UrlHelper urlHelper)
        {
            var topics = _topicService.GetAllTopics()
                .Where(t => t.IncludeInSitemap)
                .ToList();
            foreach (var topic in topics)
            {
                var url = urlHelper.RouteUrl("Topic", new {SeName = topic.GetSeName()}, GetHttpProtocol());
                WriteUrlLocation(url, UpdateFrequency.Weekly, DateTime.UtcNow);
            }
        }

        protected virtual void WriteCategories(UrlHelper urlHelper, int parentCategoryId)
        {
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
            foreach (var category in categories)
            {
                var url = urlHelper.RouteUrl("Category", new {SeName = category.GetSeName()}, GetHttpProtocol());
                WriteUrlLocation(url, UpdateFrequency.Weekly, category.UpdatedOnUtc);

                WriteCategories(urlHelper, category.Id);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This will build an xml sitemap for better index with search engines.
        ///     See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <returns>Sitemap.xml as string</returns>
        public virtual string Generate(UrlHelper urlHelper)
        {
            using (var stream = new MemoryStream())
            {
                Generate(urlHelper, stream);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        ///     This will build an xml sitemap for better index with search engines.
        ///     See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <param name="stream">Stream of sitemap.</param>
        public virtual void Generate(UrlHelper urlHelper, Stream stream)
        {
            _writer = new XmlTextWriter(stream, Encoding.UTF8);
            _writer.Formatting = Formatting.Indented;
            _writer.WriteStartDocument();
            _writer.WriteStartElement("urlset");
            _writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            _writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            _writer.WriteAttributeString("xsi:schemaLocation",
                "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

            GenerateUrlNodes(urlHelper);

            _writer.WriteEndElement();
            _writer.Close();
        }

        #endregion
    }
}