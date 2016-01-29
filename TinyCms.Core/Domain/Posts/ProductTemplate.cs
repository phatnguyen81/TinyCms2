namespace TinyCms.Core.Domain.Posts
{
    /// <summary>
    ///     Represents a product template
    /// </summary>
    public class PostTemplate : BaseEntity
    {
        /// <summary>
        ///     Gets or sets the template name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the view path
        /// </summary>
        public string ViewPath { get; set; }

        /// <summary>
        ///     Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}