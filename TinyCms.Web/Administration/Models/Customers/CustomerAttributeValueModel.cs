using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using TinyCms.Admin.Validators.Customers;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Localization;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Customers
{
    [Validator(typeof (CustomerAttributeValueValidator))]
    public class CustomerAttributeValueModel : BaseNopEntityModel, ILocalizedModel<CustomerAttributeValueLocalizedModel>
    {
        public CustomerAttributeValueModel()
        {
            Locales = new List<CustomerAttributeValueLocalizedModel>();
        }

        public int CustomerAttributeId { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<CustomerAttributeValueLocalizedModel> Locales { get; set; }
    }

    public class CustomerAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        public int LanguageId { get; set; }
    }
}