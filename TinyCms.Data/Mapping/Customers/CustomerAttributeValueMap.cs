using TinyCms.Core.Domain.Customers;

namespace TinyCms.Data.Mapping.Customers
{
    public class CustomerAttributeValueMap : NopEntityTypeConfiguration<CustomerAttributeValue>
    {
        public CustomerAttributeValueMap()
        {
            ToTable("CustomerAttributeValue");
            HasKey(cav => cav.Id);
            Property(cav => cav.Name).IsRequired().HasMaxLength(400);

            HasRequired(cav => cav.CustomerAttribute)
                .WithMany(ca => ca.CustomerAttributeValues)
                .HasForeignKey(cav => cav.CustomerAttributeId);
        }
    }
}