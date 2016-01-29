using TinyCms.Core.Configuration;

namespace TinyCms.Core.Domain
{
    public class StoreInformationSettings : ISettings
    {
        /// <summary>
        ///     Gets or sets a value indicating whether store is closed
        /// </summary>
        public bool StoreClosed { get; set; }

        /// <summary>
        ///     Gets or sets a default store theme
        /// </summary>
        public string DefaultStoreTheme { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether customers are allowed to select a theme
        /// </summary>
        public bool AllowCustomerToSelectTheme { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether mini profiler should be displayed in public store (used for debugging)
        /// </summary>
        public bool DisplayMiniProfilerInPublicStore { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether we should display warnings about the new EU cookie law
        /// </summary>
        public bool DisplayEuCookieLawWarning { get; set; }

        /// <summary>
        ///     Gets or sets a value of Facebook page URL of the site
        /// </summary>
        public string FacebookLink { get; set; }

        /// <summary>
        ///     Gets or sets a value of Twitter page URL of the site
        /// </summary>
        public string TwitterLink { get; set; }

        /// <summary>
        ///     Gets or sets a value of YouTube channel URL of the site
        /// </summary>
        public string YoutubeLink { get; set; }

        /// <summary>
        ///     Gets or sets a value of Google+ page URL of the site
        /// </summary>
        public string GooglePlusLink { get; set; }

        /// <summary>
        ///     Gets or sets the store name
        /// </summary>
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

        /// <summary>
        ///     Gets or sets the company VAT (used in Europe Union countries)
        /// </summary>
        public string CompanyVat { get; set; }
    }
}