using TinyCms.Core.Domain.Customers;

namespace TinyCms.Data.Mapping.Customers
{
    public class ExternalAuthenticationRecordMap : NopEntityTypeConfiguration<ExternalAuthenticationRecord>
    {
        public ExternalAuthenticationRecordMap()
        {
            ToTable("ExternalAuthenticationRecord");

            HasKey(ear => ear.Id);

            HasRequired(ear => ear.Customer)
                .WithMany(c => c.ExternalAuthenticationRecords)
                .HasForeignKey(ear => ear.CustomerId);
        }
    }
}