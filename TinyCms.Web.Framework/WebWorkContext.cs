using System;
using System.Linq;
using System.Web;
using TinyCms.Core;
using TinyCms.Core.Domain;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Localization;
using TinyCms.Core.Fakes;
using TinyCms.Core.Infrastructure;
using TinyCms.Services.Authentication;
using TinyCms.Services.Common;
using TinyCms.Services.Customers;
using TinyCms.Services.Helpers;
using TinyCms.Services.Localization;
using TinyCms.Web.Framework.Localization;

namespace TinyCms.Web.Framework
{
    /// <summary>
    ///     Work context for web application
    /// </summary>
    public class WebWorkContext : IWorkContext
    {
        #region Const

        private const string CustomerCookieName = "Nop.customer";

        #endregion

        #region Ctor

        public WebWorkContext(HttpContextBase httpContext,
            ICustomerService customerService,
            ILanguageService languageService,
            IGenericAttributeService genericAttributeService,
            LocalizationSettings localizationSettings,
            IUserAgentHelper userAgentHelper,
            IAuthenticationService authenticationService)
        {
            _httpContext = httpContext;
            _customerService = customerService;
            _languageService = languageService;
            _genericAttributeService = genericAttributeService;
            _localizationSettings = localizationSettings;
            _userAgentHelper = userAgentHelper;
            _authenticationService = authenticationService;
        }

        #endregion

        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly ICustomerService _customerService;
        private readonly ILanguageService _languageService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IAuthenticationService _authenticationService;

        private Customer _cachedCustomer;
        private Customer _originalCustomerIfImpersonated;
        private Language _cachedLanguage;

        #endregion

        #region Utilities

        protected virtual HttpCookie GetCustomerCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[CustomerCookieName];
        }

        protected virtual void SetCustomerCookie(Guid customerGuid)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(CustomerCookieName);
                cookie.HttpOnly = true;
                cookie.Value = customerGuid.ToString();
                if (customerGuid == Guid.Empty)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    var cookieExpires = 24*365; //TODO make configurable
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                _httpContext.Response.Cookies.Remove(CustomerCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        protected virtual Language GetLanguageFromUrl()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            var virtualPath = _httpContext.Request.AppRelativeCurrentExecutionFilePath;
            var applicationPath = _httpContext.Request.ApplicationPath;
            if (!virtualPath.IsLocalizedUrl(applicationPath, false))
                return null;

            var seoCode = virtualPath.GetLanguageSeoCodeFromUrl(applicationPath, false);
            if (String.IsNullOrEmpty(seoCode))
                return null;

            var language = _languageService
                .GetAllLanguages()
                .FirstOrDefault(l => seoCode.Equals(l.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
            if (language != null && language.Published)
            {
                return language;
            }

            return null;
        }

        protected virtual Language GetLanguageFromBrowserSettings()
        {
            if (_httpContext == null ||
                _httpContext.Request == null ||
                _httpContext.Request.UserLanguages == null)
                return null;

            var userLanguage = _httpContext.Request.UserLanguages.FirstOrDefault();
            if (String.IsNullOrEmpty(userLanguage))
                return null;

            var language = _languageService
                .GetAllLanguages()
                .FirstOrDefault(l => userLanguage.Equals(l.LanguageCulture, StringComparison.InvariantCultureIgnoreCase));
            if (language != null && language.Published)
            {
                return language;
            }

            return null;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the current customer
        /// </summary>
        public virtual Customer CurrentCustomer
        {
            get
            {
                if (_cachedCustomer != null)
                    return _cachedCustomer;

                Customer customer = null;
                if (_httpContext == null || _httpContext is FakeHttpContext)
                {
                    //check whether request is made by a background task
                    //in this case return built-in customer record for background task
                    customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.BackgroundTask);
                }

                //check whether request is made by a search engine
                //in this case return built-in customer record for search engines 
                //or comment the following two lines of code in order to disable this functionality
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    if (_userAgentHelper.IsSearchEngine())
                        customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.SearchEngine);
                }

                //registered user
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    customer = _authenticationService.GetAuthenticatedCustomer();
                }

                //impersonate user if required (currently used for 'phone order' support)
                if (customer != null && !customer.Deleted && customer.Active)
                {
                    var impersonatedCustomerId =
                        customer.GetAttribute<int?>(SystemCustomerAttributeNames.ImpersonatedCustomerId);
                    if (impersonatedCustomerId.HasValue && impersonatedCustomerId.Value > 0)
                    {
                        var impersonatedCustomer = _customerService.GetCustomerById(impersonatedCustomerId.Value);
                        if (impersonatedCustomer != null && !impersonatedCustomer.Deleted && impersonatedCustomer.Active)
                        {
                            //set impersonated customer
                            _originalCustomerIfImpersonated = customer;
                            customer = impersonatedCustomer;
                        }
                    }
                }

                //load guest customer
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    var customerCookie = GetCustomerCookie();
                    if (customerCookie != null && !String.IsNullOrEmpty(customerCookie.Value))
                    {
                        Guid customerGuid;
                        if (Guid.TryParse(customerCookie.Value, out customerGuid))
                        {
                            var customerByCookie = _customerService.GetCustomerByGuid(customerGuid);
                            if (customerByCookie != null &&
                                //this customer (from cookie) should not be registered
                                !customerByCookie.IsRegistered())
                                customer = customerByCookie;
                        }
                    }
                }

