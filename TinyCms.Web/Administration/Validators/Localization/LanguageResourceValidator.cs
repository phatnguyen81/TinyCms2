using FluentValidation;
using TinyCms.Admin.Models.Localization;
using TinyCms.Services.Localization;
using TinyCms.Web.Framework.Validators;

namespace TinyCms.Admin.Validators.Localization
{
    public class LanguageResourceValidator : BaseNopValidator<LanguageResourceModel>
    {
        public LanguageResourceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(
                    localizationService.GetResource("Admin.Configuration.Languages.Resources.Fields.Name.Required"));
            RuleFor(x => x.Value)
                .NotEmpty()
                .WithMessage(
                    localizationService.GetResource("Admin.Configuration.Languages.Resources.Fields.Value.Required"));
        }
    }
}