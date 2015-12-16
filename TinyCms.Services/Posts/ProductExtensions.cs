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

  
    }
}
