using System.Collections.Generic;
using System.Web.Mvc;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Settings
{
    public class GeneralCommonSettingsModel : BaseNopModel
    {
        public GeneralCommonSettingsModel()
        {
            StoreInformationSettings = new StoreInformationSettingsModel();
            SeoSettings = new SeoSettingsModel();
            SecuritySettings = new SecuritySettingsModel();
            LocalizationSettings = new LocalizationSettingsModel();
            FullTextSettings = new FullTextSettingsModel();
            SocialSettings = new SocialSettingsModel();
        }

        public StoreInformationSettingsModel StoreInformationSettings { get; set; }
        public SocialSettingsModel SocialSettings { get; set; }
        public SeoSettingsModel SeoSettings { get; set; }
        public SecuritySettingsModel SecuritySettings { get; set; }
        public LocalizationSettingsModel LocalizationSettings { get; set; }
        public FullTextSettingsModel FullTextSettings { get; set; }

        #region Nested classes

        public class StoreInformationSettingsModel : BaseNopModel
        {
            public StoreInformationSettingsModel()
            {
                AvailableStoreThemes = new List<ThemeConfigurationModel>();
            }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.StoreClosed")]
            public bool StoreClosed { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme")]
            [AllowHtml]
            public string DefaultStoreTheme { get; set; }

            public IList<ThemeConfigurationModel> AvailableStoreThemes { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AllowCustomerToSelectTheme")]
            public bool AllowCustomerToSelectTheme { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayEuCookieLawWarning")]
            public bool DisplayEuCookieLawWarning { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FacebookLink")]
            public string FacebookLink { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TwitterLink")]
            public string TwitterLink { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.YoutubeLink")]
            public string YoutubeLink { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GooglePlusLink")]
            public string GooglePlusLink { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SubjectFieldOnContactUsForm")]
            public bool SubjectFieldOnContactUsForm { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseSystemEmailForContactUsForm")]
            public bool UseSystemEmailForContactUsForm { get; set; }

            public string Name { get; set; }

            /// <summary>
            ///     Gets or sets the store URL
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether SSL is enabled
            /// </summary>
            public bool SslEnabled { get; set; }

            /// <summary>
            ///     Gets or sets the store secure URL (HTTPS)
            /// </summary>
            public string SecureUrl { get; set; }

            /// <summary>
            ///     Gets or sets the comma separated list of possible HTTP_HOST values
            /// </summary>
            public string Hosts { get; set; }

            /// <summary>
            ///     Gets or sets the identifier of the default language for this store; 0 is set when we use the default language
            ///     display order
            /// </summary>
            public int DefaultLanguageId { get; set; }

            /// <summary>
            ///     Gets or sets the company name
            /// </summary>
            public string CompanyName { get; set; }

            /// <summary>
            ///     Gets or sets the company address
            /// </summary>
            public string CompanyAddress { get; set; }

            /// <summary>
            ///     Gets or sets the store phone number
            /// </summary>
            public string CompanyPhoneNumber { get; set; }

            #region Nested classes

            public class ThemeConfigurationModel
            {
                public string ThemeName { get; set; }
                public string ThemeTitle { get; set; }
                public string PreviewImageUrl { get; set; }
                public string PreviewText { get; set; }
                public bool SupportRtl { get; set; }
                public bool Selected { get; set; }
            }

            #endregion
        }

        public class SeoSettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeparator")]
            [AllowHtml]
            public string PageTitleSeparator { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeoAdjustment")]
            public int PageTitleSeoAdjustment { get; set; }

            public SelectList PageTitleSeoAdjustmentValues { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultTitle")]
            [AllowHtml]
            public string DefaultTitle { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords")]
            [AllowHtml]
            public string DefaultMetaKeywords { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription")]
            [AllowHtml]
            public string DefaultMetaDescription { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GeneratePostMetaDescription")]
            [AllowHtml]
            public bool GeneratePostMetaDescription { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ConvertNonWesternChars")]
            public bool ConvertNonWesternChars { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CanonicalUrlsEnabled")]
            public bool CanonicalUrlsEnabled { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.WwwRequirement")]
            public int WwwRequirement { get; set; }

            public SelectList WwwRequirementValues { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableJsBundling")]
            public bool EnableJsBundling { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableCssBundling")]
            public bool EnableCssBundling { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TwitterMetaTags")]
            public bool TwitterMetaTags { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.OpenGraphMetaTags")]
            public bool OpenGraphMetaTags { get; set; }
        }

        public class SecuritySettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EncryptionKey")]
            [AllowHtml]
            public string EncryptionKey { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminAreaAllowedIpAddresses")]
            [AllowHtml]
            public string AdminAreaAllowedIpAddresses { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ForceSslForAllPages")]
            public bool ForceSslForAllPages { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForAdminArea")]
            public bool EnableXsrfProtectionForAdminArea { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForPublicStore")]
            public bool EnableXsrfProtectionForPublicStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HoneypotEnabled")]
            public bool HoneypotEnabled { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaEnabled")]
            public bool CaptchaEnabled { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnLoginPage")]
            public bool CaptchaShowOnLoginPage { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnRegistrationPage")]
            public bool CaptchaShowOnRegistrationPage { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnContactUsPage")]
            public bool CaptchaShowOnContactUsPage { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailWishlistToFriendPage")
            ]
            public bool CaptchaShowOnEmailWishlistToFriendPage { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailPostToFriendPage")]
            public bool CaptchaShowOnEmailPostToFriendPage { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnBlogCommentPage")]
            public bool CaptchaShowOnBlogCommentPage { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnNewsCommentPage")]
            public bool CaptchaShowOnNewsCommentPage { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnPostReviewPage")]
            public bool CaptchaShowOnPostReviewPage { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnApplyVendorPage")]
            public bool CaptchaShowOnApplyVendorPage { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPublicKey")]
            [AllowHtml]
            public string ReCaptchaPublicKey { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPrivateKey")]
            [AllowHtml]
            public string ReCaptchaPrivateKey { get; set; }
        }

        public class LocalizationSettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseImagesForLanguageSelection")]
            public bool UseImagesForLanguageSelection { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SeoFriendlyUrlsForLanguagesEnabled")]
            public bool SeoFriendlyUrlsForLanguagesEnabled { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AutomaticallyDetectLanguage")]
            public bool AutomaticallyDetectLanguage { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup")]
            public bool LoadAllLocaleRecordsOnStartup { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllLocalizedPropertiesOnStartup")]
            public bool LoadAllLocalizedPropertiesOnStartup { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllUrlRecordsOnStartup")]
            public bool LoadAllUrlRecordsOnStartup { get; set; }
        }

        public class FullTextSettingsModel : BaseNopModel
        {
            public bool Supported { get; set; }
            public bool Enabled { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.SearchMode")]
            public int SearchMode { get; set; }

            public SelectList SearchModeValues { get; set; }
        }

        public class SocialSettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FacebookAppId")]
            public string FacebookAppId { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FacebookAppSecret")]
            public string FacebookAppSecret { get; set; }
        }

        #endregion
    }
}