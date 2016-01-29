namespace TinyCms.Web.Framework.Events
{
    /// <summary>
    ///     Product search event
    /// </summary>
    public class PostSearchEvent
    {
        public string SearchTerm { get; set; }
        public bool SearchInDescriptions { get; set; }
        public int WorkingLanguageId { get; set; }
    }
}