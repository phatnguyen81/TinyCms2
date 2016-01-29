using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public class CategoryTypeMap : NopEntityTypeConfiguration<CategoryType>
    {
        public CategoryTypeMap()
        {
            ToTable("CategoryType");
            HasKey(cr => cr.Id);
            Property(cr => cr.Name).IsRequired().HasMaxLength(255);
            Property(cr => cr.SystemName).HasMaxLength(255);
        }
    }
}