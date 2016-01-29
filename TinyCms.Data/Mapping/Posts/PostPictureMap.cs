using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public class PostPictureMap : NopEntityTypeConfiguration<PostPicture>
    {
        public PostPictureMap()
        {
            ToTable("Post_Picture_Mapping");
            HasKey(pp => pp.Id);

            HasRequired(pp => pp.Picture)
                .WithMany()
                .HasForeignKey(pp => pp.PictureId);


            HasRequired(pp => pp.Post)
                .WithMany(p => p.PostPictures)
                .HasForeignKey(pp => pp.PostId);
        }
    }
}