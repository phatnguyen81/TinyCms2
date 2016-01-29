using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public class PostTemplateMap : NopEntityTypeConfiguration<PostTemplate>
    {
        public PostTemplateMap()
        {
            ToTable("PostTemplate");
            HasKey(p => p.Id);
            Property(p => p.Name).IsRequired().HasMaxLength(400);
            Property(p => p.ViewPath).IsRequired().HasMaxLength(400);
        }
    }
}