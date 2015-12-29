using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using TinyCms.Core;
using TinyCms.Core.Domain;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Messages;
using TinyCms.Core.Html;
using TinyCms.Core.Infrastructure;
using TinyCms.Services.Common;
using TinyCms.Services.Customers;
using TinyCms.Services.Events;
using TinyCms.Services.Localization;

namespace TinyCms.Services.Messages
{
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        private readonly MessageTemplatesSettings _templatesSettings;

        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public MessageTokenProvider(ILanguageService languageService,
            ILocalizationService localizationService, 
            IWorkContext workContext,
            MessageTemplatesSettings templatesSettings,
            IEventPublisher eventPublisher)
        {
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._workContext = workContext;

            this._templatesSettings = templatesSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        protected virtual string GetStoreUrl(int storeId = 0, bool useSsl = false)
        {
            var storeSettings = EngineContext.Current.Resolve<StoreInformationSettings>();

            return useSsl ? storeSettings.SecureUrl : storeSettings.Url;
        }

        #endregion

        #region Methods

        public void AddCustomerTokens(IList<Token> tokens, Customer customer)
        {
            tokens.Add(new Token("Customer.Email", customer.Email));
            tokens.Add(new Token("Customer.Username", customer.Username));
            tokens.Add(new Token("Customer.FullName", customer.GetFullName()));



            //note: we do not use SEO friendly URLS because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            string passwordRecoveryUrl = string.Format("{0}passwordrecovery/confirm?token={1}&email={2}", GetStoreUrl(), customer.GetAttribute<string>(SystemCustomerAttributeNames.PasswordRecoveryToken), HttpUtility.UrlEncode(customer.Email));
            string accountActivationUrl = string.Format("{0}customer/activation?token={1}&email={2}", GetStoreUrl(), customer.GetAttribute<string>(SystemCustomerAttributeNames.AccountActivationToken), HttpUtility.UrlEncode(customer.Email));
            tokens.Add(new Token("Customer.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("Customer.AccountActivationURL", accountActivationUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(customer, tokens);
        }

        public void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            throw new NotImplementedException();
        }

        public string[] GetListOfCampaignAllowedTokens()
        {
            throw new NotImplementedException();
        }

        public string[] GetListOfAllowedTokens()
        {
            var allowedTokens = new List<string>
            {
                "%Store.Name%",
                "%Store.URL%",
                "%Store.Email%",
                "%Store.CompanyName%",
                "%Store.CompanyAddress%",
                "%Store.CompanyPhoneNumber%",
                "%Customer.Email%", 
                "%Customer.Username%",
                "%Customer.FullName%",
                "%Customer.FirstName%",
                "%Customer.LastName%",
                "%Customer.PasswordRecoveryURL%", 
                "%Customer.AccountActivationURL%", 
                "%Post.ID%", 
                "%Post.Name%",
                "%Post.ShortDescription%", 
                "%Post.PostURLForCustomer%"
            };
            return allowedTokens.ToArray();
        }
        
        #endregion

      
    }
}
