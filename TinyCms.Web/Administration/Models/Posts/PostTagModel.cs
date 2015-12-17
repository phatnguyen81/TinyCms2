using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using TinyCms.Admin.Validators.Posts;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Localization;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Posts
{
    [Validator(typeof(PostTagValidator))]
    public partial class PostTagModel : BaseNopEntityModel, ILocalizedModel<PostTagLocalizedModel>
    {
        public PostTagModel()
        {
            Locales = new List<PostTagLocalizedModel>();
        }
        [NopResourceDisplayName("Admin.Catalog.PostTags.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.PostTags.Fields.PostCount")]
        public int PostCount { get; set; }

        public IList<PostTagLocalizedModel> Locales { get; set; }
    }

    public partial class PostTagLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.PostTags.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}