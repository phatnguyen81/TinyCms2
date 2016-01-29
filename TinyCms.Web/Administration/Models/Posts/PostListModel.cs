using System.Collections.Generic;
using System.Web.Mvc;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Posts
{
    public class PostListModel : BaseNopModel
    {
        public PostListModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailablePublishedOptions = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Posts.List.SearchPostName")]
        [AllowHtml]
        public string SearchPostName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.List.SearchCategory")]
        public int SearchCategoryId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.List.SearchIncludeSubCategories")]
        public bool SearchIncludeSubCategories { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.List.SearchPublished")]
        public int SearchPublishedId { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailablePublishedOptions { get; set; }
    }
}