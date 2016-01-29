using System.Collections.Generic;
using System.Web.Mvc;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Web.Models.Posts
{
    public class SearchModel : BaseNopModel
    {
        public SearchModel()
        {
            PagingFilteringContext = new PostsPagingFilteringModel();
            Posts = new List<PostOverviewModel>();
        }

        public string Warning { get; set; }
        public bool NoResults { get; set; }

        /// <summary>
        ///     Query string
        /// </summary>
        [NopResourceDisplayName("Search.SearchTerm")]
        [AllowHtml]
        public string q { get; set; }

        public PostsPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<PostOverviewModel> Posts { get; set; }

        #region Nested classes

        public class CategoryModel : BaseNopEntityModel
        {
            public string Breadcrumb { get; set; }
        }

        #endregion
    }
}