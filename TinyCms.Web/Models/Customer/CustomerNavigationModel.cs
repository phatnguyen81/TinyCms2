using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Web.Models.Customer
{
    public partial class CustomerNavigationModel : BaseNopModel
    {
        public bool HideInfo { get; set; }
        public bool HideChangePassword { get; set; }
        public bool HideAvatar { get; set; }

        public CustomerNavigationEnum SelectedTab { get; set; }
    }

    public enum CustomerNavigationEnum
    {
        Info = 0,
        Addresses = 10,
        Orders = 20,
        BackInStockSubscriptions = 30,
        ReturnRequests = 40,
        DownloadablePosts = 50,
        RewardPoints = 60,
        ChangePassword = 70,
        Avatar = 80,
        ForumSubscriptions = 90
    }
}