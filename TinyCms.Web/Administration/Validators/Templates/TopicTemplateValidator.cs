using FluentValidation;
using TinyCms.Admin.Models.Templates;
using TinyCms.Services.Localization;
using TinyCms.Web.Framework.Validators;

namespace TinyCms.Admin.Validators.Templates
{
    public class TopicTemplateValidator : BaseNopValidator<TopicTemplateModel>
    {
        public TopicTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.System.Templates.Topic.Name.Required"));
            RuleFor(x => x.ViewPath)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.System.Templates.Topic.ViewPath.Required"));
        }
    }
}