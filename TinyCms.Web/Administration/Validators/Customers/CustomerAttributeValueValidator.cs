using FluentValidation;
using TinyCms.Admin.Models.Customers;
using TinyCms.Services.Localization;
using TinyCms.Web.Framework.Validators;

namespace TinyCms.Admin.Validators.Customers
{
    public class CustomerAttributeValueValidator : BaseNopValidator<CustomerAttributeValueModel>
    {
        public CustomerAttributeValueValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(
                    localizationService.GetResource("Admin.Customers.CustomerAttributes.Values.Fields.Name.Required"));
        }
    }
}