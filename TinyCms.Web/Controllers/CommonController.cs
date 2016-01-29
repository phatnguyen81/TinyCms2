using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Domain;
using TinyCms.Core.Domain.Common;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Localization;
using TinyCms.Core.Domain.Messages;
using TinyCms.Core.Infrastructure;
using TinyCms.Services.Common;
using TinyCms.Services.Customers;
using TinyCms.Services.Localization;
using TinyCms.Services.Logging;
using TinyCms.Services.Messages;
using TinyCms.Services.Security;
using TinyCms.Services.Seo;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Security;
using TinyCms.Web.Framework.Security.Captcha;
using TinyCms.Web.Framework.Themes;
using TinyCms.Web.Infrastructure.Cache;
using TinyCms.Web.Models.Common;

namespace TinyCms.Web.Controllers
{
    public class CommonController : BasePublicController
    {
        #region Constructors

        public CommonController(
            ILanguageService languageService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IQueuedEmailService queuedEmailService,
            IEmailAccountService emailAccountService,
            ISitemapGenerator sitemapGenerator,
            IThemeContext themeContext,
            IThemeProvider themeProvider,
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper,
            IPermissionService permissionService,
            ICacheManager cacheManager,
            ICustomerActivityService customerActivityService,
            CustomerSettings customerSettings,
            StoreInformationSettings storeInformationSettings,
            EmailAccountSettings emailAccountSettings,
            CommonSettings commonSettings,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings)
        {
            _languageService = languageService;
            _localizationService = localizationService;
            _workContext = workContext;
            _queuedEmailService = queuedEmailService;
            _emailAccountService = emailAccountService;
            _sitemapGenerator = sitemapGenerator;
            _themeContext = themeContext;
            _themeProvider = themeProvider;
            _genericAttributeService = genericAttributeService;
            _webHelper = webHelper;
            _permissionService = permissionService;
            _cacheManager = cacheManager;
            _customerActivityService = customerActivityService;

            _customerSettings = customerSettings;
            _storeInformationSettings = storeInformationSettings;
            _emailAccountSettings = emailAccountSettings;
            _commonSettings = commonSettings;
            _localizationSettings = localizationSettings;
            _captchaSettings = captchaSettings;
        }

        #endregion

        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly IThemeContext _themeContext;
        private readonly IThemeProvider _themeProvider;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        private readonly ICacheManager _cacheManager;
        private readonly ICustomerActivityService _customerActivityService;

        private readonly CustomerSettings _customerSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly CommonSettings _commonSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;

        #endregion

        #region Utilities

        #endregion

        #region Methods

        //page not found
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;

            return View();
        }

        //favicon
        [ChildActionOnly]
        public ActionResult Favicon(int? version)
        {
            //try loading a store specific favicon
            var faviconFileName = "favicon.ico";
            var localFaviconPath = Path.Combine(Request.PhysicalApplicationPath, faviconFileName);
            if (!System.IO.File.Exists(localFaviconPath))
            {
                //try loading a generic favicon
                faviconFileName = "favicon.ico";
                localFaviconPath = Path.Combine(Request.PhysicalApplicationPath, faviconFileName);
                if (!System.IO.File.Exists(localFaviconPath))
                {
                    return Content("");
                }
            }

            var model = new FaviconModel
            {
                FaviconUrl = _webHelper.GetStoreLocation() + faviconFileName
            };
            return PartialView(model);
        }

        //footer
        [ChildActionOnly]
        public ActionResult JavaScriptDisabledWarning()
        {
            if (!_commonSettings.DisplayJavaScriptDisabledWarning)
                return Content("");

            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            return PartialView();
        }

        public ActionResult GenericUrl()
        {
            //seems that no entity was found
            return InvokeHttp404();
        }

