using System.Collections.Generic;
using TinyCms.Core.Domain.Posts;

namespace TinyCms.Services.Posts
{
    /// <summary>
    ///     Post tag service interface
    /// </summary>
    public interface IPostTagService
    {
        /// <summary>
        ///     Delete a post tag
        /// </summary>
        /// <param name="postTag">Post tag</param>
        void DeletePostTag(PostTag postTag);

        /// <summary>
        ///     Gets all post tags
        /// </summary>
        /// <returns>Post tags</returns>
        IList<PostTag> GetAllPostTags();

        /// <summary>
        ///     Gets post tag
        /// </summary>
        /// <param name="postTagId">Post tag identifier</param>
        /// <returns>Post tag</returns>
        PostTag GetPostTagById(int postTagId);

        /// <summary>
        ///     Gets post tag by name
        /// </summary>
        /// <param name="name">Post tag name</param>
        /// <returns>Post tag</returns>
        PostTag GetPostTagByName(string name);

        /// <summary>
        ///     Inserts a post tag
        /// </summary>
        /// <param name="postTag">Post tag</param>
        void InsertPostTag(PostTag postTag);

        /// <summary>
        ///     Updates the post tag
        /// </summary>
        /// <param name="postTag">Post tag</param>
        void UpdatePostTag(PostTag postTag);

        /// <summary>
        ///     Get number of posts
        /// </summary>
        /// <param name="postTagId">Post tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Number of posts</returns>
        int GetPostCount(int postTagId);
    }
}