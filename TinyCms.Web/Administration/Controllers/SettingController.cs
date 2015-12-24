﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TinyCms.Admin.Extensions;
using TinyCms.Admin.Models.Customers;
using TinyCms.Admin.Models.Settings;
using TinyCms.Core;
using TinyCms.Core.Domain;
using TinyCms.Core.Domain.Common;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Localization;
using TinyCms.Core.Domain.Media;
using TinyCms.Core.Domain.Posts;
using TinyCms.Core.Domain.Security;
using TinyCms.Core.Domain.Seo;
using TinyCms.Services.Common;
using TinyCms.Services.Configuration;
using TinyCms.Services.Customers;
using TinyCms.Services.Helpers;
using TinyCms.Services.Localization;
using TinyCms.Services.Logging;
using TinyCms.Services.Media;
using TinyCms.Services.Security;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Controllers;
using TinyCms.Web.Framework.Kendoui;
using TinyCms.Web.Framework.Localization;
using TinyCms.Web.Framework.Mvc;
using TinyCms.Web.Framework.Security;
using TinyCms.Web.Framework.Security.Captcha;
using TinyCms.Web.Framework.Themes;

namespace TinyCms.Admin.Controllers
{
    public partial class SettingController : BaseAdminController
	{
		#region Fields

        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEncryptionService _encryptionService;
        private readonly IThemeProvider _themeProvider;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IFulltextService _fulltextService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;

        #endregion

        #region Constructors

        public SettingController(ISettingService settingService,
            IPictureService pictureService, 
            ILocalizationService localizationService, 
            IDateTimeHelper dateTimeHelper,
            IEncryptionService encryptionService,
            IThemeProvider themeProvider,
            ICustomerService customerService, 
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IFulltextService fulltextService, 
            IMaintenanceService maintenanceService,
            IWorkContext workContext, 
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService)
        {
            this._settingService = settingService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._encryptionService = encryptionService;
            this._themeProvider = themeProvider;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._fulltextService = fulltextService;
            this._maintenanceService = maintenanceService;
            this._workContext = workContext;
            this._genericAttributeService = genericAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
        }

        #endregion

        #region Utilities

      

        #endregion

        #region Methods




        public ActionResult CustomerUser()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerSettings = _settingService.LoadSetting<CustomerSettings>();
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>();
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>();

            //merge settings
            var model = new CustomerUserSettingsModel();
            model.CustomerSettings = customerSettings.ToModel();

            model.DateTimeSettings.AllowCustomersToSetTimeZone = dateTimeSettings.AllowCustomersToSetTimeZone;
            model.DateTimeSettings.DefaultStoreTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id;
            foreach (TimeZoneInfo timeZone in _dateTimeHelper.GetSystemTimeZones())
            {
                model.DateTimeSettings.AvailableTimeZones.Add(new SelectListItem
                {
                    Text = timeZone.DisplayName,
                    Value = timeZone.Id,
                    Selected = timeZone.Id.Equals(_dateTimeHelper.DefaultStoreTimeZone.Id, StringComparison.InvariantCultureIgnoreCase)
                });
            }

            model.ExternalAuthenticationSettings.AutoRegisterEnabled = externalAuthenticationSettings.AutoRegisterEnabled;

            return View(model);
        }
        [HttpPost]
        public ActionResult CustomerUser(CustomerUserSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            var customerSettings = _settingService.LoadSetting<CustomerSettings>();
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>();
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>();

            customerSettings = model.CustomerSettings.ToEntity(customerSettings);
            _settingService.SaveSetting(customerSettings);


            dateTimeSettings.DefaultStoreTimeZoneId = model.DateTimeSettings.DefaultStoreTimeZoneId;
            dateTimeSettings.AllowCustomersToSetTimeZone = model.DateTimeSettings.AllowCustomersToSetTimeZone;
            _settingService.SaveSetting(dateTimeSettings);

            externalAuthenticationSettings.AutoRegisterEnabled = model.ExternalAuthenticationSettings.AutoRegisterEnabled;
            _settingService.SaveSetting(externalAuthenticationSettings);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabIndex();

            return RedirectToAction("CustomerUser");
        }



