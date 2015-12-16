using System.Collections.Generic;
using TinyCms.Core.Domain.Localization;

namespace TinyCms.Core.Domain.Posts
{
    /// <summary>
    /// Represents a product tag
    /// </summary>
    public partial class PostTag : BaseEntity, ILocalizedEntity
    {
        private ICollection<Post> _products;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the products
        /// </summary>
        public virtual ICollection<Post> Products
        {
            get { return _products ?? (_products = new List<Post>()); }
            protected set { _products = value; }
        }
    }
}
