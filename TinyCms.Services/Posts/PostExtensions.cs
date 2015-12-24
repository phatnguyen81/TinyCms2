using System;
using System.Collections.Generic;
using System.Linq;
using TinyCms.Core.Domain.Posts;

namespace TinyCms.Services.Posts
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class PostExtensions
    {
     
        
        /// <summary>
        /// Indicates whether a post tag exists
        /// </summary>
        /// <param name="post">Post</param>
        /// <param name="postTagId">Post tag identifier</param>
        /// <returns>Result</returns>
        public static bool PostTagExists(this Post post,
            int postTagId)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            bool result = post.PostTags.ToList().Find(pt => pt.Id == postTagId) != null;
            return result;
        }

        /// <summary>
        /// Finds a related post item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="postId1">The first post identifier</param>
        /// <param name="postId2">The second post identifier</param>
        /// <returns>Related post</returns>
        public static RelatedPost FindRelatedPost(this IList<RelatedPost> source,
            int postId1, int postId2)
        {
            foreach (RelatedPost relatedPost in source)
                if (relatedPost.PostId1 == postId1 && relatedPost.PostId2 == postId2)
                    return relatedPost;
            return null;
        }

  
    }
}
