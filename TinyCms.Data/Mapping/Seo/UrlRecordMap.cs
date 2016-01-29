using TinyCms.Core.Domain.Seo;

namespace TinyCms.Data.Mapping.Seo
{
    public class UrlRecordMap : NopEntityTypeConfiguration<UrlRecord>
    {
        public UrlRecordMap()
        {
            ToTable("UrlRecord");
            HasKey(lp => lp.Id);

            Property(lp => lp.EntityName).IsRequired().HasMaxLength(400);
            Property(lp => lp.Slug).IsRequired().HasMaxLength(400);
        }
    }
}