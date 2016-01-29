using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using TinyCms.Admin.Validators.Customers;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Localization;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Customers
{
    [Validator(typeof (CustomerAttributeValidator))]
    public class CustomerAttributeModel : BaseNopEntityModel, ILocalizedModel<CustomerAttributeLocalizedModel>
    {
        public CustomerAttributeModel()
        {
            Locales = new List<CustomerAttributeLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.AttributeControlType")]
        public int AttributeControlTypeId { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.AttributeControlType")]
        [AllowHtml]
        public string AttributeControlTypeName { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<CustomerAttributeLocalizedModel> Locales { get; set; }
    }

    public class CustomerAttributeLocalizedModel : ILocalizedModelLocal
    {
        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        public int LanguageId { get; set; }
    }
}