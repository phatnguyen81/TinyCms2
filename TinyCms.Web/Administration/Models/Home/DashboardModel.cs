using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Home
{
    public class DashboardModel : BaseNopModel
    {
        public bool IsLoggedInAsVendor { get; set; }
    }
}