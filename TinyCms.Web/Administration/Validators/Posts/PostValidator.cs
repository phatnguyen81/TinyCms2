using FluentValidation;
using TinyCms.Admin.Models.Posts;
using TinyCms.Services.Localization;
using TinyCms.Web.Framework.Validators;

namespace TinyCms.Admin.Validators.Posts
{
    public class PostValidator : BaseNopValidator<PostModel>
    {
        public PostValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Posts.Fields.Name.Required"));
        }
    }
}