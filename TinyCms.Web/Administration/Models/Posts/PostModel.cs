using System;
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
    [Validator(typeof (PostValidator))]
    public partial class PostModel : BaseNopEntityModel, ILocalizedModel<PostLocalizedModel>
    {
        public PostModel()
        {
            Locales = new List<PostLocalizedModel>();
            PostPictureModels = new List<PostPictureModel>();
            CopyPostModel = new CopyPostModel();
            AvailableCategories = new List<SelectListItem>();
            AddPictureModel = new PostPictureModel();
            AvailablePostTemplates = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.ID")]
        public override int Id { get; set; }

        //picture thumbnail
        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.PostTemplate")]
        public int PostTemplateId { get; set; }
        public IList<SelectListItem> AvailablePostTemplates { get; set; }
        
        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.PostTags")]
        public string PostTags { get; set; }

     
        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.CreatedBy")]
        public string CreatedByName { get; set; }
        public int CreatedBy { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.ApprovedBy")]
        public string ApprovedByName { get; set; }
        public int ApprovedBy { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.CreatedOn")]
        public DateTime? CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.ApprovedOn")]
        public DateTime? ApprovedOn { get; set; }

        public IList<PostLocalizedModel> Locales { get; set; }
        
        //ACL (customer roles)
        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.SubjectToAcl")]
        public bool SubjectToAcl { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.AclCustomerRoles")]
        public List<CustomerRoleModel> AvailableCustomerRoles { get; set; }

        public int[] SelectedCustomerRoleIds { get; set; }



        //categories
        public IList<SelectListItem> AvailableCategories { get; set; }
        
        //pictures
        public PostPictureModel AddPictureModel { get; set; }
        public IList<PostPictureModel> PostPictureModels { get; set; }
        
        //copy post
        public CopyPostModel CopyPostModel { get; set; }

        #region Nested classes

        public partial class RelatedPostModel : BaseNopEntityModel
        {
            public int PostId2 { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Posts.RelatedPosts.Fields.Post")]
            public string Post2Name { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Posts.RelatedPosts.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }
        public partial class AddRelatedPostModel : BaseNopModel
        {
            public AddRelatedPostModel()
            {
                AvailableCategories = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Posts.List.SearchPostName")]
            [AllowHtml]
            public string SearchPostName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Posts.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Posts.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Posts.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Posts.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Posts.List.SearchPostType")]
            public int SearchPostTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }

            public int PostId { get; set; }

            public int[] SelectedPostIds { get; set; }

        }

       
        public partial class PostPictureModel : BaseNopEntityModel
        {
            public int PostId { get; set; }

            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Catalog.Posts.Pictures.Fields.Picture")]
            public int PictureId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Posts.Pictures.Fields.Picture")]
            public string PictureUrl { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Posts.Pictures.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Posts.Pictures.Fields.OverrideAltAttribute")]
            [AllowHtml]
            public string OverrideAltAttribute { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Posts.Pictures.Fields.OverrideTitleAttribute")]
            [AllowHtml]
            public string OverrideTitleAttribute { get; set; }
        }

        public partial class PostCategoryModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Posts.Categories.Fields.Category")]
            public string Category { get; set; }

            public int PostId { get; set; }

            public int CategoryId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Posts.Categories.Fields.IsFeaturedPost")]
            public bool IsFeaturedPost { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Posts.Categories.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }
        #endregion
    }


    public partial class PostLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.ShortDescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.FullDescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Posts.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
   
}