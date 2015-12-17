using System.Collections.Generic;
using TinyCms.Core.Domain.Localization;

namespace TinyCms.Core.Domain.Posts
{
    /// <summary>
    /// Represents a post tag
    /// </summary>
    public partial class PostTag : BaseEntity, ILocalizedEntity
    {
        private ICollection<Post> _posts;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the posts
        /// </summary>
        public virtual ICollection<Post> Posts
        {
            get { return _posts ?? (_posts = new List<Post>()); }
            protected set { _posts = value; }
        }
    }
}
