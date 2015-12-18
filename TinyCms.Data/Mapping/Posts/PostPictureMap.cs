using TinyCms.Core.Domain.Posts;

namespace TinyCms.Data.Mapping.Posts
{
    public partial class PostPictureMap : NopEntityTypeConfiguration<PostPicture>
    {
        public PostPictureMap()
        {
            this.ToTable("Post_Picture_Mapping");
            this.HasKey(pp => pp.Id);
            
            this.HasRequired(pp => pp.Picture)
                .WithMany()
                .HasForeignKey(pp => pp.PictureId);


            this.HasRequired(pp => pp.Post)
                .WithMany(p => p.PostPictures)
                .HasForeignKey(pp => pp.PostId);
        }
    }
}