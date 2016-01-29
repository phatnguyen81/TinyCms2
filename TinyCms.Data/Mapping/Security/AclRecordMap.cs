using TinyCms.Core.Domain.Security;

namespace TinyCms.Data.Mapping.Security
{
    public class AclRecordMap : NopEntityTypeConfiguration<AclRecord>
    {
        public AclRecordMap()
        {
            ToTable("AclRecord");
            HasKey(ar => ar.Id);

            Property(ar => ar.EntityName).IsRequired().HasMaxLength(400);

            HasRequired(ar => ar.CustomerRole)
                .WithMany()
                .HasForeignKey(ar => ar.CustomerRoleId)
                .WillCascadeOnDelete(true);
        }
    }
}