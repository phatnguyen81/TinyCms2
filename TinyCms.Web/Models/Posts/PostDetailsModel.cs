using System;
using System.Collections.Generic;
using TinyCms.Web.Framework.Mvc;
using TinyCms.Web.Models.Media;

namespace TinyCms.Web.Models.Posts
{
    public class PostDetailsModel : BaseNopEntityModel
    {
        public PostDetailsModel()
        {
            DefaultPictureModel = new PictureModel();
            PictureModels = new List<PictureModel>();
            PostTags = new List<PostTagModel>();
            Breadcrumb = new PostBreadcrumbModel();
        }

        //picture(s)
        public PictureModel DefaultPictureModel { get; set; }
        public IList<PictureModel> PictureModels { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string PostTemplateViewPath { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public DateTime CreatedOn { get; set; }
        public PostBreadcrumbModel Breadcrumb { get; set; }
        public IList<PostTagModel> PostTags { get; set; }

        #region Nested Classes

        public class PostBreadcrumbModel : BaseNopModel
        {
            public PostBreadcrumbModel()
            {
                CategoryBreadcrumb = new List<CategorySimpleModel>();
            }

            public bool Enabled { get; set; }
            public int PostId { get; set; }
            public string PostName { get; set; }
            public string PostSeName { get; set; }
            public IList<CategorySimpleModel> CategoryBreadcrumb { get; set; }
        }

        #endregion
    }
}