        //robots.txt file
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult RobotsTextFile()
        {
            var sb = new StringBuilder();

            //if robots.txt exists, let's use it
            var robotsFile = Path.Combine(_webHelper.MapPath("~/"), "robots.custom.txt");
            if (System.IO.File.Exists(robotsFile))
            {
                //the robots.txt file exists
                var robotsFileContent = System.IO.File.ReadAllText(robotsFile);
                sb.Append(robotsFileContent);
            }
            else
            {
                //doesn't exist. Let's generate it (default behavior)

                var disallowPaths = new List<string>
                {
                    "/bin/",
                    "/content/files/",
                    "/content/files/exportimport/",
                    "/country/getstatesbycountryid",
                    "/install",
                    "/setproductreviewhelpfulness"
                };
                var localizableDisallowPaths = new List<string>
                {
                    "/addproducttocart/catalog/",
                    "/addproducttocart/details/",
                    "/backinstocksubscriptions/manage",
                    "/boards/forumsubscriptions",
                    "/boards/forumwatch",
                    "/boards/postedit",
                    "/boards/postdelete",
                    "/boards/postcreate",
                    "/boards/topicedit",
                    "/boards/topicdelete",
                    "/boards/topiccreate",
                    "/boards/topicmove",
                    "/boards/topicwatch",
                    "/cart",
                    "/checkout",
                    "/checkout/billingaddress",
                    "/checkout/completed",
                    "/checkout/confirm",
                    "/checkout/shippingaddress",
                    "/checkout/shippingmethod",
                    "/checkout/paymentinfo",
                    "/checkout/paymentmethod",
                    "/clearcomparelist",
                    "/compareproducts",
                    "/compareproducts/add/*",
                    "/customer/avatar",
                    "/customer/activation",
                    "/customer/addresses",
                    "/customer/changepassword",
                    "/customer/checkusernameavailability",
                    "/customer/downloadableproducts",
                    "/customer/info",
                    "/deletepm",
                    "/emailwishlist",
                    "/inboxupdate",
                    "/newsletter/subscriptionactivation",
                    "/onepagecheckout",
                    "/order/history",
                    "/orderdetails",
                    "/passwordrecovery/confirm",
                    "/poll/vote",
                    "/privatemessages",
                    "/returnrequest",
                    "/returnrequest/history",
                    "/rewardpoints/history",
                    "/sendpm",
                    "/sentupdate",
                    "/shoppingcart/*",
                    "/subscribenewsletter",
                    "/topic/authenticate",
                    "/viewpm",
                    "/uploadfileproductattribute",
                    "/uploadfilecheckoutattribute",
                    "/wishlist"
                };


                const string newLine = "\r\n"; //Environment.NewLine
                sb.Append("User-agent: *");
                sb.Append(newLine);

                var storeSettings = EngineContext.Current.Resolve<StoreInformationSettings>();
                //sitemaps
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //URLs are localizable. Append SEO code
                    foreach (var language in _languageService.GetAllLanguages())
                    {
                        sb.AppendFormat("Sitemap: {0}{1}/sitemap.xml", storeSettings.Url, language.UniqueSeoCode);
                        sb.Append(newLine);
                    }
                }
                else
                {
                    //localizable paths (without SEO code)
                    sb.AppendFormat("Sitemap: {0}sitemap.xml", storeSettings.Url);
                    sb.Append(newLine);
                }

                //usual paths
                foreach (var path in disallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }
                //localizable paths (without SEO code)
                foreach (var path in localizableDisallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //URLs are localizable. Append SEO code
                    foreach (var language in _languageService.GetAllLanguages())
                    {
                        foreach (var path in localizableDisallowPaths)
                        {
                            sb.AppendFormat("Disallow: {0}{1}", language.UniqueSeoCode, path);
                            sb.Append(newLine);
                        }
                    }
                }

                //load and add robots.txt additions to the end of file.
                var robotsAdditionsFile = Path.Combine(_webHelper.MapPath("~/"), "robots.additions.txt");
                if (System.IO.File.Exists(robotsAdditionsFile))
                {
                    var robotsFileContent = System.IO.File.ReadAllText(robotsAdditionsFile);
                    sb.Append(robotsFileContent);
                }
            }


            Response.ContentType = "text/plain";
            Response.Write(sb.ToString());
            return null;
        }

        //SEO sitemap page
        [NopHttpsRequirement(SslRequirement.No)]
        //available even when a store is closed
        [StoreClosed(true)]
        public ActionResult SitemapXml()
        {
            if (!_commonSettings.SitemapEnabled)
                return RedirectToRoute("HomePage");

            var cacheKey = string.Format(ModelCacheEventConsumer.SITEMAP_SEO_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var siteMap = _cacheManager.Get(cacheKey, () => _sitemapGenerator.Generate(Url));
            return Content(siteMap, "text/xml");
        }

        #endregion
    }
}