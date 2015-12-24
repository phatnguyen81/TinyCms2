using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TinyCms.Core.Caching;
using TinyCms.Core.Data;
using TinyCms.Core.Domain.Common;
using TinyCms.Core.Domain.Posts;
using TinyCms.Data;
using TinyCms.Services.Events;

namespace TinyCms.Services.Posts
{
    /// <summary>
    /// Post tag service
    /// </summary>
    public partial class PostTagService : IPostTagService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        private const string POSTTAG_COUNT_KEY = "Nop.posttag.count";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string POSTTAG_PATTERN_KEY = "Nop.posttag.";

        #endregion

        #region Fields

        private readonly IRepository<PostTag> _postTagRepository;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly CommonSettings _commonSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="postTagRepository">Post tag repository</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="eventPublisher">Event published</param>
        public PostTagService(IRepository<PostTag> postTagRepository,
            IDataProvider dataProvider, 
            IDbContext dbContext,
            CommonSettings commonSettings,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher)
        {
            this._postTagRepository = postTagRepository;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._commonSettings = commonSettings;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Nested classes

        private class PostTagWithCount
        {
            public int PostTagId { get; set; }
            public int PostCount { get; set; }
        }

        #endregion
        
        #region Utilities

        /// <summary>
        /// Get post count for each of existing post tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Dictionary of "post tag ID : post count"</returns>
        private Dictionary<int, int> GetPostCount()
        {
            string key = string.Format(POSTTAG_COUNT_KEY);
            return _cacheManager.Get(key, () =>
            {

                if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
                {
                    //stored procedures are enabled and supported by the database. 
                    //It's much faster than the LINQ implementation below 

                    #region Use stored procedure

                    //prepare parameters

                    //invoke stored procedure
                    var result = _dbContext.SqlQuery<PostTagWithCount>(
                        "Exec PostTagCountLoadAll");

                    var dictionary = new Dictionary<int, int>();
                    foreach (var item in result)
                        dictionary.Add(item.PostTagId, item.PostCount);
                    return dictionary;

                    #endregion
                }
                else
                {
                    //stored procedures aren't supported. Use LINQ
                    #region Search posts
                    var query = from pt in _postTagRepository.Table
                                select new
                                {
                                    Id = pt.Id,
                                    PostCount = pt.Posts
                                        //published and not deleted posts
                                        .Count(p => !p.Deleted && p.Published)
                                };

                    var dictionary = new Dictionary<int, int>();
                    foreach (var item in query)
                        dictionary.Add(item.Id, item.PostCount);
                    return dictionary;

                    #endregion

                }
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a post tag
        /// </summary>
        /// <param name="postTag">Post tag</param>
        public virtual void DeletePostTag(PostTag postTag)
        {
            if (postTag == null)
                throw new ArgumentNullException("postTag");

            _postTagRepository.Delete(postTag);

            //cache
            _cacheManager.RemoveByPattern(POSTTAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(postTag);
        }

        /// <summary>
        /// Gets all post tags
        /// </summary>
        /// <returns>Post tags</returns>
        public virtual IList<PostTag> GetAllPostTags()
        {
            var query = _postTagRepository.Table;
            var postTags = query.ToList();
            return postTags;
        }

        /// <summary>
        /// Gets post tag
        /// </summary>
        /// <param name="postTagId">Post tag identifier</param>
        /// <returns>Post tag</returns>
        public virtual PostTag GetPostTagById(int postTagId)
        {
            if (postTagId == 0)
                return null;

            return _postTagRepository.GetById(postTagId);
        }

        /// <summary>
        /// Gets post tag by name
        /// </summary>
        /// <param name="name">Post tag name</param>
        /// <returns>Post tag</returns>
        public virtual PostTag GetPostTagByName(string name)
        {
            var query = from pt in _postTagRepository.Table
                        where pt.Name == name
                        select pt;

            var postTag = query.FirstOrDefault();
            return postTag;
        }

        /// <summary>
        /// Inserts a post tag
        /// </summary>
        /// <param name="postTag">Post tag</param>
        public virtual void InsertPostTag(PostTag postTag)
        {
            if (postTag == null)
                throw new ArgumentNullException("postTag");

            _postTagRepository.Insert(postTag);

            //cache
            _cacheManager.RemoveByPattern(POSTTAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(postTag);
        }

        /// <summary>
        /// Updates the post tag
        /// </summary>
        /// <param name="postTag">Post tag</param>
        public virtual void UpdatePostTag(PostTag postTag)
        {
            if (postTag == null)
                throw new ArgumentNullException("postTag");

            _postTagRepository.Update(postTag);

            //cache
            _cacheManager.RemoveByPattern(POSTTAG_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(postTag);
        }

        /// <summary>
        /// Get number of posts
        /// </summary>
        /// <param name="postTagId">Post tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Number of posts</returns>
        public virtual int GetPostCount(int postTagId)
        {
            var dictionary = GetPostCount();
            if (dictionary.ContainsKey(postTagId))
                return dictionary[postTagId];
            
            return 0;
        }

        #endregion
    }
}
