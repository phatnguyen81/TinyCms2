using TinyCms.Core.Domain.Logging;

namespace TinyCms.Data.Mapping.Logging
{
    public class ActivityLogTypeMap : NopEntityTypeConfiguration<ActivityLogType>
    {
        public ActivityLogTypeMap()
        {
            ToTable("ActivityLogType");
            HasKey(alt => alt.Id);

            Property(alt => alt.SystemKeyword).IsRequired().HasMaxLength(100);
            Property(alt => alt.Name).IsRequired().HasMaxLength(200);
        }
    }
}