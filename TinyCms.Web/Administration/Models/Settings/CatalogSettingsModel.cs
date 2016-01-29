using System.Collections.Generic;
using System.Web.Mvc;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Settings
{
    public class CatalogSettingsModel : BaseNopModel
    {
        public CatalogSettingsModel()
        {
            AvailableCategories = new List<SelectListItem>();
        }

        public List<SelectListItem> AvailableCategories { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowPostsFromSubcategories")]
        public bool ShowPostsFromSubcategories { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryPostNumber")]
        public bool ShowCategoryPostNumber { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryPostNumberIncludingSubcategories")]
        public bool ShowCategoryPostNumberIncludingSubcategories { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.CategoryBreadcrumbEnabled")]
        public bool CategoryBreadcrumbEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ComparePostsEnabled")]
        public bool ComparePostsEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowBestsellersOnHomepage")]
        public bool ShowBestsellersOnHomepage { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfBestsellersOnHomepage")]
        public int NumberOfBestsellersOnHomepage { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePostsPerPage")]
        public int SearchPagePostsPerPage { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageAllowCustomersToSelectPageSize")]
        public bool SearchPageAllowCustomersToSelectPageSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePageSizeOptions")]
        public string SearchPagePageSizeOptions { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PostSearchAutoCompleteEnabled")]
        public bool PostSearchAutoCompleteEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PostSearchAutoCompleteNumberOfPosts")]
        public int PostSearchAutoCompleteNumberOfPosts { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowPostImagesInSearchAutoComplete")]
        public bool ShowPostImagesInSearchAutoComplete { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PostSearchTermMinimumLength")]
        public int PostSearchTermMinimumLength { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfPostTags")]
        public int NumberOfPostTags { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreFeaturedPosts")]
        public bool IgnoreFeaturedPosts { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreAcl")]
        public bool IgnoreAcl { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreStoreLimitations")]
        public bool IgnoreStoreLimitations { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShareCornerCategoryId")]
        public int ShareCornerCategoryId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShareCornerPostNumber")]
        public int ShareCornerPostNumber { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PictureAndVideoCategoryId")]
        public int PictureAndVideoCategoryId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PictureAndVideoPostNumber")]
        public int PictureAndVideoPostNumber { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.TravelViaPictureCategoryId")]
        public int TravelViaPictureCategoryId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.TravelViaPicturePostNumber")]
        public int TravelViaPicturePostNumber { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DiscoveryCategoryId")]
        public int DiscoveryCategoryId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DiscoveryPostNumber")]
        public int DiscoveryPostNumber { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PromotionCategoryId")]
        public int PromotionCategoryId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PromotionPostNumber")]
        public int PromotionPostNumber { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SummerTourismId")]
        public int SummerTourismId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SummerTourismPostNumber")]
        public int SummerTourismPostNumber { get; set; }
    }
}