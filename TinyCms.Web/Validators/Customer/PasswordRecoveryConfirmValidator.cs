using FluentValidation;
using TinyCms.Core.Domain.Customers;
using TinyCms.Services.Localization;
using TinyCms.Web.Framework.Validators;
using TinyCms.Web.Models.Customer;

namespace TinyCms.Web.Validators.Customer
{
    public class PasswordRecoveryConfirmValidator : BaseNopValidator<PasswordRecoveryConfirmModel>
    {
        public PasswordRecoveryConfirmValidator(ILocalizationService localizationService,
            CustomerSettings customerSettings)
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Account.PasswordRecovery.NewPassword.Required"));
            RuleFor(x => x.NewPassword)
                .Length(customerSettings.PasswordMinLength, 999)
                .WithMessage(
                    string.Format(
                        localizationService.GetResource("Account.PasswordRecovery.NewPassword.LengthValidation"),
                        customerSettings.PasswordMinLength));
            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Account.PasswordRecovery.ConfirmNewPassword.Required"));
            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword)
                .WithMessage(
                    localizationService.GetResource("Account.PasswordRecovery.NewPassword.EnteredPasswordsDoNotMatch"));
        }
    }
}