using TinyCms.Core.Domain.Logging;

namespace TinyCms.Data.Mapping.Logging
{
    public class ActivityLogMap : NopEntityTypeConfiguration<ActivityLog>
    {
        public ActivityLogMap()
        {
            ToTable("ActivityLog");
            HasKey(al => al.Id);
            Property(al => al.Comment).IsRequired();

            HasRequired(al => al.ActivityLogType)
                .WithMany()
                .HasForeignKey(al => al.ActivityLogTypeId);

            HasRequired(al => al.Customer)
                .WithMany()
                .HasForeignKey(al => al.CustomerId);
        }
    }
}