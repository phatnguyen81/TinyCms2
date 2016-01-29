using TinyCms.Core.Domain.Localization;

namespace TinyCms.Data.Mapping.Localization
{
    public class LanguageMap : NopEntityTypeConfiguration<Language>
    {
        public LanguageMap()
        {
            ToTable("Language");
            HasKey(l => l.Id);
            Property(l => l.Name).IsRequired().HasMaxLength(100);
            Property(l => l.LanguageCulture).IsRequired().HasMaxLength(20);
            Property(l => l.UniqueSeoCode).HasMaxLength(2);
            Property(l => l.FlagImageFileName).HasMaxLength(50);
        }
    }
}