using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using TinyCms.Core.Domain.Configuration;

namespace TinyCms.Data.Mapping.Configuration
{
    public partial class SettingMap : NopEntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            this.ToTable("Setting");
            this.HasKey(s => s.Id);
            this.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_Setting_Name", 1) {IsUnique = true}));
            this.Property(s => s.Value).IsRequired().HasMaxLength(2000);
        }
    }
}