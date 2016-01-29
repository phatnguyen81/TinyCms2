using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public class CategoryTemplateMap : NopEntityTypeConfiguration<CategoryTemplate>
    {
        public CategoryTemplateMap()
        {
            ToTable("CategoryTemplate");
            HasKey(p => p.Id);
            Property(p => p.Name).IsRequired().HasMaxLength(400);
            Property(p => p.ViewPath).IsRequired().HasMaxLength(400);
        }
    }
}