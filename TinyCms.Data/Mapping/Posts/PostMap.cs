using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public class PostMap : NopEntityTypeConfiguration<Post>
    {
        public PostMap()
        {
            ToTable("Post");
            HasKey(p => p.Id);
            Property(p => p.Name).IsRequired().HasMaxLength(400);
            Property(p => p.MetaKeywords).HasMaxLength(400);
            Property(p => p.MetaTitle).HasMaxLength(400);
            HasMany(p => p.PostTags)
                .WithMany(pt => pt.Posts)
                .Map(m => m.ToTable("Post_PostTag_Mapping"));
        }
    }
}