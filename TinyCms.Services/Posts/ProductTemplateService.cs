using System;
using System.Collections.Generic;
using System.Linq;
using TinyCms.Core.Data;
using TinyCms.Core.Domain.Posts;
using TinyCms.Services.Events;

namespace TinyCms.Services.Posts
{
    /// <summary>
    /// Post template service
    /// </summary>
    public partial class PostTemplateService : IPostTemplateService
    {
        #region Fields

        private readonly IRepository<PostTemplate> _postTemplateRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="postTemplateRepository">Post template repository</param>
        /// <param name="eventPublisher">Event published</param>
        public PostTemplateService(IRepository<PostTemplate> postTemplateRepository,
            IEventPublisher eventPublisher)
        {
            this._postTemplateRepository = postTemplateRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete post template
        /// </summary>
        /// <param name="postTemplate">Post template</param>
        public virtual void DeletePostTemplate(PostTemplate postTemplate)
        {
            if (postTemplate == null)
                throw new ArgumentNullException("postTemplate");

            _postTemplateRepository.Delete(postTemplate);

            //event notification
            _eventPublisher.EntityDeleted(postTemplate);
        }

        /// <summary>
        /// Gets all post templates
        /// </summary>
        /// <returns>Post templates</returns>
        public virtual IList<PostTemplate> GetAllPostTemplates()
        {
            var query = from pt in _postTemplateRepository.Table
                        orderby pt.DisplayOrder
                        select pt;

            var templates = query.ToList();
            return templates;
        }

        /// <summary>
        /// Gets a post template
        /// </summary>
        /// <param name="postTemplateId">Post template identifier</param>
        /// <returns>Post template</returns>
        public virtual PostTemplate GetPostTemplateById(int postTemplateId)
        {
            if (postTemplateId == 0)
                return null;

            return _postTemplateRepository.GetById(postTemplateId);
        }

        /// <summary>
        /// Inserts post template
        /// </summary>
        /// <param name="postTemplate">Post template</param>
        public virtual void InsertPostTemplate(PostTemplate postTemplate)
        {
            if (postTemplate == null)
                throw new ArgumentNullException("postTemplate");

            _postTemplateRepository.Insert(postTemplate);

            //event notification
            _eventPublisher.EntityInserted(postTemplate);
        }

        /// <summary>
        /// Updates the post template
        /// </summary>
        /// <param name="postTemplate">Post template</param>
        public virtual void UpdatePostTemplate(PostTemplate postTemplate)
        {
            if (postTemplate == null)
                throw new ArgumentNullException("postTemplate");

            _postTemplateRepository.Update(postTemplate);

            //event notification
            _eventPublisher.EntityUpdated(postTemplate);
        }
        
        #endregion
    }
}
