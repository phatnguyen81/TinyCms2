using FluentValidation;
using TinyCms.Admin.Models.Templates;
using TinyCms.Services.Localization;
using TinyCms.Web.Framework.Validators;

namespace TinyCms.Admin.Validators.Templates
{
    public class PostTemplateValidator : BaseNopValidator<PostTemplateModel>
    {
        public PostTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.System.Templates.Product.Name.Required"));
            RuleFor(x => x.ViewPath)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.System.Templates.Product.ViewPath.Required"));
        }
    }
}