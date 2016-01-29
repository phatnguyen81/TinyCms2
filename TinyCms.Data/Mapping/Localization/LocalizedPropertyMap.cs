using TinyCms.Core.Domain.Localization;

namespace TinyCms.Data.Mapping.Localization
{
    public class LocalizedPropertyMap : NopEntityTypeConfiguration<LocalizedProperty>
    {
        public LocalizedPropertyMap()
        {
            ToTable("LocalizedProperty");
            HasKey(lp => lp.Id);

            Property(lp => lp.LocaleKeyGroup).IsRequired().HasMaxLength(400);
            Property(lp => lp.LocaleKey).IsRequired().HasMaxLength(400);
            Property(lp => lp.LocaleValue).IsRequired();

            HasRequired(lp => lp.Language)
                .WithMany()
                .HasForeignKey(lp => lp.LanguageId);
        }
    }
}