using System;
using System.Collections.Generic;
using TinyCms.Core;
using TinyCms.Core.Domain.Catalog;
using TinyCms.Core.Domain.Posts;

namespace TinyCms.Services.Posts
{
    /// <summary>
    /// Post service
    /// </summary>
    public partial interface IPostService
    {
        #region Posts

        /// <summary>
        /// Delete a post
        /// </summary>
        /// <param name="post">Post</param>
        void DeletePost(Post post);

        /// <summary>
        /// Gets all posts displayed on the home page
        /// </summary>
        /// <returns>Posts</returns>
        IList<Post> GetAllPostsDisplayedOnHomePage();
        
        /// <summary>
        /// Gets post
        /// </summary>
        /// <param name="postId">Post identifier</param>
        /// <returns>Post</returns>
        Post GetPostById(int postId);
        
        /// <summary>
        /// Gets posts by identifier
        /// </summary>
        /// <param name="postIds">Post identifiers</param>
        /// <returns>Posts</returns>
        IList<Post> GetPostsByIds(int[] postIds);

        /// <summary>
        /// Inserts a post
        /// </summary>
        /// <param name="post">Post</param>
        void InsertPost(Post post);

        /// <summary>
        /// Updates the post
        /// </summary>
        /// <param name="post">Post</param>
        void UpdatePost(Post post);

        /// <summary>
        /// Get (visible) post number in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>Post number</returns>
        int GetCategoryPostNumber(IList<int> categoryIds = null);

        /// <summary>
        /// Search posts
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="postType">Post type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only posts marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="markedAsNewOnly">A values indicating whether to load only posts marked as "new"; "false" to load all records; "true" to load "marked as new" only</param>
        /// <param name="featuredPosts">A value indicating whether loaded posts are marked as featured (relates only to categories and manufacturers). 0 to load featured posts only, 1 to load not featured posts only, null to load all posts</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="postTagId">Post tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in post descriptions</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in post SKU</param>
        /// <param name="searchPostTags">A value indicating whether to search by a specified "keyword" in post tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="filteredSpecs">Filtered post specification identifiers</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" posts
        /// false - load only "Unpublished" posts
        /// </param>
        /// <returns>Posts</returns>
        IPagedList<Post> SearchPosts(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredPosts = null,
            int postTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchPostTags = false,
            int languageId = 0,
            PostSortingEnum orderBy = PostSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null);

       

      /// <summary>
        /// Update post review totals
        /// </summary>
        /// <param name="post">Post</param>
        void UpdatePostReviewTotals(Post post);

       
      
        #endregion


        #region Post pictures

        /// <summary>
        /// Deletes a post picture
        /// </summary>
        /// <param name="postPicture">Post picture</param>
        void DeletePostPicture(PostPicture postPicture);

        /// <summary>
        /// Gets a post pictures by post identifier
        /// </summary>
        /// <param name="postId">The post identifier</param>
        /// <returns>Post pictures</returns>
        IList<PostPicture> GetPostPicturesByPostId(int postId);

        /// <summary>
        /// Gets a post picture
        /// </summary>
        /// <param name="postPictureId">Post picture identifier</param>
        /// <returns>Post picture</returns>
        PostPicture GetPostPictureById(int postPictureId);

        /// <summary>
        /// Inserts a post picture
        /// </summary>
        /// <param name="postPicture">Post picture</param>
        void InsertPostPicture(PostPicture postPicture);

        /// <summary>
        /// Updates a post picture
        /// </summary>
        /// <param name="postPicture">Post picture</param>
        void UpdatePostPicture(PostPicture postPicture);

        #endregion


    }
}
