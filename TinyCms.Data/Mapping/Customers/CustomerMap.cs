using TinyCms.Core.Domain.Customers;

namespace TinyCms.Data.Mapping.Customers
{
    public class CustomerMap : NopEntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            ToTable("Customer");
            HasKey(c => c.Id);
            Property(u => u.Username).HasMaxLength(1000);
            Property(u => u.Email).HasMaxLength(1000);
            Property(u => u.SystemName).HasMaxLength(400);

            Ignore(u => u.PasswordFormat);

            HasMany(c => c.CustomerRoles)
                .WithMany()
                .Map(m => m.ToTable("Customer_CustomerRole_Mapping"));
        }
    }
}