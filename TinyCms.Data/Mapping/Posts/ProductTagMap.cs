using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public partial class ProductTagMap : NopEntityTypeConfiguration<PostTag>
    {
        public ProductTagMap()
        {
            this.ToTable("PostTag");
            this.HasKey(pt => pt.Id);
            this.Property(pt => pt.Name).IsRequired().HasMaxLength(400);
        }
    }
}