                //create guest if not exists
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    customer = _customerService.InsertGuestCustomer();
                }


                //validation
                if (!customer.Deleted && customer.Active)
                {
                    SetCustomerCookie(customer.CustomerGuid);
                    _cachedCustomer = customer;
                }

                return _cachedCustomer;
            }
            set
            {
                SetCustomerCookie(value.CustomerGuid);
                _cachedCustomer = value;
            }
        }

        /// <summary>
        ///     Gets or sets the original customer (in case the current one is impersonated)
        /// </summary>
        public virtual Customer OriginalCustomerIfImpersonated
        {
            get { return _originalCustomerIfImpersonated; }
        }

        /// <summary>
        ///     Get or set current user working language
        /// </summary>
        public virtual Language WorkingLanguage
        {
            get
            {
                if (_cachedLanguage != null)
                    return _cachedLanguage;

                Language detectedLanguage = null;
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //get language from URL
                    detectedLanguage = GetLanguageFromUrl();
                }
                if (detectedLanguage == null && _localizationSettings.AutomaticallyDetectLanguage)
                {
                    //get language from browser settings
                    //but we do it only once
                    if (!CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.LanguageAutomaticallyDetected,
                        _genericAttributeService))
                    {
                        detectedLanguage = GetLanguageFromBrowserSettings();
                        if (detectedLanguage != null)
                        {
                            _genericAttributeService.SaveAttribute(CurrentCustomer,
                                SystemCustomerAttributeNames.LanguageAutomaticallyDetected,
                                true);
                        }
                    }
                }
                if (detectedLanguage != null)
                {
                    //the language is detected. now we need to save it
                    if (CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId,
                        _genericAttributeService) != detectedLanguage.Id)
                    {
                        _genericAttributeService.SaveAttribute(CurrentCustomer, SystemCustomerAttributeNames.LanguageId,
                            detectedLanguage.Id);
                    }
                }

                var allLanguages = _languageService.GetAllLanguages();
                //find current customer language
                var languageId = CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId,
                    _genericAttributeService);
                var language = allLanguages.FirstOrDefault(x => x.Id == languageId);
                if (language == null)
                {
                    //it not found, then let's load the default currency for the current language (if specified)
                    languageId = EngineContext.Current.Resolve<StoreInformationSettings>().DefaultLanguageId;
                    language = allLanguages.FirstOrDefault(x => x.Id == languageId);
                }
                if (language == null)
                {
                    //it not specified, then return the first (filtered by current store) found one
                    language = allLanguages.FirstOrDefault();
                }
                if (language == null)
                {
                    //it not specified, then return the first found one
                    language = _languageService.GetAllLanguages().FirstOrDefault();
                }

                //cache
                _cachedLanguage = language;
                return _cachedLanguage;
            }
            set
            {
                var languageId = value != null ? value.Id : 0;
                _genericAttributeService.SaveAttribute(CurrentCustomer,
                    SystemCustomerAttributeNames.LanguageId,
                    languageId);

                //reset cache
                _cachedLanguage = null;
            }
        }


        /// <summary>
        ///     Get or set value indicating whether we're in admin area
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        #endregion
    }
}