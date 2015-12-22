using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Data;
using TinyCms.Core.Domain.Catalog;
using TinyCms.Core.Domain.Common;
using TinyCms.Core.Domain.Localization;
using TinyCms.Core.Domain.Posts;
using TinyCms.Core.Domain.Security;
using TinyCms.Data;
using TinyCms.Services.Customers;
using TinyCms.Services.Events;
using TinyCms.Services.Localization;
using TinyCms.Services.Messages;
using TinyCms.Services.Security;

namespace TinyCms.Services.Posts
{
    /// <summary>
    /// Post service
    /// </summary>
    public partial class PostService : IPostService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : post ID
        /// </remarks>
        private const string PRODUCTS_BY_ID_KEY = "Nop.post.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTS_PATTERN_KEY = "Nop.post.";
        #endregion

        #region Fields

        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<PostPicture> _postPictureRepository;
        private readonly ILanguageService _languageService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CommonSettings _commonSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAclService _aclService;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<CategoryType> _categoryTypeRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="postRepository">Post repository</param>
        /// <param name="relatedPostRepository">Related post repository</param>
        /// <param name="crossSellPostRepository">Cross-sell post repository</param>
        /// <param name="tierPriceRepository">Tier price repository</param>
        /// <param name="localizedPropertyRepository">Localized property repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="postPictureRepository">Post picture repository</param>
        /// <param name="postSpecificationAttributeRepository">Post specification attribute repository</param>
        /// <param name="postReviewRepository">Post review repository</param>
        /// <param name="postWarehouseInventoryRepository">Post warehouse inventory repository</param>
        /// <param name="postAttributeService">Post attribute service</param>
        /// <param name="postAttributeParser">Post attribute parser service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="workContext">Work context</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        public PostService(ICacheManager cacheManager,
            IRepository<Post> postRepository,
            IRepository<PostPicture> postPictureRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<AclRecord> aclRepository,
            ILanguageService languageService,
            IWorkflowMessageService workflowMessageService,
            IDataProvider dataProvider, 
            IDbContext dbContext,
            IWorkContext workContext,
            LocalizationSettings localizationSettings, 
            CommonSettings commonSettings,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            IAclService aclService, IRepository<Category> categoryRepository, IRepository<CategoryType> categoryTypeRepository)
        {
            this._cacheManager = cacheManager;
            this._postRepository = postRepository;
            this._postPictureRepository = postPictureRepository;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._aclRepository = aclRepository;
            this._languageService = languageService;
            this._workflowMessageService = workflowMessageService;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._localizationSettings = localizationSettings;
            this._commonSettings = commonSettings;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
            this._aclService = aclService;
            _categoryRepository = categoryRepository;
            _categoryTypeRepository = categoryTypeRepository;
        }

        #endregion
        
        #region Methods

        #region Posts

        /// <summary>
        /// Delete a post
        /// </summary>
        /// <param name="post">Post</param>
        public virtual void DeletePost(Post post)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            post.Deleted = true;
            //delete post
            UpdatePost(post);
        }

        /// <summary>
        /// Gets all posts displayed on the home page
        /// </summary>
        /// <returns>Posts</returns>
        public virtual IList<Post> GetAllPostsDisplayedOnHomePage()
        {
            var query = from p in _postRepository.Table
                        orderby p.DisplayOrder, p.Name
                        where p.Published &&
                        !p.Deleted &&
                        p.ShowOnHomePage
                        select p;
            var posts = query.ToList();
            return posts;
        }
        
        /// <summary>
        /// Gets post
        /// </summary>
        /// <param name="postId">Post identifier</param>
        /// <returns>Post</returns>
        public virtual Post GetPostById(int postId)
        {
            if (postId == 0)
                return null;
            
            string key = string.Format(PRODUCTS_BY_ID_KEY, postId);
            return _cacheManager.Get(key, () => _postRepository.GetById(postId));
        }

        /// <summary>
        /// Get posts by identifiers
        /// </summary>
        /// <param name="postIds">Post identifiers</param>
        /// <returns>Posts</returns>
        public virtual IList<Post> GetPostsByIds(int[] postIds)
        {
            if (postIds == null || postIds.Length == 0)
                return new List<Post>();

            var query = from p in _postRepository.Table
                        where postIds.Contains(p.Id)
                        select p;
            var posts = query.ToList();
            //sort by passed identifiers
            var sortedPosts = new List<Post>();
            foreach (int id in postIds)
            {
                var post = posts.Find(x => x.Id == id);
                if (post != null)
                    sortedPosts.Add(post);
            }
            return sortedPosts;
        }

        /// <summary>
        /// Inserts a post
        /// </summary>
        /// <param name="post">Post</param>
        public virtual void InsertPost(Post post)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            //insert
            _postRepository.Insert(post);

            //clear cache
            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            
            //event notification
            _eventPublisher.EntityInserted(post);
        }

        /// <summary>
        /// Updates the post
        /// </summary>
        /// <param name="post">Post</param>
        public virtual void UpdatePost(Post post)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            //update
            _postRepository.Update(post);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(post);
        }

        /// <summary>
        /// Get (visible) post number in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>Post number</returns>
        public virtual int GetCategoryPostNumber(IList<int> categoryIds = null)
        {
            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            var query = _postRepository.Table;
            query = query.Where(p => !p.Deleted && p.Published);

            //category filtering
            if (categoryIds != null && categoryIds.Count > 0)
            {
                query = from p in query
                        from pc in p.PostCategories.Where(pc => categoryIds.Contains(pc.CategoryId))
                        select p;
            }

            if (!_catalogSettings.IgnoreAcl)
            {
                //Access control list. Allowed customer roles
                var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

                query = from p in query
                        join acl in _aclRepository.Table
                        on new { c1 = p.Id, c2 = "Post" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into p_acl
                        from acl in p_acl.DefaultIfEmpty()
                        where !p.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select p;
            }

            //only distinct posts
            var result = query.Select(p => p.Id).Distinct().Count();
            return result;
        }

     

        /// <summary>
        /// Search posts
        /// </summary>
        /// <param name="filterableSpecificationAttributeOptionIds">The specification attribute option identifiers applied to loaded posts (all pages)</param>
        /// <param name="loadFilterableSpecificationAttributeOptionIds">A value indicating whether we should load the specification attribute option identifiers applied to loaded posts (all pages)</param>
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
        public virtual IPagedList<Post> SearchPosts(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            bool? featuredPosts = null,
            int postTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchPostTags = false,
            int languageId = 0,
            PostSortingEnum orderBy = PostSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null)
        {

            //search by keyword
            bool searchLocalizedValue = false;
            if (languageId > 0)
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (categoryIds !=null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database. 
                //It's much faster than the LINQ implementation below 

                #region Use stored procedure
                
                //pass category identifiers as comma-delimited string
                string commaSeparatedCategoryIds = categoryIds == null ? "" : string.Join(",", categoryIds);


                //pass customer role identifiers as comma-delimited string
                string commaSeparatedAllowedCustomerRoleIds = string.Join(",", allowedCustomerRolesIds);


                
                //some databases don't support int.MaxValue
                if (pageSize == int.MaxValue)
                    pageSize = int.MaxValue - 1;
                
                //prepare parameters
                var pCategoryIds = _dataProvider.GetParameter();
                pCategoryIds.ParameterName = "CategoryIds";
                pCategoryIds.Value = commaSeparatedCategoryIds != null ? (object)commaSeparatedCategoryIds : DBNull.Value;
                pCategoryIds.DbType = DbType.String;
                
                var pPostTagId = _dataProvider.GetParameter();
                pPostTagId.ParameterName = "PostTagId";
                pPostTagId.Value = postTagId;
                pPostTagId.DbType = DbType.Int32;

                var pFeaturedPosts = _dataProvider.GetParameter();
                pFeaturedPosts.ParameterName = "FeaturedPosts";
                pFeaturedPosts.Value = featuredPosts.HasValue ? (object)featuredPosts.Value : DBNull.Value;
                pFeaturedPosts.DbType = DbType.Boolean;

                var pKeywords = _dataProvider.GetParameter();
                pKeywords.ParameterName = "Keywords";
                pKeywords.Value = keywords != null ? (object)keywords : DBNull.Value;
                pKeywords.DbType = DbType.String;

                var pSearchDescriptions = _dataProvider.GetParameter();
                pSearchDescriptions.ParameterName = "SearchDescriptions";
                pSearchDescriptions.Value = searchDescriptions;
                pSearchDescriptions.DbType = DbType.Boolean;

                var pSearchPostTags = _dataProvider.GetParameter();
                pSearchPostTags.ParameterName = "SearchPostTags";
                pSearchPostTags.Value = searchPostTags;
                pSearchPostTags.DbType = DbType.Boolean;

                var pUseFullTextSearch = _dataProvider.GetParameter();
                pUseFullTextSearch.ParameterName = "UseFullTextSearch";
                pUseFullTextSearch.Value = _commonSettings.UseFullTextSearch;
                pUseFullTextSearch.DbType = DbType.Boolean;

                var pFullTextMode = _dataProvider.GetParameter();
                pFullTextMode.ParameterName = "FullTextMode";
                pFullTextMode.Value = (int)_commonSettings.FullTextMode;
                pFullTextMode.DbType = DbType.Int32;

                var pLanguageId = _dataProvider.GetParameter();
                pLanguageId.ParameterName = "LanguageId";
                pLanguageId.Value = searchLocalizedValue ? languageId : 0;
                pLanguageId.DbType = DbType.Int32;

                var pOrderBy = _dataProvider.GetParameter();
                pOrderBy.ParameterName = "OrderBy";
                pOrderBy.Value = (int)orderBy;
                pOrderBy.DbType = DbType.Int32;

                var pAllowedCustomerRoleIds = _dataProvider.GetParameter();
                pAllowedCustomerRoleIds.ParameterName = "AllowedCustomerRoleIds";
                pAllowedCustomerRoleIds.Value = commaSeparatedAllowedCustomerRoleIds;
                pAllowedCustomerRoleIds.DbType = DbType.String;

                var pPageIndex = _dataProvider.GetParameter();
                pPageIndex.ParameterName = "PageIndex";
                pPageIndex.Value = pageIndex;
                pPageIndex.DbType = DbType.Int32;

                var pPageSize = _dataProvider.GetParameter();
                pPageSize.ParameterName = "PageSize";
                pPageSize.Value = pageSize;
                pPageSize.DbType = DbType.Int32;

                var pShowHidden = _dataProvider.GetParameter();
                pShowHidden.ParameterName = "ShowHidden";
                pShowHidden.Value = showHidden;
                pShowHidden.DbType = DbType.Boolean;

                var pOverridePublished = _dataProvider.GetParameter();
                pOverridePublished.ParameterName = "OverridePublished";
                pOverridePublished.Value = overridePublished != null ? (object)overridePublished.Value : DBNull.Value;
                pOverridePublished.DbType = DbType.Boolean;

                var pFilterableSpecificationAttributeOptionIds = _dataProvider.GetParameter();
                pFilterableSpecificationAttributeOptionIds.ParameterName = "FilterableSpecificationAttributeOptionIds";
                pFilterableSpecificationAttributeOptionIds.Direction = ParameterDirection.Output;
                pFilterableSpecificationAttributeOptionIds.Size = int.MaxValue - 1;
                pFilterableSpecificationAttributeOptionIds.DbType = DbType.String;

                var pTotalRecords = _dataProvider.GetParameter();
                pTotalRecords.ParameterName = "TotalRecords";
                pTotalRecords.Direction = ParameterDirection.Output;
                pTotalRecords.DbType = DbType.Int32;

                //invoke stored procedure
                var posts = _dbContext.ExecuteStoredProcedureList<Post>(
                    "PostLoadAllPaged",
                    pCategoryIds,
                    pPostTagId,
                    pFeaturedPosts,
                    pKeywords,
                    pSearchDescriptions,
                    pSearchPostTags,
                    pUseFullTextSearch,
                    pFullTextMode,
                    pLanguageId,
                    pOrderBy,
                    pAllowedCustomerRoleIds,
                    pPageIndex,
                    pPageSize,
                    pShowHidden,
                    pOverridePublished,
                    pTotalRecords);
                //get filterable specification attribute option identifier
                //return posts
                int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
                return new PagedList<Post>(posts, pageIndex, pageSize, totalRecords);

                #endregion
            }
            else
            {
                //stored procedures aren't supported. Use LINQ

                #region Search posts

                //posts
                var query = _postRepository.Table;
                query = query.Where(p => !p.Deleted);
                if (!overridePublished.HasValue)
                {
                    //process according to "showHidden"
                    if (!showHidden)
                    {
                        query = query.Where(p => p.Published);
                    }
                }
                else if (overridePublished.Value)
                {
                    //published only
                    query = query.Where(p => p.Published);
                }
                else if (!overridePublished.Value)
                {
                    //unpublished only
                    query = query.Where(p => !p.Published);
                }


          
                //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
                //That's why we pass the date value
                var nowUtc = DateTime.UtcNow;
       
            
                //searching by keyword
                if (!String.IsNullOrWhiteSpace(keywords))
                {
                    query = from p in query
                            join lp in _localizedPropertyRepository.Table on p.Id equals lp.EntityId into p_lp
                            from lp in p_lp.DefaultIfEmpty()
                            from pt in p.PostTags.DefaultIfEmpty()
                            where (p.Name.Contains(keywords)) ||
                                  (searchDescriptions && p.ShortDescription.Contains(keywords)) ||
                                  (searchDescriptions && p.FullDescription.Contains(keywords)) ||
                                  (searchPostTags && pt.Name.Contains(keywords)) ||
                                  //localized values
                                  (searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Post" && lp.LocaleKey == "Name" && lp.LocaleValue.Contains(keywords)) ||
                                  (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Post" && lp.LocaleKey == "ShortDescription" && lp.LocaleValue.Contains(keywords)) ||
                                  (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Post" && lp.LocaleKey == "FullDescription" && lp.LocaleValue.Contains(keywords))
                            select p;
                }

                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    query = from p in query
                            join acl in _aclRepository.Table
                            on new { c1 = p.Id, c2 = "Post" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into p_acl
                            from acl in p_acl.DefaultIfEmpty()
                            where !p.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select p;
                }

             
                //category filtering
                if (categoryIds != null && categoryIds.Count > 0)
                {
                    query = from p in query
                            from pc in p.PostCategories.Where(pc => categoryIds.Contains(pc.CategoryId))
                            where (!featuredPosts.HasValue || featuredPosts.Value == pc.IsFeaturedPost)
                            select p;
                }

         
                //related posts filtering
                //if (relatedToPostId > 0)
                //{
                //    query = from p in query
                //            join rp in _relatedPostRepository.Table on p.Id equals rp.PostId2
                //            where (relatedToPostId == rp.PostId1)
                //            select p;
                //}

                //tag filtering
                if (postTagId > 0)
                {
                    query = from p in query
                            from pt in p.PostTags.Where(pt => pt.Id == postTagId)
                            select p;
                }

                //only distinct posts (group by ID)
                //if we use standard Distinct() method, then all fields will be compared (low performance)
                //it'll not work in SQL Server Compact when searching posts by a keyword)
                query = from p in query
                        group p by p.Id
                        into pGroup
                        orderby pGroup.Key
                        select pGroup.FirstOrDefault();

                //sort posts
                if (orderBy == PostSortingEnum.Position && categoryIds != null && categoryIds.Count > 0)
                {
                    //category position
                    var firstCategoryId = categoryIds[0];
                    query = query.OrderBy(p => p.PostCategories.FirstOrDefault(pc => pc.CategoryId == firstCategoryId).DisplayOrder);
                }
 
                else if (orderBy == PostSortingEnum.Position)
                {
                    //otherwise sort by name
                    query = query.OrderBy(p => p.Name);
                }
                else if (orderBy == PostSortingEnum.NameAsc)
                {
                    //Name: A to Z
                    query = query.OrderBy(p => p.Name);
                }
                else if (orderBy == PostSortingEnum.NameDesc)
                {
                    //Name: Z to A
                    query = query.OrderByDescending(p => p.Name);
                }
                else if (orderBy == PostSortingEnum.CreatedOn)
                {
                    //creation date
                    query = query.OrderByDescending(p => p.CreatedOnUtc);
                }
                else
                {
                    //actually this code is not reachable
                    query = query.OrderBy(p => p.Name);
                }

                var posts = new PagedList<Post>(query, pageIndex, pageSize);

            

                //return posts
                return posts;

                #endregion
            }
        }

        /// <summary>
        /// Update post review totals
        /// </summary>
        /// <param name="post">Post</param>
        public virtual void UpdatePostReviewTotals(Post post)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            //int approvedRatingSum = 0;
            //int notApprovedRatingSum = 0; 
            //int approvedTotalReviews = 0;
            //int notApprovedTotalReviews = 0;
            //var reviews = post.PostReviews;
            //foreach (var pr in reviews)
            //{
            //    if (pr.IsApproved)
            //    {
            //        approvedRatingSum += pr.Rating;
            //        approvedTotalReviews ++;
            //    }
            //    else
            //    {
            //        notApprovedRatingSum += pr.Rating;
            //        notApprovedTotalReviews++;
            //    }
            //}

            //post.ApprovedRatingSum = approvedRatingSum;
            //post.NotApprovedRatingSum = notApprovedRatingSum;
            //post.ApprovedTotalReviews = approvedTotalReviews;
            //post.NotApprovedTotalReviews = notApprovedTotalReviews;
            UpdatePost(post);
        }

       
        #endregion

        #region Post pictures

        /// <summary>
        /// Deletes a post picture
        /// </summary>
        /// <param name="postPicture">Post picture</param>
        public virtual void DeletePostPicture(PostPicture postPicture)
        {
            if (postPicture == null)
                throw new ArgumentNullException("postPicture");

            _postPictureRepository.Delete(postPicture);

            //event notification
            _eventPublisher.EntityDeleted(postPicture);
        }

        /// <summary>
        /// Gets a post pictures by post identifier
        /// </summary>
        /// <param name="postId">The post identifier</param>
        /// <returns>Post pictures</returns>
        public virtual IList<PostPicture> GetPostPicturesByPostId(int postId)
        {
            var query = from pp in _postPictureRepository.Table
                        where pp.PostId == postId
                        orderby pp.DisplayOrder
                        select pp;
            var postPictures = query.ToList();
            return postPictures;
        }

        /// <summary>
        /// Gets a post picture
        /// </summary>
        /// <param name="postPictureId">Post picture identifier</param>
        /// <returns>Post picture</returns>
        public virtual PostPicture GetPostPictureById(int postPictureId)
        {
            if (postPictureId == 0)
                return null;

            return _postPictureRepository.GetById(postPictureId);
        }

        /// <summary>
        /// Inserts a post picture
        /// </summary>
        /// <param name="postPicture">Post picture</param>
        public virtual void InsertPostPicture(PostPicture postPicture)
        {
            if (postPicture == null)
                throw new ArgumentNullException("postPicture");

            _postPictureRepository.Insert(postPicture);

            //event notification
            _eventPublisher.EntityInserted(postPicture);
        }

        /// <summary>
        /// Updates a post picture
        /// </summary>
        /// <param name="postPicture">Post picture</param>
        public virtual void UpdatePostPicture(PostPicture postPicture)
        {
            if (postPicture == null)
                throw new ArgumentNullException("postPicture");

            _postPictureRepository.Update(postPicture);

            //event notification
            _eventPublisher.EntityUpdated(postPicture);
        }

        #endregion


        #endregion
    }
}
