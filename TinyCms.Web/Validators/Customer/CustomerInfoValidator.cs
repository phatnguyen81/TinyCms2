using FluentValidation;
using TinyCms.Core.Domain.Customers;
using TinyCms.Services.Localization;
using TinyCms.Web.Framework.Validators;
using TinyCms.Web.Models.Customer;

namespace TinyCms.Web.Validators.Customer
{
    public class CustomerInfoValidator : BaseNopValidator<CustomerInfoModel>
    {
        public CustomerInfoValidator(ILocalizationService localizationService,
            CustomerSettings customerSettings)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Account.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));

            if (customerSettings.UsernamesEnabled && customerSettings.AllowUsersToChangeUsernames)
            {
                RuleFor(x => x.Username)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Account.Fields.Username.Required"));
            }
        }
    }
}