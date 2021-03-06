using System;
using System.Linq;
using TinyCms.Core.Data;
using TinyCms.Core.Domain.Customers;
using TinyCms.Services.Helpers;

namespace TinyCms.Services.Customers
{
    /// <summary>
    ///     Customer report service
    /// </summary>
    public class CustomerReportService : ICustomerReportService
    {
        #region Ctor

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="dateTimeHelper">Date time helper</param>
        public CustomerReportService(IRepository<Customer> customerRepository,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper)
        {
            _customerRepository = customerRepository;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets a report of customers registered in the last days
        /// </summary>
        /// <param name="days">Customers registered in the last days</param>
        /// <returns>Number of registered customers</returns>
        public virtual int GetRegisteredCustomersReport(int days)
        {
            var date = _dateTimeHelper.ConvertToUserTime(DateTime.Now).AddDays(-days);

            var registeredCustomerRole = _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered);
            if (registeredCustomerRole == null)
                return 0;

            var query = from c in _customerRepository.Table
                from cr in c.CustomerRoles
                where !c.Deleted &&
                      cr.Id == registeredCustomerRole.Id &&
                      c.CreatedOnUtc >= date
                //&& c.CreatedOnUtc <= DateTime.UtcNow
                select c;
            var count = query.Count();
            return count;
        }

        #endregion

        #region Fields

        private readonly IRepository<Customer> _customerRepository;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion
    }
}