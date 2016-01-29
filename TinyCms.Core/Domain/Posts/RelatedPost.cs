namespace TinyCms.Core.Domain.Posts
{
    /// <summary>
    ///     Represents a related product
    /// </summary>
    public class RelatedPost : BaseEntity
    {
        /// <summary>
        ///     Gets or sets the first product identifier
        /// </summary>
        public int PostId1 { get; set; }

        /// <summary>
        ///     Gets or sets the second product identifier
        /// </summary>
        public int PostId2 { get; set; }

        /// <summary>
        ///     Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}