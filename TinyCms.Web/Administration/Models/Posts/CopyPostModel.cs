using System.Web.Mvc;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Posts
{
    public partial class CopyPostModel : BaseNopEntityModel
    {

        [NopResourceDisplayName("Admin.Catalog.Posts.Copy.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Copy.CopyImages")]
        public bool CopyImages { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Copy.Published")]
        public bool Published { get; set; }

    }
}