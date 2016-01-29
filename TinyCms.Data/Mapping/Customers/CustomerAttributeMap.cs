using TinyCms.Core.Domain.Customers;

namespace TinyCms.Data.Mapping.Customers
{
    public class CustomerAttributeMap : NopEntityTypeConfiguration<CustomerAttribute>
    {
        public CustomerAttributeMap()
        {
            ToTable("CustomerAttribute");
            HasKey(ca => ca.Id);
            Property(ca => ca.Name).IsRequired().HasMaxLength(400);

            Ignore(ca => ca.AttributeControlType);
        }
    }
}