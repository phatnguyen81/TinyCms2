using TinyCms.Web.Framework.Mvc;
using TinyCms.Web.Models.Media;

namespace TinyCms.Web.Models.Posts
{
    public partial class PostOverviewModel : BaseNopEntityModel
    {
        public PostOverviewModel()
        {
            DefaultPictureModel = new PictureModel();
        }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string SeName { get; set; }

        public bool MarkAsNew { get; set; }

        //picture
        public PictureModel DefaultPictureModel { get; set; }

    }
}