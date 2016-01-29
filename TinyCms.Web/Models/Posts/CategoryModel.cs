using System.Collections.Generic;
using TinyCms.Web.Framework.Mvc;
using TinyCms.Web.Models.Media;

namespace TinyCms.Web.Models.Posts
{
    public class CategoryModel : BaseNopEntityModel
    {
        public CategoryModel()
        {
            PictureModel = new PictureModel();
            FeaturedPosts = new List<PostOverviewModel>();
            Posts = new List<PostOverviewModel>();
            SubCategories = new List<SubCategoryModel>();
            CategoryBreadcrumb = new List<CategoryModel>();

            PagingFilteringContext = new PostsPagingFilteringModel();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public PictureModel PictureModel { get; set; }
        public bool DisplayCategoryBreadcrumb { get; set; }
        public IList<CategoryModel> CategoryBreadcrumb { get; set; }
        public CategoryModel ParentCategory { get; set; }
        public IList<SubCategoryModel> SubCategories { get; set; }
        public IList<PostOverviewModel> FeaturedPosts { get; set; }
        public IList<PostOverviewModel> Posts { get; set; }
        public PostsPagingFilteringModel PagingFilteringContext { get; set; }

        #region Nested Classes

        public class SubCategoryModel : BaseNopEntityModel
        {
            public SubCategoryModel()
            {
                PictureModel = new PictureModel();
            }

            public string Name { get; set; }
            public string SeName { get; set; }
            public string Description { get; set; }
            public PictureModel PictureModel { get; set; }
        }

        #endregion
    }


    public class CategoryBoxModel : BaseNopEntityModel
    {
        public CategoryBoxModel()
        {
            Posts = new List<PostOverviewModel>();
        }

        public string Name { get; set; }
        public string SeName { get; set; }
        public List<PostOverviewModel> Posts { get; set; }
    }
}