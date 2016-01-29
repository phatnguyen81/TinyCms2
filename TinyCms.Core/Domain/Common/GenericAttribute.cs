namespace TinyCms.Core.Domain.Common
{
    /// <summary>
    ///     Represents a generic attribute
    /// </summary>
    public class GenericAttribute : BaseEntity
    {
        /// <summary>
        ///     Gets or sets the entity identifier
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        ///     Gets or sets the key group
        /// </summary>
        public string KeyGroup { get; set; }

        /// <summary>
        ///     Gets or sets the key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or sets the value
        /// </summary>
        public string Value { get; set; }
    }
}