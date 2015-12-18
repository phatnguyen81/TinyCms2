using System.Collections.Generic;
using TinyCms.Core.Domain.Posts;

namespace TinyCms.Services.Posts
{
    /// <summary>
    /// Post template interface
    /// </summary>
    public partial interface IPostTemplateService
    {
        /// <summary>
        /// Delete post template
        /// </summary>
        /// <param name="postTemplate">Post template</param>
        void DeletePostTemplate(PostTemplate postTemplate);

        /// <summary>
        /// Gets all post templates
        /// </summary>
        /// <returns>Post templates</returns>
        IList<PostTemplate> GetAllPostTemplates();

        /// <summary>
        /// Gets a post template
        /// </summary>
        /// <param name="postTemplateId">Post template identifier</param>
        /// <returns>Post template</returns>
        PostTemplate GetPostTemplateById(int postTemplateId);

        /// <summary>
        /// Inserts post template
        /// </summary>
        /// <param name="postTemplate">Post template</param>
        void InsertPostTemplate(PostTemplate postTemplate);

        /// <summary>
        /// Updates the post template
        /// </summary>
        /// <param name="postTemplate">Post template</param>
        void UpdatePostTemplate(PostTemplate postTemplate);
    }
}
