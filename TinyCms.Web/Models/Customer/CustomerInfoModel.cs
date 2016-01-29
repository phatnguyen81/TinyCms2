using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;
using TinyCms.Web.Validators.Customer;

namespace TinyCms.Web.Models.Customer
{
    [Validator(typeof (CustomerInfoValidator))]
    public class CustomerInfoModel : BaseNopModel
    {
        public CustomerInfoModel()
        {
            AssociatedExternalAuthRecords = new List<AssociatedExternalAuthModel>();
            CustomerAttributes = new List<CustomerAttributeModel>();
        }

        [NopResourceDisplayName("Account.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        public bool CheckUsernameAvailabilityEnabled { get; set; }
        public bool AllowUsersToChangeUsernames { get; set; }
        public bool UsernamesEnabled { get; set; }

        [NopResourceDisplayName("Account.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        //external authentication
        [NopResourceDisplayName("Account.AssociatedExternalAuth")]
        public IList<AssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; }

        public int NumberOfExternalAuthenticationProviders { get; set; }
        public IList<CustomerAttributeModel> CustomerAttributes { get; set; }

        #region Nested classes

        public class AssociatedExternalAuthModel : BaseNopEntityModel
        {
            public string Email { get; set; }
            public string ExternalIdentifier { get; set; }
            public string AuthMethodName { get; set; }
        }

        #endregion
    }
}