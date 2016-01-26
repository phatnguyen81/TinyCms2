using TinyCms.Core.Configuration;

namespace TinyCms.Core.Domain.Posts
{
    /// <summary>
    /// Catalog settings
    /// </summary>
    public class CatalogSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating details pages of unpublished product details pages could be open (for SEO optimization)
        /// </summary>
        public bool AllowViewUnpublishedPostPage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating customers should see "discontinued" message when visibting details pages of unpublished products (if "AllowViewUnpublishedPostPage" is "true)
        /// </summary>
        public bool DisplayDiscontinuedMessageForUnpublishedPosts { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether "Published" or "Disable buy/wishlist buttons" flags should be updated after order cancellation (deletion).
        /// Of course, when qty > configured minimum stock level
        /// </summary>
        public bool PublishBackPostWhenCancellingOrders { get; set; }

        
        /// <summary>
        /// Gets or sets a value indicating whether product sorting is enabled
        /// </summary>
        public bool AllowPostSorting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to change product view mode
        /// </summary>
        public bool AllowPostViewModeChanging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to change product view mode
        /// </summary>
        public string DefaultViewMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a category details page should include products from subcategories
        /// </summary>
        public bool ShowPostsFromSubcategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether number of products should be displayed beside each category
        /// </summary>
        public bool ShowCategoryPostNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we include subcategories (when 'ShowCategoryPostNumber' is 'true')
        /// </summary>
        public bool ShowCategoryPostNumberIncludingSubcategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether category breadcrumb is enabled
        /// </summary>
        public bool CategoryBreadcrumbEnabled { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether a 'Share button' is enabled
        /// </summary>
        public bool ShowShareButton { get; set; }

        /// <summary>
        /// Gets or sets a share code (e.g. AddThis button code)
        /// </summary>
        public string PageShareCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating product reviews must be approved
        /// </summary>
        public bool PostReviewsMustBeApproved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default rating value of the product reviews
        /// </summary>
        public int DefaultPostRatingValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users write product reviews.
        /// </summary>
        public bool AllowAnonymousUsersToReviewPost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether notification of a store owner about new product reviews is enabled
        /// </summary>
        public bool NotifyStoreOwnerAboutNewPostReviews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product 'Email a friend' feature is enabled
        /// </summary>
        public bool EmailAFriendEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous users to email a friend.
        /// </summary>
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }

        /// <summary>
        /// Gets or sets a number of "Recently viewed products"
        /// </summary>
        public int RecentlyViewedPostsNumber { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether "Recently viewed products" feature is enabled
        /// </summary>
        public bool RecentlyViewedPostsEnabled { get; set; }
        /// <summary>
        /// Gets or sets a number of products on the "New products" page
        /// </summary>
        public int NewPostsNumber { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether "New products" page is enabled
        /// </summary>
        public bool NewPostsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Compare products" feature is enabled
        /// </summary>
        public bool ComparePostsEnabled { get; set; }
        /// <summary>
        /// Gets or sets an allowed number of products to be compared
        /// </summary>
        public int ComparePostsNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether autocomplete is enabled
        /// </summary>
        public bool PostSearchAutoCompleteEnabled { get; set; }
        /// <summary>
        /// Gets or sets a number of products to return when using "autocomplete" feature
        /// </summary>
        public int PostSearchAutoCompleteNumberOfPosts { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to show product images in the auto complete search
        /// </summary>
        public bool ShowPostImagesInSearchAutoComplete { get; set; }
        /// <summary>
        /// Gets or sets a minimum search term length
        /// </summary>
        public int PostSearchTermMinimumLength { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to show bestsellers on home page
        /// </summary>
        public bool ShowBestsellersOnHomepage { get; set; }
        /// <summary>
        /// Gets or sets a number of bestsellers on home page
        /// </summary>
        public int NumberOfBestsellersOnHomepage { get; set; }

        /// <summary>
        /// Gets or sets a number of products per page on the search products page
        /// </summary>
        public int SearchPagePostsPerPage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select page size on the search products page
        /// </summary>
        public bool SearchPageAllowCustomersToSelectPageSize { get; set; }
        /// <summary>
        /// Gets or sets the available customer selectable page size options on the search products page
        /// </summary>
        public string SearchPagePageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets "List of products purchased by other customers who purchased the above" option is enable
        /// </summary>
        public bool PostsAlsoPurchasedEnabled { get; set; }

        /// <summary>
        /// Gets or sets a number of products also purchased by other customers to display
        /// </summary>
        public int PostsAlsoPurchasedNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should process attribute change using AJAX. It's used for dynamical attribute change, SKU/GTIN update of combinations, conditional attributes
        /// </summary>
        public bool AjaxProcessAttributeChange { get; set; }
        
        /// <summary>
        /// Gets or sets a number of product tags that appear in the tag cloud
        /// </summary>
        public int NumberOfPostTags { get; set; }

        /// <summary>
        /// Gets or sets a number of products per page on 'products by tag' page
        /// </summary>
        public int PostsByTagPageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size for 'products by tag'
        /// </summary>
        public bool PostsByTagAllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options for 'products by tag'
        /// </summary>
        public string PostsByTagPageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include "Short description" in compare products
        /// </summary>
        public bool IncludeShortDescriptionInComparePosts { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to include "Full description" in compare products
        /// </summary>
        public bool IncludeFullDescriptionInComparePosts { get; set; }
        /// <summary>
        /// An option indicating whether products on category and manufacturer pages should include featured products as well
        /// </summary>
        public bool IncludeFeaturedPostsInNormalLists { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether tier prices should be displayed with applied discounts (if available)
        /// </summary>
        public bool DisplayTierPricesWithDiscounts { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to ignore discounts (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreDiscounts { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to ignore featured products (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreFeaturedPosts { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to ignore ACL rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreAcl { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to ignore "limit per store" rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreStoreLimitations { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to cache product prices. It can significantly improve performance when enabled.
        /// </summary>
        public bool CachePostPrices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating maximum number of 'back in stock' subscription
        /// </summary>
        public int MaximumBackInStockSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the value indicating how many manufacturers to display in manufacturers block
        /// </summary>
        public int ManufacturersBlockItemsToDisplay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax in the footer (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoFooter { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax on product details pages (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoPostDetailsPage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax in product boxes (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoPostBoxes { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax on shopping cart page (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoShoppingCart { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax on wishlist page (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoWishlist { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to display information about shipping and tax on order details page (used in Germany)
        /// </summary>
        public bool DisplayTaxShippingInfoOrderDetailsPage { get; set; }


        /// <summary>
        /// Gets or sets the default value to use for Category page size options (for new categories)
        /// </summary>
        public string DefaultCategoryPageSizeOptions { get; set; }
        /// <summary>
        /// Gets or sets the default value to use for Category page size (for new categories)
        /// </summary>
        public int DefaultCategoryPageSize { get; set; }
        /// <summary>
        /// Gets or sets the default value to use for Manufacturer page size options (for new manufacturers)
        /// </summary>
        public string DefaultManufacturerPageSizeOptions { get; set; }
        /// <summary>
        /// Gets or sets the default value to use for Manufacturer page size (for new manufacturers)
        /// </summary>
        public int DefaultManufacturerPageSize { get; set; }

        public int ShareCornerCategoryId { get; set; }

        public int ShareCornerPostNumber { get; set; }

        public int PictureAndVideoCategoryId { get; set; }

        public int PictureAndVideoPostNumber { get; set; }

        public int TravelViaPictureCategoryId { get; set; }

        public int TravelViaPicturePostNumber { get; set; }

        public int DiscoveryCategoryId { get; set; }

        public int DiscoveryPostNumber { get; set; }

        public int PromotionCategoryId { get; set; }

        public int PromotionPostNumber { get; set; }

        public int SummerTourismId { get; set; }

        public int SummerTourismPostNumber { get; set; }

        public int VideoPostTemplateId { get; set; }
    }
}