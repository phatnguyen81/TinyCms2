using TinyCms.Core.Domain.Media;

namespace TinyCms.Data.Mapping.Media
{
    public class PictureMap : NopEntityTypeConfiguration<Picture>
    {
        public PictureMap()
        {
            ToTable("Picture");
            HasKey(p => p.Id);
            Property(p => p.PictureBinary).IsMaxLength();
            Property(p => p.MimeType).IsRequired().HasMaxLength(40);
            Property(p => p.SeoFilename).HasMaxLength(300);
        }
    }
}