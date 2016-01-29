namespace TinyCms.Web.Framework.Kendoui
{
    public class DataSourceRequest
    {
        public DataSourceRequest()
        {
            Page = 1;
            PageSize = 10;
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}