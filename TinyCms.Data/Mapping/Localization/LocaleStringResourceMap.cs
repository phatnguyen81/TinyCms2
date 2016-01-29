using TinyCms.Core.Domain.Localization;

namespace TinyCms.Data.Mapping.Localization
{
    public class LocaleStringResourceMap : NopEntityTypeConfiguration<LocaleStringResource>
    {
        public LocaleStringResourceMap()
        {
            ToTable("LocaleStringResource");
            HasKey(lsr => lsr.Id);
            Property(lsr => lsr.ResourceName).IsRequired().HasMaxLength(200);
            Property(lsr => lsr.ResourceValue).IsRequired();


            HasRequired(lsr => lsr.Language)
                .WithMany(l => l.LocaleStringResources)
                .HasForeignKey(lsr => lsr.LanguageId);
        }
    }
}