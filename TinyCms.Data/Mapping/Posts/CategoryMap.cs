using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public class CategoryMap : NopEntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            ToTable("Category");
            HasKey(c => c.Id);
            Property(c => c.Name).IsRequired().HasMaxLength(400);
            Property(c => c.MetaKeywords).HasMaxLength(400);
            Property(c => c.MetaTitle).HasMaxLength(400);
        }
    }
}