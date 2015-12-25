using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TinyCms.Web.Models.Posts;

namespace TinyCms.Web.Models.Customer
{
    public class CustomerProfileModel
    {

        public CustomerProfileModel()
        {
            PagingFilteringContext = new ProfilePagingFilteringModel();
        }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int PostCount { get; set; }
        public string AvatarUrl { get; set; }

        public List<PostOverviewModel> Posts { get; set; }

        public ProfilePagingFilteringModel PagingFilteringContext { get; set; }
        //public PostOverview
    }
}