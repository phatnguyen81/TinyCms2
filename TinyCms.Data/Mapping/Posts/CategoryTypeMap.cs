using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public partial class CategoryTypeMap : NopEntityTypeConfiguration<CategoryType>
    {
        public CategoryTypeMap()
        {
            this.ToTable("CategoryType");
            this.HasKey(cr => cr.Id);
            this.Property(cr => cr.Name).IsRequired().HasMaxLength(255);
            this.Property(cr => cr.SystemName).HasMaxLength(255);
        }
    }
}