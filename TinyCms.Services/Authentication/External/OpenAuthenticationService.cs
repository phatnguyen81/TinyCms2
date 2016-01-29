//Contributor:  Nicholas Mayne

using System;
using System.Collections.Generic;
using System.Linq;
using TinyCms.Core.Data;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Plugins;
using TinyCms.Services.Customers;

namespace TinyCms.Services.Authentication.External
{
    /// <summary>
    ///     Open authentication service
    /// </summary>
    public class OpenAuthenticationService : IOpenAuthenticationService
    {
        private readonly ICustomerService _customerService;
        private readonly IRepository<ExternalAuthenticationRecord> _externalAuthenticationRecordRepository;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IPluginFinder _pluginFinder;

        public OpenAuthenticationService(
            IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            ICustomerService customerService, IPluginFinder pluginFinder)
        {
            _externalAuthenticationRecordRepository = externalAuthenticationRecordRepository;
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _customerService = customerService;
            _pluginFinder = pluginFinder;
        }

        /// <summary>
        ///     Load active external authentication methods
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Payment methods</returns>
        public virtual IList<IExternalAuthenticationMethod> LoadActiveExternalAuthenticationMethods()
        {
            return LoadAllExternalAuthenticationMethods()
                .Where(
                    provider =>
                        _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Contains(
                            provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase))
                .ToList();
        }

        /// <summary>
        ///     Load external authentication method by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found external authentication method</returns>
        public virtual IExternalAuthenticationMethod LoadExternalAuthenticationMethodBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IExternalAuthenticationMethod>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IExternalAuthenticationMethod>();
            return null;
        }

        /// <summary>
        ///     Load all external authentication methods
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>External authentication methods</returns>
        public virtual IList<IExternalAuthenticationMethod> LoadAllExternalAuthenticationMethods()
        {
            return _pluginFinder
                .GetPlugins<IExternalAuthenticationMethod>()
                .ToList();
        }

        public virtual void AssociateExternalAccountWithUser(Customer customer, OpenAuthenticationParameters parameters)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            //find email
            string email = null;
            if (parameters.UserClaims != null)
                foreach (var userClaim in parameters.UserClaims
                    .Where(x => x.Contact != null && !String.IsNullOrEmpty(x.Contact.Email)))
                {
                    //found
                    email = userClaim.Contact.Email;
                    break;
                }

            var externalAuthenticationRecord = new ExternalAuthenticationRecord
            {
                CustomerId = customer.Id,
                Email = email,
                ExternalIdentifier = parameters.ExternalIdentifier,
                ExternalDisplayIdentifier = parameters.ExternalDisplayIdentifier,
                OAuthToken = parameters.OAuthToken,
                OAuthAccessToken = parameters.OAuthAccessToken,
                ProviderSystemName = parameters.ProviderSystemName
            };

            _externalAuthenticationRecordRepository.Insert(externalAuthenticationRecord);
        }

        public virtual bool AccountExists(OpenAuthenticationParameters parameters)
        {
            return GetUser(parameters) != null;
        }

        public virtual Customer GetUser(OpenAuthenticationParameters parameters)
        {
            var record = _externalAuthenticationRecordRepository.Table
                .FirstOrDefault(o => o.ExternalIdentifier == parameters.ExternalIdentifier &&
                                     o.ProviderSystemName == parameters.ProviderSystemName);

            if (record != null)
                return _customerService.GetCustomerById(record.CustomerId);

            return null;
        }

        public virtual IList<ExternalAuthenticationRecord> GetExternalIdentifiersFor(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            return customer.ExternalAuthenticationRecords.ToList();
        }

        public virtual void DeletExternalAuthenticationRecord(ExternalAuthenticationRecord externalAuthenticationRecord)
        {
            if (externalAuthenticationRecord == null)
                throw new ArgumentNullException("externalAuthenticationRecord");

            _externalAuthenticationRecordRepository.Delete(externalAuthenticationRecord);
        }

        public virtual void RemoveAssociation(OpenAuthenticationParameters parameters)
        {
            var record = _externalAuthenticationRecordRepository.Table
                .FirstOrDefault(o => o.ExternalIdentifier == parameters.ExternalIdentifier &&
                                     o.ProviderSystemName == parameters.ProviderSystemName);

            if (record != null)
                _externalAuthenticationRecordRepository.Delete(record);
        }
    }
}