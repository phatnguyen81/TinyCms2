using TinyCms.Data.Mapping;
using Nop.Plugin.Widgets.AdsBanner.Domain;

namespace Nop.Plugin.Widgets.AdsBanner.Data
{
    public partial class AdsBannerRecordMap : NopEntityTypeConfiguration<AdsBannerRecord>
    {
        public AdsBannerRecordMap()
        {
            this.ToTable("AdsBannerRecord");
            this.HasKey(pr => pr.Id);
            this.Property(pr => pr.Name).IsRequired();

            this.HasRequired(pp => pp.Picture)
              .WithMany()
              .HasForeignKey(pp => pp.PictureId);

        }
    }
}