        public ActionResult GeneralCommon()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var model = new GeneralCommonSettingsModel();
            //store information
            var storeInformationSettings = _settingService.LoadSetting<StoreInformationSettings>();
            var commonSettings = _settingService.LoadSetting<CommonSettings>();
            model.StoreInformationSettings.StoreClosed = storeInformationSettings.StoreClosed;
            //themes
            model.StoreInformationSettings.DefaultStoreTheme = storeInformationSettings.DefaultStoreTheme;
            model.StoreInformationSettings.AvailableStoreThemes = _themeProvider
                .GetThemeConfigurations()
                .Select(x => new GeneralCommonSettingsModel.StoreInformationSettingsModel.ThemeConfigurationModel
                {
                    ThemeTitle = x.ThemeTitle,
                    ThemeName = x.ThemeName,
                    PreviewImageUrl = x.PreviewImageUrl,
                    PreviewText = x.PreviewText,
                    SupportRtl = x.SupportRtl,
                    Selected = x.ThemeName.Equals(storeInformationSettings.DefaultStoreTheme, StringComparison.InvariantCultureIgnoreCase)
                })
                .ToList();
            model.StoreInformationSettings.AllowCustomerToSelectTheme = storeInformationSettings.AllowCustomerToSelectTheme;
            //EU Cookie law
            model.StoreInformationSettings.DisplayEuCookieLawWarning = storeInformationSettings.DisplayEuCookieLawWarning;
            //social pages
            model.StoreInformationSettings.FacebookLink = storeInformationSettings.FacebookLink;
            model.StoreInformationSettings.TwitterLink = storeInformationSettings.TwitterLink;
            model.StoreInformationSettings.YoutubeLink = storeInformationSettings.YoutubeLink;
            model.StoreInformationSettings.GooglePlusLink = storeInformationSettings.GooglePlusLink;
            //contact us
            model.StoreInformationSettings.SubjectFieldOnContactUsForm = commonSettings.SubjectFieldOnContactUsForm;
            model.StoreInformationSettings.UseSystemEmailForContactUsForm = commonSettings.UseSystemEmailForContactUsForm;
       

            //seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>();
            model.SeoSettings.PageTitleSeparator = seoSettings.PageTitleSeparator;
            model.SeoSettings.PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment;
            model.SeoSettings.PageTitleSeoAdjustmentValues = seoSettings.PageTitleSeoAdjustment.ToSelectList();
            model.SeoSettings.DefaultTitle = seoSettings.DefaultTitle;
            model.SeoSettings.DefaultMetaKeywords = seoSettings.DefaultMetaKeywords;
            model.SeoSettings.DefaultMetaDescription = seoSettings.DefaultMetaDescription;
            model.SeoSettings.GeneratePostMetaDescription = seoSettings.GeneratePostMetaDescription;
            model.SeoSettings.ConvertNonWesternChars = seoSettings.ConvertNonWesternChars;
            model.SeoSettings.CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled;
            model.SeoSettings.WwwRequirement = (int)seoSettings.WwwRequirement;
            model.SeoSettings.WwwRequirementValues = seoSettings.WwwRequirement.ToSelectList();
            model.SeoSettings.EnableJsBundling = seoSettings.EnableJsBundling;
            model.SeoSettings.EnableCssBundling = seoSettings.EnableCssBundling;
            model.SeoSettings.TwitterMetaTags = seoSettings.TwitterMetaTags;
            model.SeoSettings.OpenGraphMetaTags = seoSettings.OpenGraphMetaTags;
          

            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>();
            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>();
            model.SecuritySettings.EncryptionKey = securitySettings.EncryptionKey;
            if (securitySettings.AdminAreaAllowedIpAddresses != null)
                for (int i = 0; i < securitySettings.AdminAreaAllowedIpAddresses.Count; i++)
                {
                    model.SecuritySettings.AdminAreaAllowedIpAddresses += securitySettings.AdminAreaAllowedIpAddresses[i];
                    if (i != securitySettings.AdminAreaAllowedIpAddresses.Count - 1)
                        model.SecuritySettings.AdminAreaAllowedIpAddresses += ",";
                }
            model.SecuritySettings.ForceSslForAllPages = securitySettings.ForceSslForAllPages;
            model.SecuritySettings.EnableXsrfProtectionForAdminArea = securitySettings.EnableXsrfProtectionForAdminArea;
            model.SecuritySettings.EnableXsrfProtectionForPublicStore = securitySettings.EnableXsrfProtectionForPublicStore;
            model.SecuritySettings.HoneypotEnabled = securitySettings.HoneypotEnabled;
            model.SecuritySettings.CaptchaEnabled = captchaSettings.Enabled;
            model.SecuritySettings.CaptchaShowOnLoginPage = captchaSettings.ShowOnLoginPage;
            model.SecuritySettings.CaptchaShowOnRegistrationPage = captchaSettings.ShowOnRegistrationPage;
            model.SecuritySettings.CaptchaShowOnContactUsPage = captchaSettings.ShowOnContactUsPage;
            model.SecuritySettings.CaptchaShowOnEmailWishlistToFriendPage = captchaSettings.ShowOnEmailWishlistToFriendPage;
            model.SecuritySettings.CaptchaShowOnEmailPostToFriendPage = captchaSettings.ShowOnEmailPostToFriendPage;
            model.SecuritySettings.CaptchaShowOnBlogCommentPage = captchaSettings.ShowOnBlogCommentPage;
            model.SecuritySettings.CaptchaShowOnNewsCommentPage = captchaSettings.ShowOnNewsCommentPage;
            model.SecuritySettings.CaptchaShowOnPostReviewPage = captchaSettings.ShowOnPostReviewPage;
            model.SecuritySettings.CaptchaShowOnApplyVendorPage = captchaSettings.ShowOnApplyVendorPage;
            model.SecuritySettings.ReCaptchaPublicKey = captchaSettings.ReCaptchaPublicKey;
            model.SecuritySettings.ReCaptchaPrivateKey = captchaSettings.ReCaptchaPrivateKey;

       

