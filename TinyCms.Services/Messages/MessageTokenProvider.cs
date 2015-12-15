using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using TinyCms.Core;
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

      

        #endregion

        #region Methods

        public void AddCustomerTokens(IList<Token> tokens, Customer customer)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        
        #endregion

      
    }
}
