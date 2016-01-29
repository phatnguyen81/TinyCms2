namespace TinyCms.Core.Domain.Posts
{
    /// <summary>
    ///     Represents a product category mapping
    /// </summary>
    public class PostCategory : BaseEntity
    {
        /// <summary>
        ///     Gets or sets the product identifier
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        ///     Gets or sets the category identifier
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the product is featured
        /// </summary>
        public bool IsFeaturedPost { get; set; }

        /// <summary>
        ///     Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        ///     Gets the category
        /// </summary>
        public virtual Category Category { get; set; }

        /// <summary>
        ///     Gets the product
        /// </summary>
        public virtual Post Post { get; set; }
    }
}