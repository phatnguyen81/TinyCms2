using TinyCms.Core.Domain.Cms;

namespace TinyCms.Data.Mapping.Cms
{
    public partial class WidgetZoneMap : NopEntityTypeConfiguration<WidgetZone>
    {
        public WidgetZoneMap()
        {
            this.ToTable("WidgetZone");
            this.HasKey(pr => pr.Id);
            this.Property(pr => pr.Name).IsRequired();
            this.Property(pr => pr.SystemName).IsRequired().HasMaxLength(255);
        }
    }
}