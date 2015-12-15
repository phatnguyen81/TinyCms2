using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Home
{
    public partial class DashboardModel : BaseNopModel
    {
        public bool IsLoggedInAsVendor { get; set; }
    }
}