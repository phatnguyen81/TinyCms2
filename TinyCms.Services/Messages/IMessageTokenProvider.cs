using System.Collections.Generic;
using TinyCms.Core.Domain;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Messages;

namespace TinyCms.Services.Messages
{
    public interface IMessageTokenProvider
    {
        void AddStoreTokens(IList<Token> tokens, StoreInformationSettings store, EmailAccount emailAccount);
        void AddCustomerTokens(IList<Token> tokens, Customer customer);
        void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription);
        string[] GetListOfCampaignAllowedTokens();
        string[] GetListOfAllowedTokens();
    }
}