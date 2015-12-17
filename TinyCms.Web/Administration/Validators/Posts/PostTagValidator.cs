using FluentValidation;
using TinyCms.Admin.Models.Posts;
using TinyCms.Services.Localization;
using TinyCms.Web.Framework.Validators;

namespace TinyCms.Admin.Validators.Posts
{
    public class PostTagValidator : BaseNopValidator<PostTagModel>
    {
        public PostTagValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.ProductTags.Fields.Name.Required"));
        }
    }
}