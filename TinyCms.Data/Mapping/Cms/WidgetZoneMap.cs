using TinyCms.Core.Domain.Cms;

namespace TinyCms.Data.Mapping.Cms
{
    public class WidgetZoneMap : NopEntityTypeConfiguration<WidgetZone>
    {
        public WidgetZoneMap()
        {
            ToTable("WidgetZone");
            HasKey(pr => pr.Id);
            Property(pr => pr.Name).IsRequired();
            Property(pr => pr.SystemName).IsRequired().HasMaxLength(255);
        }
    }
}