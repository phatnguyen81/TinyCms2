using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Web.Models.Posts
{
    public partial class PostTagModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }

        public int PostCount { get; set; }
    }
}