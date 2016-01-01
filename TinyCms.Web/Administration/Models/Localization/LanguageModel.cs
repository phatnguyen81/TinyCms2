using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using TinyCms.Admin.Models.Customers;
using TinyCms.Admin.Validators.Localization;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Localization
{
    [Validator(typeof(LanguageValidator))]
    public partial class LanguageModel : BaseNopEntityModel
    {
        public LanguageModel()
        {
            FlagFileNames = new List<string>();
        }

        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.LanguageCulture")]
        [AllowHtml]
        public string LanguageCulture { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.UniqueSeoCode")]
        [AllowHtml]
        public string UniqueSeoCode { get; set; }
        
        //flags
        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.FlagImageFileName")]
        [AllowHtml]
        public string FlagImageFileName { get; set; }
        public IList<string> FlagFileNames { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.Rtl")]
        public bool Rtl { get; set; }


        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Languages.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }


     
    }
}