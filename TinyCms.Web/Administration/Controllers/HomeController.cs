using System.Web.Mvc;
using TinyCms.Admin.Models.Home;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Domain.Common;
using TinyCms.Services.Configuration;

namespace TinyCms.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        #region Ctor

        public HomeController(
            CommonSettings commonSettings,
            ISettingService settingService,
            IWorkContext workContext,
            ICacheManager cacheManager)
        {
            _commonSettings = commonSettings;
            _settingService = settingService;
            _workContext = workContext;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Fields

        private readonly CommonSettings _commonSettings;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Methods

        public ActionResult Index()
        {
            var model = new DashboardModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult NopCommerceNewsHideAdv()
        {
            _commonSettings.HideAdvertisementsOnAdminArea = !_commonSettings.HideAdvertisementsOnAdminArea;
            _settingService.SaveSetting(_commonSettings);
            return Content("Setting changed");
        }

        #endregion
    }
}