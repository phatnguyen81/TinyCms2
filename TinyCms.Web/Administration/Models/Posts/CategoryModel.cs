using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using TinyCms.Admin.Models.Customers;
using TinyCms.Admin.Validators.Posts;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Localization;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Posts
{
    [Validator(typeof (CategoryValidator))]
    public class CategoryModel : BaseNopEntityModel, ILocalizedModel<CategoryLocalizedModel>
    {
        public CategoryModel()
        {
            Locales = new List<CategoryLocalizedModel>();
            AvailableCategories = new List<SelectListItem>();
            AvailableCategoryTemplates = new List<SelectListItem>();
            AvailableCategoryTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.CategoryTemplate")]
        public int CategoryTemplateId { get; set; }

        public IList<SelectListItem> AvailableCategoryTemplates { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.CategoryType")]
        public int CategoryTypeId { get; set; }

        public IList<SelectListItem> AvailableCategoryTypes { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Parent")]
        public int ParentCategoryId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.IncludeInTopMenu")]
        public bool IncludeInTopMenu { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Deleted")]
        public bool Deleted { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public string Breadcrumb { get; set; }
        //ACL
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SubjectToAcl")]
        public bool SubjectToAcl { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.AclCustomerRoles")]
        public List<CustomerRoleModel> AvailableCustomerRoles { get; set; }

        public int[] SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<CategoryLocalizedModel> Locales { get; set; }

        #region Nested classes

        public class CategoryPostModel : BaseNopEntityModel
        {
            public int CategoryId { get; set; }
            public int PostId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Posts.Fields.Post")]
            public string PostName { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Posts.Fields.IsFeaturedPost")]
            public bool IsFeaturedPost { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Posts.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public class AddCategoryPostModel : BaseNopModel
        {
            public AddCategoryPostModel()
            {
                AvailableCategories = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Posts.List.SearchPostName")]
            [AllowHtml]
            public string SearchPostName { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Posts.List.SearchCategory")]
            public int SearchCategoryId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public int CategoryId { get; set; }
            public int[] SelectedPostIds { get; set; }
        }

        #endregion
    }

    public class CategoryLocalizedModel : ILocalizedModelLocal
    {
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        public int LanguageId { get; set; }
    }
}