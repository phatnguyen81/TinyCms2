using TinyCms.Core.Domain.Tasks;

namespace TinyCms.Data.Mapping.Tasks
{
    public class ScheduleTaskMap : NopEntityTypeConfiguration<ScheduleTask>
    {
        public ScheduleTaskMap()
        {
            ToTable("ScheduleTask");
            HasKey(t => t.Id);
            Property(t => t.Name).IsRequired();
            Property(t => t.Type).IsRequired();
        }
    }
}