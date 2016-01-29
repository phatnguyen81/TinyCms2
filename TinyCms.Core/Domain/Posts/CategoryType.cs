namespace TinyCms.Core.Domain.Posts
{
    /// <summary>
    ///     Represents a category
    /// </summary>
    public class CategoryType : BaseEntity
    {
        public string SystemName { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}