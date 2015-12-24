using System.Web.Mvc;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Settings
{
    public partial class CatalogSettingsModel : BaseNopModel
    {
        
        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowViewUnpublishedPostPage")]
        public bool AllowViewUnpublishedPostPage { get; set; }
        public bool AllowViewUnpublishedPostPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayDiscontinuedMessageForUnpublishedPosts")]
        public bool DisplayDiscontinuedMessageForUnpublishedPosts { get; set; }
        public bool DisplayDiscontinuedMessageForUnpublishedPosts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowPostSku")]
        public bool ShowPostSku { get; set; }
        public bool ShowPostSku_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowManufacturerPartNumber")]
        public bool ShowManufacturerPartNumber { get; set; }
        public bool ShowManufacturerPartNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowGtin")]
        public bool ShowGtin { get; set; }
        public bool ShowGtin_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowFreeShippingNotification")]
        public bool ShowFreeShippingNotification { get; set; }
        public bool ShowFreeShippingNotification_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowPostSorting")]
        public bool AllowPostSorting { get; set; }
        public bool AllowPostSorting_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowPostViewModeChanging")]
        public bool AllowPostViewModeChanging { get; set; }
        public bool AllowPostViewModeChanging_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowPostsFromSubcategories")]
        public bool ShowPostsFromSubcategories { get; set; }
        public bool ShowPostsFromSubcategories_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryPostNumber")]
        public bool ShowCategoryPostNumber { get; set; }
        public bool ShowCategoryPostNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryPostNumberIncludingSubcategories")]
        public bool ShowCategoryPostNumberIncludingSubcategories { get; set; }
        public bool ShowCategoryPostNumberIncludingSubcategories_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.CategoryBreadcrumbEnabled")]
        public bool CategoryBreadcrumbEnabled { get; set; }
        public bool CategoryBreadcrumbEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowShareButton")]
        public bool ShowShareButton { get; set; }
        public bool ShowShareButton_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PageShareCode")]
        [AllowHtml]
        public string PageShareCode { get; set; }
        public bool PageShareCode_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PostReviewsMustBeApproved")]
        public bool PostReviewsMustBeApproved { get; set; }
        public bool PostReviewsMustBeApproved_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToReviewPost")]
        public bool AllowAnonymousUsersToReviewPost { get; set; }
        public bool AllowAnonymousUsersToReviewPost_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NotifyStoreOwnerAboutNewPostReviews")]
        public bool NotifyStoreOwnerAboutNewPostReviews { get; set; }
        public bool NotifyStoreOwnerAboutNewPostReviews_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.EmailAFriendEnabled")]
        public bool EmailAFriendEnabled { get; set; }
        public bool EmailAFriendEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToEmailAFriend")]
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }
        public bool AllowAnonymousUsersToEmailAFriend_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyViewedPostsNumber")]
        public int RecentlyViewedPostsNumber { get; set; }
        public bool RecentlyViewedPostsNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyViewedPostsEnabled")]
        public bool RecentlyViewedPostsEnabled { get; set; }
        public bool RecentlyViewedPostsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NewPostsNumber")]
        public int NewPostsNumber { get; set; }
        public bool NewPostsNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NewPostsEnabled")]
        public bool NewPostsEnabled { get; set; }
        public bool NewPostsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ComparePostsEnabled")]
        public bool ComparePostsEnabled { get; set; }
        public bool ComparePostsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowBestsellersOnHomepage")]
        public bool ShowBestsellersOnHomepage { get; set; }
        public bool ShowBestsellersOnHomepage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfBestsellersOnHomepage")]
        public int NumberOfBestsellersOnHomepage { get; set; }
        public bool NumberOfBestsellersOnHomepage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePostsPerPage")]
        public int SearchPagePostsPerPage { get; set; }
        public bool SearchPagePostsPerPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageAllowCustomersToSelectPageSize")]
        public bool SearchPageAllowCustomersToSelectPageSize { get; set; }
        public bool SearchPageAllowCustomersToSelectPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPagePageSizeOptions")]
        public string SearchPagePageSizeOptions { get; set; }
        public bool SearchPagePageSizeOptions_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PostSearchAutoCompleteEnabled")]
        public bool PostSearchAutoCompleteEnabled { get; set; }
        public bool PostSearchAutoCompleteEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PostSearchAutoCompleteNumberOfPosts")]
        public int PostSearchAutoCompleteNumberOfPosts { get; set; }
        public bool PostSearchAutoCompleteNumberOfPosts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowPostImagesInSearchAutoComplete")]
        public bool ShowPostImagesInSearchAutoComplete { get; set; }
        public bool ShowPostImagesInSearchAutoComplete_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PostSearchTermMinimumLength")]
        public int PostSearchTermMinimumLength { get; set; }
        public bool PostSearchTermMinimumLength_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PostsAlsoPurchasedEnabled")]
        public bool PostsAlsoPurchasedEnabled { get; set; }
        public bool PostsAlsoPurchasedEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PostsAlsoPurchasedNumber")]
        public int PostsAlsoPurchasedNumber { get; set; }
        public bool PostsAlsoPurchasedNumber_OverrideForStore { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfPostTags")]
        public int NumberOfPostTags { get; set; }
        public bool NumberOfPostTags_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PostsByTagPageSize")]
        public int PostsByTagPageSize { get; set; }
        public bool PostsByTagPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PostsByTagAllowCustomersToSelectPageSize")]
        public bool PostsByTagAllowCustomersToSelectPageSize { get; set; }
        public bool PostsByTagAllowCustomersToSelectPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.PostsByTagPageSizeOptions")]
        public string PostsByTagPageSizeOptions { get; set; }
        public bool PostsByTagPageSizeOptions_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IncludeShortDescriptionInComparePosts")]
        public bool IncludeShortDescriptionInComparePosts { get; set; }
        public bool IncludeShortDescriptionInComparePosts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IncludeFullDescriptionInComparePosts")]
        public bool IncludeFullDescriptionInComparePosts { get; set; }
        public bool IncludeFullDescriptionInComparePosts_OverrideForStore { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreDiscounts")]
        public bool IgnoreDiscounts { get; set; }
        public bool IgnoreDiscounts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreFeaturedPosts")]
        public bool IgnoreFeaturedPosts { get; set; }
        public bool IgnoreFeaturedPosts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreAcl")]
        public bool IgnoreAcl { get; set; }
        public bool IgnoreAcl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreStoreLimitations")]
        public bool IgnoreStoreLimitations { get; set; }
        public bool IgnoreStoreLimitations_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.CachePostPrices")]
        public bool CachePostPrices { get; set; }
        public bool CachePostPrices_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ManufacturersBlockItemsToDisplay")]
        public int ManufacturersBlockItemsToDisplay { get; set; }
        public bool ManufacturersBlockItemsToDisplay_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoFooter")]
        public bool DisplayTaxShippingInfoFooter { get; set; }
        public bool DisplayTaxShippingInfoFooter_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoPostDetailsPage")]
        public bool DisplayTaxShippingInfoPostDetailsPage { get; set; }
        public bool DisplayTaxShippingInfoPostDetailsPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoPostBoxes")]
        public bool DisplayTaxShippingInfoPostBoxes { get; set; }
        public bool DisplayTaxShippingInfoPostBoxes_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoShoppingCart")]
        public bool DisplayTaxShippingInfoShoppingCart { get; set; }
        public bool DisplayTaxShippingInfoShoppingCart_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoWishlist")]
        public bool DisplayTaxShippingInfoWishlist { get; set; }
        public bool DisplayTaxShippingInfoWishlist_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.DisplayTaxShippingInfoOrderDetailsPage")]
        public bool DisplayTaxShippingInfoOrderDetailsPage { get; set; }
        public bool DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore { get; set; }
    }
}