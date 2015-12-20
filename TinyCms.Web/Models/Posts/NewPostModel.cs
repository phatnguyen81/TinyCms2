using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TinyCms.Web.Models.Posts
{
    public class NewPostModel
    {
        public string Title { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public int CategoryId { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; }

        public string PostTags { get; set; }
    }
}