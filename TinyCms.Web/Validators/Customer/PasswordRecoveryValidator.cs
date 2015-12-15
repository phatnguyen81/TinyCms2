using FluentValidation;
using TinyCms.Services.Localization;
using TinyCms.Web.Framework.Validators;
using TinyCms.Web.Models.Customer;

namespace TinyCms.Web.Validators.Customer
{
    public class PasswordRecoveryValidator : BaseNopValidator<PasswordRecoveryModel>
    {
        public PasswordRecoveryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.PasswordRecovery.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }}
}