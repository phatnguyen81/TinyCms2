using System.Collections.Generic;
using System.IO;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Messages;

namespace TinyCms.Services.ExportImport
{
    /// <summary>
    /// Export manager interface
    /// </summary>
    public partial interface IExportManager
    {
      
        
        /// <summary>
        /// Export customer list to XLSX
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="customers">Customers</param>
        void ExportCustomersToXlsx(Stream stream, IList<Customer> customers);
        
        /// <summary>
        /// Export customer list to xml
        /// </summary>
        /// <param name="customers">Customers</param>
        /// <returns>Result in XML format</returns>
        string ExportCustomersToXml(IList<Customer> customers);

        /// <summary>
        /// Export newsletter subscribers to TXT
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Result in TXT (string) format</returns>
        string ExportNewsletterSubscribersToTxt(IList<NewsLetterSubscription> subscriptions);

    }
}
