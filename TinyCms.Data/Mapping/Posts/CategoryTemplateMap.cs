using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public partial class CategoryTemplateMap : NopEntityTypeConfiguration<CategoryTemplate>
    {
        public CategoryTemplateMap()
        {
            this.ToTable("CategoryTemplate");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
            this.Property(p => p.ViewPath).IsRequired().HasMaxLength(400);
        }
    }
}