using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public partial class PostMap : NopEntityTypeConfiguration<Post>
    {
        public PostMap()
        {
            this.ToTable("Post");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
            this.Property(p => p.MetaKeywords).HasMaxLength(400);
            this.Property(p => p.MetaTitle).HasMaxLength(400);
            this.HasMany(p => p.PostTags)
                .WithMany(pt => pt.Posts)
                .Map(m => m.ToTable("Post_PostTag_Mapping"));
        }
    }
}