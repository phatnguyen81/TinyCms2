using System.Web.Mvc;
using FluentValidation.Attributes;
using TinyCms.Admin.Validators.Templates;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Templates
{
    [Validator(typeof(PostTemplateValidator))]
    public partial class PostTemplateModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.System.Templates.Product.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.System.Templates.Product.ViewPath")]
        [AllowHtml]
        public string ViewPath { get; set; }

        [NopResourceDisplayName("Admin.System.Templates.Product.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}