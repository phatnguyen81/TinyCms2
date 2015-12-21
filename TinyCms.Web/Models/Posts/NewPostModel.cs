using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TinyCms.Web.Models.Posts
{
    public class NewPostModel
    {
        public string Title { get; set; }

        [AllowHtml]
        [UIHint("RichEditor")]
        public string Body { get; set; }

        public int CategoryId { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; }

        [UIHint("Picture")]
        public int PictureId1 { get; set; }

        [UIHint("Picture")]
        public int PictureId2 { get; set; }

        [UIHint("Picture")]
        public int PictureId3 { get; set; }

        [UIHint("Picture")]
        public int PictureId4 { get; set; }

        [UIHint("Picture")]
        public int PictureId5 { get; set; }

        [UIHint("Picture")]
        public string PostTags { get; set; }
    }
}