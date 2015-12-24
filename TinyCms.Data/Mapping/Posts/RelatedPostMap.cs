using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public partial class RelatedPostMap : NopEntityTypeConfiguration<RelatedPost>
    {
        public RelatedPostMap()
        {
            this.ToTable("RelatedPost");
            this.HasKey(c => c.Id);
        }
    }
}