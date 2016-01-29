using TinyCms.Core.Domain.Customers;

namespace TinyCms.Data.Mapping.Customers
{
    public class CustomerRoleMap : NopEntityTypeConfiguration<CustomerRole>
    {
        public CustomerRoleMap()
        {
            ToTable("CustomerRole");
            HasKey(cr => cr.Id);
            Property(cr => cr.Name).IsRequired().HasMaxLength(255);
            Property(cr => cr.SystemName).HasMaxLength(255);
        }
    }
}