            //localization
            var localizationSettings = _settingService.LoadSetting<LocalizationSettings>();
            model.LocalizationSettings.UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection;
            model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
            model.LocalizationSettings.AutomaticallyDetectLanguage = localizationSettings.AutomaticallyDetectLanguage;
            model.LocalizationSettings.LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup;
            model.LocalizationSettings.LoadAllLocalizedPropertiesOnStartup = localizationSettings.LoadAllLocalizedPropertiesOnStartup;
            model.LocalizationSettings.LoadAllUrlRecordsOnStartup = localizationSettings.LoadAllUrlRecordsOnStartup;

            //full-text support
            model.FullTextSettings.Supported = _fulltextService.IsFullTextSupported();
            model.FullTextSettings.Enabled = commonSettings.UseFullTextSearch;
            model.FullTextSettings.SearchMode = (int)commonSettings.FullTextMode;
            model.FullTextSettings.SearchModeValues = commonSettings.FullTextMode.ToSelectList();


            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult GeneralCommon(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


       

            //store information settings
            var storeInformationSettings = _settingService.LoadSetting<StoreInformationSettings>();
            var commonSettings = _settingService.LoadSetting<CommonSettings>();
            storeInformationSettings.StoreClosed = model.StoreInformationSettings.StoreClosed;
            storeInformationSettings.DefaultStoreTheme = model.StoreInformationSettings.DefaultStoreTheme;
            storeInformationSettings.AllowCustomerToSelectTheme = model.StoreInformationSettings.AllowCustomerToSelectTheme;
            //EU Cookie law
            storeInformationSettings.DisplayEuCookieLawWarning = model.StoreInformationSettings.DisplayEuCookieLawWarning;
            //social pages
            storeInformationSettings.FacebookLink = model.StoreInformationSettings.FacebookLink;
            storeInformationSettings.TwitterLink = model.StoreInformationSettings.TwitterLink;
            storeInformationSettings.YoutubeLink = model.StoreInformationSettings.YoutubeLink;
            storeInformationSettings.GooglePlusLink = model.StoreInformationSettings.GooglePlusLink;
            //contact us
            commonSettings.SubjectFieldOnContactUsForm = model.StoreInformationSettings.SubjectFieldOnContactUsForm;
            commonSettings.UseSystemEmailForContactUsForm = model.StoreInformationSettings.UseSystemEmailForContactUsForm;


            _settingService.SaveSetting(storeInformationSettings, x => x.StoreClosed, false);
            _settingService.SaveSetting(storeInformationSettings, x => x.DefaultStoreTheme, false);
            _settingService.SaveSetting(storeInformationSettings, x => x.AllowCustomerToSelectTheme, false);
            _settingService.SaveSetting(storeInformationSettings, x => x.DisplayEuCookieLawWarning, false);
            _settingService.SaveSetting(storeInformationSettings, x => x.FacebookLink, false);
            _settingService.SaveSetting(storeInformationSettings, x => x.TwitterLink, false);
            _settingService.SaveSetting(storeInformationSettings, x => x.YoutubeLink, false);
            _settingService.SaveSetting(storeInformationSettings, x => x.GooglePlusLink, false);
            _settingService.SaveSetting(commonSettings, x => x.SubjectFieldOnContactUsForm, false);
            _settingService.SaveSetting(commonSettings, x => x.UseSystemEmailForContactUsForm, false);



            //seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>();
            seoSettings.PageTitleSeparator = model.SeoSettings.PageTitleSeparator;
            seoSettings.PageTitleSeoAdjustment = (PageTitleSeoAdjustment)model.SeoSettings.PageTitleSeoAdjustment;
            seoSettings.DefaultTitle = model.SeoSettings.DefaultTitle;
            seoSettings.DefaultMetaKeywords = model.SeoSettings.DefaultMetaKeywords;
            seoSettings.DefaultMetaDescription = model.SeoSettings.DefaultMetaDescription;
            seoSettings.GeneratePostMetaDescription = model.SeoSettings.GeneratePostMetaDescription;
            seoSettings.ConvertNonWesternChars = model.SeoSettings.ConvertNonWesternChars;
            seoSettings.CanonicalUrlsEnabled = model.SeoSettings.CanonicalUrlsEnabled;
            seoSettings.WwwRequirement = (WwwRequirement)model.SeoSettings.WwwRequirement;
            seoSettings.EnableJsBundling = model.SeoSettings.EnableJsBundling;
            seoSettings.EnableCssBundling = model.SeoSettings.EnableCssBundling;
            seoSettings.TwitterMetaTags = model.SeoSettings.TwitterMetaTags;
            seoSettings.OpenGraphMetaTags = model.SeoSettings.OpenGraphMetaTags;

            _settingService.SaveSetting(seoSettings, x => x.PageTitleSeparator, false);
            _settingService.SaveSetting(seoSettings, x => x.PageTitleSeoAdjustment, false);
            _settingService.SaveSetting(seoSettings, x => x.DefaultTitle, false);
            _settingService.SaveSetting(seoSettings, x => x.DefaultMetaKeywords, false);
            _settingService.SaveSetting(seoSettings, x => x.DefaultMetaDescription, false);
            _settingService.SaveSetting(seoSettings, x => x.GeneratePostMetaDescription, false);
            _settingService.SaveSetting(seoSettings, x => x.ConvertNonWesternChars, false);
            _settingService.SaveSetting(seoSettings, x => x.CanonicalUrlsEnabled, false);
            _settingService.SaveSetting(seoSettings, x => x.WwwRequirement, false);
            _settingService.SaveSetting(seoSettings, x => x.EnableJsBundling, false);
            _settingService.SaveSetting(seoSettings, x => x.EnableCssBundling, false);
            _settingService.SaveSetting(seoSettings, x => x.TwitterMetaTags, false);
            _settingService.SaveSetting(seoSettings, x => x.OpenGraphMetaTags, false);

            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>();
            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>();
            if (securitySettings.AdminAreaAllowedIpAddresses == null)
                securitySettings.AdminAreaAllowedIpAddresses = new List<string>();
            securitySettings.AdminAreaAllowedIpAddresses.Clear();
            if (!String.IsNullOrEmpty(model.SecuritySettings.AdminAreaAllowedIpAddresses))
                foreach (string s in model.SecuritySettings.AdminAreaAllowedIpAddresses.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    if (!String.IsNullOrWhiteSpace(s))
                        securitySettings.AdminAreaAllowedIpAddresses.Add(s.Trim());
            securitySettings.ForceSslForAllPages = model.SecuritySettings.ForceSslForAllPages;
            securitySettings.EnableXsrfProtectionForAdminArea = model.SecuritySettings.EnableXsrfProtectionForAdminArea;
            securitySettings.EnableXsrfProtectionForPublicStore = model.SecuritySettings.EnableXsrfProtectionForPublicStore;
            securitySettings.HoneypotEnabled = model.SecuritySettings.HoneypotEnabled;
            _settingService.SaveSetting(securitySettings);
            captchaSettings.Enabled = model.SecuritySettings.CaptchaEnabled;
            captchaSettings.ShowOnLoginPage = model.SecuritySettings.CaptchaShowOnLoginPage;
            captchaSettings.ShowOnRegistrationPage = model.SecuritySettings.CaptchaShowOnRegistrationPage;
            captchaSettings.ShowOnContactUsPage = model.SecuritySettings.CaptchaShowOnContactUsPage;
            captchaSettings.ShowOnEmailWishlistToFriendPage = model.SecuritySettings.CaptchaShowOnEmailWishlistToFriendPage;
            captchaSettings.ShowOnEmailPostToFriendPage = model.SecuritySettings.CaptchaShowOnEmailPostToFriendPage;
            captchaSettings.ShowOnBlogCommentPage = model.SecuritySettings.CaptchaShowOnBlogCommentPage;
            captchaSettings.ShowOnNewsCommentPage = model.SecuritySettings.CaptchaShowOnNewsCommentPage;
            captchaSettings.ShowOnPostReviewPage = model.SecuritySettings.CaptchaShowOnPostReviewPage;
            captchaSettings.ShowOnApplyVendorPage = model.SecuritySettings.CaptchaShowOnApplyVendorPage;
            captchaSettings.ReCaptchaPublicKey = model.SecuritySettings.ReCaptchaPublicKey;
            captchaSettings.ReCaptchaPrivateKey = model.SecuritySettings.ReCaptchaPrivateKey;
            _settingService.SaveSetting(captchaSettings);
            if (captchaSettings.Enabled &&
                (String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPublicKey) || String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPrivateKey)))
            {
                //captcha is enabled but the keys are not entered
                ErrorNotification("Captcha is enabled but the appropriate keys are not entered");
            }

        

