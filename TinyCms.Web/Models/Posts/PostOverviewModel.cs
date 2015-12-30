using System;
using System.EnterpriseServices.Internal;
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
        public int ViewCount { get; set; }
        public int CommentCount { get; set; }
        public int ShareCount { get; set; }

        public DateTime CreatedOn { get; set; }

        public int PostTemplateId { get; set; }

        public bool Publish { get; set; }
        //picture
        public PictureModel DefaultPictureModel { get; set; }

    }
}