using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public class PostCategoryMap : NopEntityTypeConfiguration<PostCategory>
    {
        public PostCategoryMap()
        {
            ToTable("Post_Category_Mapping");
            HasKey(pc => pc.Id);

            HasRequired(pc => pc.Category)
                .WithMany()
                .HasForeignKey(pc => pc.CategoryId);


            HasRequired(pc => pc.Post)
                .WithMany(p => p.PostCategories)
                .HasForeignKey(pc => pc.PostId);
        }
    }
}