            //localization settings
            var localizationSettings = _settingService.LoadSetting<LocalizationSettings>();
            localizationSettings.UseImagesForLanguageSelection = model.LocalizationSettings.UseImagesForLanguageSelection;
            if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled != model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
                //clear cached values of routes
                System.Web.Routing.RouteTable.Routes.ClearSeoFriendlyUrlsCachedValueForRoutes();
            }
            localizationSettings.AutomaticallyDetectLanguage = model.LocalizationSettings.AutomaticallyDetectLanguage;
            localizationSettings.LoadAllLocaleRecordsOnStartup = model.LocalizationSettings.LoadAllLocaleRecordsOnStartup;
            localizationSettings.LoadAllLocalizedPropertiesOnStartup = model.LocalizationSettings.LoadAllLocalizedPropertiesOnStartup;
            localizationSettings.LoadAllUrlRecordsOnStartup = model.LocalizationSettings.LoadAllUrlRecordsOnStartup;
            _settingService.SaveSetting(localizationSettings);

            //full-text
            commonSettings.FullTextMode = (FulltextSearchMode)model.FullTextSettings.SearchMode;
            _settingService.SaveSetting(commonSettings);
            _settingService.ClearCache();
            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabIndex();

            return RedirectToAction("GeneralCommon");
        }

        public ActionResult Catalog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var catalogSettings = _settingService.LoadSetting<CatalogSettings>();
            var model = catalogSettings.ToModel();
                model.AllowViewUnpublishedPostPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowViewUnpublishedPostPage);
                model.DisplayDiscontinuedMessageForUnpublishedPosts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedPosts);
                model.ShowPostSku_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowPostSku);
                model.ShowManufacturerPartNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowManufacturerPartNumber);
                model.ShowGtin_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowGtin);
                model.ShowFreeShippingNotification_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowFreeShippingNotification);
                model.AllowPostSorting_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowPostSorting);
                model.AllowPostViewModeChanging_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowPostViewModeChanging);
                model.ShowPostsFromSubcategories_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowPostsFromSubcategories);
                model.ShowCategoryPostNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowCategoryPostNumber);
                model.ShowCategoryPostNumberIncludingSubcategories_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowCategoryPostNumberIncludingSubcategories);
                model.CategoryBreadcrumbEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.CategoryBreadcrumbEnabled);
                model.ShowShareButton_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowShareButton);
                model.PageShareCode_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PageShareCode);
                model.PostReviewsMustBeApproved_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PostReviewsMustBeApproved);
                model.AllowAnonymousUsersToReviewPost_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowAnonymousUsersToReviewPost);
                model.NotifyStoreOwnerAboutNewPostReviews_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NotifyStoreOwnerAboutNewPostReviews);
                model.EmailAFriendEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.EmailAFriendEnabled);
                model.AllowAnonymousUsersToEmailAFriend_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend);
                model.RecentlyViewedPostsNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.RecentlyViewedPostsNumber);
                model.RecentlyViewedPostsEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.RecentlyViewedPostsEnabled);
                model.NewPostsNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NewPostsNumber);
                model.NewPostsEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NewPostsEnabled);
                model.ComparePostsEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ComparePostsEnabled);
                model.ShowBestsellersOnHomepage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowBestsellersOnHomepage);
                model.NumberOfBestsellersOnHomepage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NumberOfBestsellersOnHomepage);
                model.SearchPagePostsPerPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.SearchPagePostsPerPage);
                model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize);
                model.SearchPagePageSizeOptions_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.SearchPagePageSizeOptions);
                model.PostSearchAutoCompleteEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PostSearchAutoCompleteEnabled);
                model.PostSearchAutoCompleteNumberOfPosts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PostSearchAutoCompleteNumberOfPosts);
                model.ShowPostImagesInSearchAutoComplete_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowPostImagesInSearchAutoComplete);
                model.PostSearchTermMinimumLength_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PostSearchTermMinimumLength);
                model.PostsAlsoPurchasedEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PostsAlsoPurchasedEnabled);
                model.PostsAlsoPurchasedNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PostsAlsoPurchasedNumber);
                model.NumberOfPostTags_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NumberOfPostTags);
                model.PostsByTagPageSize_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PostsByTagPageSize);
                model.PostsByTagAllowCustomersToSelectPageSize_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PostsByTagAllowCustomersToSelectPageSize);
                model.PostsByTagPageSizeOptions_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PostsByTagPageSizeOptions);
                model.IncludeShortDescriptionInComparePosts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IncludeShortDescriptionInComparePosts);
                model.IncludeFullDescriptionInComparePosts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IncludeFullDescriptionInComparePosts);
                model.IgnoreDiscounts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IgnoreDiscounts);
                model.IgnoreFeaturedPosts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IgnoreFeaturedPosts);
                model.IgnoreAcl_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IgnoreAcl);
                model.IgnoreStoreLimitations_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IgnoreStoreLimitations);
                model.CachePostPrices_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.CachePostPrices);
                model.ManufacturersBlockItemsToDisplay_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ManufacturersBlockItemsToDisplay);
                model.DisplayTaxShippingInfoFooter_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoFooter);
                model.DisplayTaxShippingInfoPostDetailsPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoPostDetailsPage);
                model.DisplayTaxShippingInfoPostBoxes_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoPostBoxes);
                model.DisplayTaxShippingInfoShoppingCart_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart);
                model.DisplayTaxShippingInfoWishlist_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoWishlist);
                model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage);
            return View(model);
        }
        [HttpPost]
        public ActionResult Catalog(CatalogSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var catalogSettings = _settingService.LoadSetting<CatalogSettings>();
            catalogSettings = model.ToEntity(catalogSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            //if (model.AllowViewUnpublishedPostPage_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.AllowViewUnpublishedPostPage, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.AllowViewUnpublishedPostPage);

            //if (model.DisplayDiscontinuedMessageForUnpublishedPosts_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedPosts, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedPosts);

            //if (model.ShowPostSku_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.ShowPostSku, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.ShowPostSku);
            
            //if (model.ShowManufacturerPartNumber_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.ShowManufacturerPartNumber, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.ShowManufacturerPartNumber);
            
            //if (model.ShowGtin_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.ShowGtin, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.ShowGtin);

            //if (model.ShowFreeShippingNotification_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.ShowFreeShippingNotification, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.ShowFreeShippingNotification);
            
            //if (model.AllowPostSorting_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.AllowPostSorting, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.AllowPostSorting);
            
            //if (model.AllowPostViewModeChanging_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.AllowPostViewModeChanging, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.AllowPostViewModeChanging);
            
            //if (model.ShowPostsFromSubcategories_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.ShowPostsFromSubcategories, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.ShowPostsFromSubcategories);
            
            //if (model.ShowCategoryPostNumber_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.ShowCategoryPostNumber, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.ShowCategoryPostNumber);
            
            //if (model.ShowCategoryPostNumberIncludingSubcategories_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.ShowCategoryPostNumberIncludingSubcategories, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.ShowCategoryPostNumberIncludingSubcategories);
            
            //if (model.CategoryBreadcrumbEnabled_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.CategoryBreadcrumbEnabled, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.CategoryBreadcrumbEnabled);
            
            //if (model.ShowShareButton_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.ShowShareButton, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.ShowShareButton);

            //if (model.PageShareCode_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.PageShareCode, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.PageShareCode);

            //if (model.PostReviewsMustBeApproved_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.PostReviewsMustBeApproved, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.PostReviewsMustBeApproved);
            
            //if (model.AllowAnonymousUsersToReviewPost_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.AllowAnonymousUsersToReviewPost, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.AllowAnonymousUsersToReviewPost);
            
            //if (model.NotifyStoreOwnerAboutNewPostReviews_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.NotifyStoreOwnerAboutNewPostReviews, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.NotifyStoreOwnerAboutNewPostReviews);
            
            //if (model.EmailAFriendEnabled_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.EmailAFriendEnabled, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.EmailAFriendEnabled);
            
            //if (model.AllowAnonymousUsersToEmailAFriend_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend);
            
            //if (model.RecentlyViewedPostsNumber_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.RecentlyViewedPostsNumber, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.RecentlyViewedPostsNumber);
            
            //if (model.RecentlyViewedPostsEnabled_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.RecentlyViewedPostsEnabled, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.RecentlyViewedPostsEnabled);
            
            //if (model.NewPostsNumber_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.NewPostsNumber, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.NewPostsNumber);
            
            //if (model.NewPostsEnabled_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.NewPostsEnabled, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.NewPostsEnabled);
            
            //if (model.ComparePostsEnabled_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.ComparePostsEnabled, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.ComparePostsEnabled);
            
            //if (model.ShowBestsellersOnHomepage_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.ShowBestsellersOnHomepage, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.ShowBestsellersOnHomepage);
            
            //if (model.NumberOfBestsellersOnHomepage_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.NumberOfBestsellersOnHomepage, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.NumberOfBestsellersOnHomepage);
            
            //if (model.SearchPagePostsPerPage_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.SearchPagePostsPerPage, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.SearchPagePostsPerPage);

            //if (model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize);

            //if (model.SearchPagePageSizeOptions_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.SearchPagePageSizeOptions, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.SearchPagePageSizeOptions);
            
            //if (model.PostSearchAutoCompleteEnabled_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.PostSearchAutoCompleteEnabled, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.PostSearchAutoCompleteEnabled);
            
            //if (model.PostSearchAutoCompleteNumberOfPosts_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.PostSearchAutoCompleteNumberOfPosts, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.PostSearchAutoCompleteNumberOfPosts);
            
            //if (model.ShowPostImagesInSearchAutoComplete_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.ShowPostImagesInSearchAutoComplete, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.ShowPostImagesInSearchAutoComplete);

            //if (model.PostSearchTermMinimumLength_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.PostSearchTermMinimumLength, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.PostSearchTermMinimumLength);
            
            //if (model.PostsAlsoPurchasedEnabled_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.PostsAlsoPurchasedEnabled, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.PostsAlsoPurchasedEnabled);
            
            //if (model.PostsAlsoPurchasedNumber_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.PostsAlsoPurchasedNumber, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.PostsAlsoPurchasedNumber);
            
            //if (model.NumberOfPostTags_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.NumberOfPostTags, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.NumberOfPostTags);
            
            //if (model.PostsByTagPageSize_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.PostsByTagPageSize, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.PostsByTagPageSize);
            
            //if (model.PostsByTagAllowCustomersToSelectPageSize_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.PostsByTagAllowCustomersToSelectPageSize, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.PostsByTagAllowCustomersToSelectPageSize);
            
            //if (model.PostsByTagPageSizeOptions_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.PostsByTagPageSizeOptions, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.PostsByTagPageSizeOptions);
            
            //if (model.IncludeShortDescriptionInComparePosts_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.IncludeShortDescriptionInComparePosts, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.IncludeShortDescriptionInComparePosts);
            
            //if (model.IncludeFullDescriptionInComparePosts_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.IncludeFullDescriptionInComparePosts, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.IncludeFullDescriptionInComparePosts);
            
            //if (model.IgnoreDiscounts_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.IgnoreDiscounts, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.IgnoreDiscounts);
            
            //if (model.IgnoreFeaturedPosts_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.IgnoreFeaturedPosts, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.IgnoreFeaturedPosts);

            //if (model.IgnoreAcl_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.IgnoreAcl, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.IgnoreAcl);

            //if (model.IgnoreStoreLimitations_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.IgnoreStoreLimitations, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.IgnoreStoreLimitations);

            //if (model.CachePostPrices_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.CachePostPrices, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.CachePostPrices);

            //if (model.ManufacturersBlockItemsToDisplay_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.ManufacturersBlockItemsToDisplay);

            //if (model.DisplayTaxShippingInfoFooter_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.DisplayTaxShippingInfoFooter, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.DisplayTaxShippingInfoFooter);

            //if (model.DisplayTaxShippingInfoPostDetailsPage_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.DisplayTaxShippingInfoPostDetailsPage, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.DisplayTaxShippingInfoPostDetailsPage);

            //if (model.DisplayTaxShippingInfoPostBoxes_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.DisplayTaxShippingInfoPostBoxes, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.DisplayTaxShippingInfoPostBoxes);

            //if (model.DisplayTaxShippingInfoShoppingCart_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart);

            //if (model.DisplayTaxShippingInfoWishlist_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.DisplayTaxShippingInfoWishlist);

            //if (model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabIndex();

            return RedirectToAction("Catalog");
        }

        //all settings
        public ActionResult AllSettings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            return View();
        }
        [HttpPost]
        //do not validate request token (XSRF)
        //for some reasons it does not work with "filtering" support
        [AdminAntiForgery(true)]
        public ActionResult AllSettings(DataSourceRequest command,
            TinyCms.Web.Framework.Kendoui.Filter filter = null, IEnumerable<Sort> sort = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var settings = _settingService
                .GetAllSettings()
                .Select(x =>
                    new SettingModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Value = x.Value,
                    })
                .AsQueryable()
                .Filter(filter)
                .Sort(sort);

            var gridModel = new DataSourceResult
            {
                Data = settings.PagedForCommand(command).ToList(),
                Total = settings.Count()
            };

            return Json(gridModel);
        }
        [HttpPost]
        public ActionResult SettingUpdate(SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var setting = _settingService.GetSettingById(model.Id);
            if (setting == null)
                return Content("No setting could be loaded with the specified ID");


            if (!setting.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                //setting name or store has been changed
                _settingService.DeleteSetting(setting);
            }

            _settingService.SetSetting(model.Name, model.Value);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            return new NullJsonResult();
        }
        [HttpPost]
        public ActionResult SettingAdd([Bind(Exclude = "Id")] SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }
            _settingService.SetSetting(model.Name, model.Value);

            //activity log
            _customerActivityService.InsertActivity("AddNewSetting", _localizationService.GetResource("ActivityLog.AddNewSetting"), model.Name);

            return new NullJsonResult();
        }
        [HttpPost]
        public ActionResult SettingDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var setting = _settingService.GetSettingById(id);
            if (setting == null)
                throw new ArgumentException("No setting found with the specified id");
            _settingService.DeleteSetting(setting);

            //activity log
            _customerActivityService.InsertActivity("DeleteSetting", _localizationService.GetResource("ActivityLog.DeleteSetting"), setting.Name);

            return new NullJsonResult();
        }
        #endregion
    }
}
