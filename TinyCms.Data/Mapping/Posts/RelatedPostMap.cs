using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public class RelatedPostMap : NopEntityTypeConfiguration<RelatedPost>
    {
        public RelatedPostMap()
        {
            ToTable("RelatedPost");
            HasKey(c => c.Id);
        }
    }
}