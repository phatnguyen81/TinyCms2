using Nop.Plugin.Widgets.AdsBanner.Domain;
using TinyCms.Data.Mapping;

namespace Nop.Plugin.Widgets.AdsBanner.Data
{
    public class AdsBannerRecordMap : NopEntityTypeConfiguration<AdsBannerRecord>
    {
        public AdsBannerRecordMap()
        {
            ToTable("AdsBannerRecord");
            HasKey(pr => pr.Id);
            Property(pr => pr.Name).IsRequired();

            HasRequired(pp => pp.Picture)
                .WithMany()
                .HasForeignKey(pp => pp.PictureId);
        }
    }
}