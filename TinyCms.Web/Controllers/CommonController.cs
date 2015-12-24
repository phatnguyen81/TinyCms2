using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Domain;
using TinyCms.Core.Domain.Common;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Localization;
using TinyCms.Core.Domain.Messages;
using TinyCms.Services.Common;
using TinyCms.Services.Localization;
using TinyCms.Services.Logging;
using TinyCms.Services.Messages;
using TinyCms.Services.Security;
using TinyCms.Services.Seo;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Security.Captcha;
using TinyCms.Web.Framework.Themes;
using TinyCms.Web.Models.Common;

namespace TinyCms.Web.Controllers
{
    public partial class CommonController : BasePublicController
    {
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
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._sitemapGenerator = sitemapGenerator;
            this._themeContext = themeContext;
            this._themeProvider = themeProvider;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
            this._permissionService = permissionService;
            this._cacheManager = cacheManager;
            this._customerActivityService = customerActivityService;

            this._customerSettings = customerSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._commonSettings = commonSettings;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
        }

        #endregion

        #region Utilities

     
        #endregion
       
        #region Methods

        //page not found
        public ActionResult PageNotFound()
        {
            this.Response.StatusCode = 404;
            this.Response.TrySkipIisCustomErrors = true;

            return View();
        }
        //favicon
        [ChildActionOnly]
        public ActionResult Favicon()
        {
            //try loading a store specific favicon
            var faviconFileName = "favicon.ico";
            var localFaviconPath = System.IO.Path.Combine(Request.PhysicalApplicationPath, faviconFileName);
            if (!System.IO.File.Exists(localFaviconPath))
            {
                //try loading a generic favicon
                faviconFileName = "favicon.ico";
                localFaviconPath = System.IO.Path.Combine(Request.PhysicalApplicationPath, faviconFileName);
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

        #endregion

    }
}
