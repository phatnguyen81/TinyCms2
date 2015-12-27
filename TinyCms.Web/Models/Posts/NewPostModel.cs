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
        public NewPostModel()
        {
            AvailableCategories = new List<SelectListItem>();
        }
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        public string Title { get; set; }

        [AllowHtml]
        [UIHint("RichEditor")]
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Body { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chuyên mục")]
        public int CategoryId { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; }

        [UIHint("Picture")]
        public string PictureIds { get; set; }

        public string PostTags { get; set; }
    }
}