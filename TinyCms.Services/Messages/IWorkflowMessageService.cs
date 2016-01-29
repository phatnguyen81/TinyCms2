using System.Collections.Generic;
using TinyCms.Core.Domain.Customers;

namespace TinyCms.Services.Messages
{
    public interface IWorkflowMessageService
    {
        #region Misc

        /// <summary>
        ///     Sends a test email
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <param name="sendToEmail">Send to email</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendTestEmail(int messageTemplateId, string sendToEmail,
            List<Token> tokens, int languageId);

        #endregion

        #region Customer workflow

        /// <summary>
        ///     Sends 'New customer' notification message to a store owner
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendCustomerRegisteredNotificationMessage(Customer customer, int languageId);

        /// <summary>
        ///     Sends a welcome message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendCustomerWelcomeMessage(Customer customer, int languageId);

        /// <summary>
        ///     Sends an email validation message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendCustomerEmailValidationMessage(Customer customer, int languageId);

        /// <summary>
        ///     Sends password recovery message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        int SendCustomerPasswordRecoveryMessage(Customer customer, int languageId);

        #endregion
    }
}