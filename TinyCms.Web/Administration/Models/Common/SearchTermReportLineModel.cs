using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Common
{
    public class SearchTermReportLineModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.SearchTermReport.Keyword")]
        public string Keyword { get; set; }

        [NopResourceDisplayName("Admin.SearchTermReport.Count")]
        public int Count { get; set; }
    }
}