using FluentValidation;
using TinyCms.Admin.Models.Messages;
using TinyCms.Services.Localization;
using TinyCms.Web.Framework.Validators;

namespace TinyCms.Admin.Validators.Messages
{
    public class QueuedEmailValidator : BaseNopValidator<QueuedEmailModel>
    {
        public QueuedEmailValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.From)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.System.QueuedEmails.Fields.From.Required"));
            RuleFor(x => x.To)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.System.QueuedEmails.Fields.To.Required"));

            RuleFor(x => x.SentTries)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.System.QueuedEmails.Fields.SentTries.Required"))
                .InclusiveBetween(0, 99999)
                .WithMessage(localizationService.GetResource("Admin.System.QueuedEmails.Fields.SentTries.Range"));
        }
    }
}