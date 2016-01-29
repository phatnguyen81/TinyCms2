using System.Collections.Generic;
using System.Web.Mvc;
using TinyCms.Web.Models.Posts;

namespace TinyCms.Web.Models.Customer
{
    public class CustomerProfileModel
    {
        public CustomerProfileModel()
        {
            PagingFilteringContext = new ProfilePagingFilteringModel();
            StatusList = new List<SelectListItem>
            {
                new SelectListItem {Text = "Tất cả", Value = string.Empty},
                new SelectListItem {Text = "Đã duyệt", Value = "Approved"},
                new SelectListItem {Text = "Chưa duyệt", Value = "NotApprove"}
            };
        }

        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int PostCount { get; set; }
        public string AvatarUrl { get; set; }
        public int TotalPosts { get; set; }
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public List<PostOverviewModel> Posts { get; set; }
        public ProfilePagingFilteringModel PagingFilteringContext { get; set; }
    }
}