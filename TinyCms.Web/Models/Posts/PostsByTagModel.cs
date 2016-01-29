using System.Collections.Generic;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Web.Models.Posts
{
    public class PostsByTagModel : BaseNopEntityModel
    {
        public PostsByTagModel()
        {
            Posts = new List<PostOverviewModel>();
            PagingFilteringContext = new PostsPagingFilteringModel();
        }

        public string TagName { get; set; }
        public string TagSeName { get; set; }
        public PostsPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<PostOverviewModel> Posts { get; set; }
    }
}