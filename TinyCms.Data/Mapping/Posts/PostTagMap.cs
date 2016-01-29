using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public class PostTagMap : NopEntityTypeConfiguration<PostTag>
    {
        public PostTagMap()
        {
            ToTable("PostTag");
            HasKey(pt => pt.Id);
            Property(pt => pt.Name).IsRequired().HasMaxLength(400);
        }
    }
}