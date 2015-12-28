using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Plugin.Widgets.AdsBanner.Domain;
using Nop.Plugin.Widgets.AdsBanner.Extensions;
using Nop.Plugin.Widgets.AdsBanner.Infrastructure.Cache;
using Nop.Plugin.Widgets.AdsBanner.Models;
using Nop.Plugin.Widgets.AdsBanner.Services;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Services.Cms;
using TinyCms.Services.Configuration;
using TinyCms.Services.Helpers;
using TinyCms.Services.Localization;
using TinyCms.Services.Media;
using TinyCms.Services.Security;
using TinyCms.Web.Framework.Controllers;
using TinyCms.Web.Framework.Kendoui;

namespace Nop.Plugin.Widgets.AdsBanner.Controllers
{
    public class WidgetsAdsBannerController : BasePluginController
    {

        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly ICacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly IAdsBannerService _adsBannerService;
        private readonly IWidgetService _widgetService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;

        public WidgetsAdsBannerController(IWorkContext workContext,
            IPictureService pictureService,
            ISettingService settingService,
            ICacheManager cacheManager,
            ILocalizationService localizationService,
            IAdsBannerService adsBannerService,
            IWidgetService widgetService,
            IDateTimeHelper dateTimeHelper,
            IPermissionService permissionService)
        {
            this._workContext = workContext;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._localizationService = localizationService;
            _adsBannerService = adsBannerService;
            _widgetService = widgetService;
            _dateTimeHelper = dateTimeHelper;
            _permissionService = permissionService;
        }

        protected string GetPictureUrl(int pictureId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.PICTURE_URL_MODEL_KEY, pictureId);
            return _cacheManager.Get(cacheKey, () =>
            {
                var url = _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false);
                //little hack here. nulls aren't cacheable so set it to ""
                if (url == null)
                    url = "";

                return url;
            });
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new AdsBannerListModel();
            return View("~/Plugins/Widgets.AdsBanner/Views/WidgetsAdsBanner/Configure.cshtml", model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, AdsBannerListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();
            var adsbanners = _adsBannerService.GetAllAdsBanners(model.SearchAdsBannerName,null,
                command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = adsbanners.Select(q =>
                {
                    var ads = q.ToModel();
                    if (q.FromDateUtc != null)
                        ads.FromDate = _dateTimeHelper.ConvertToUserTime(q.FromDateUtc.Value, DateTimeKind.Utc);
                    if (q.ToDateUtc != null)
                        ads.ToDate = _dateTimeHelper.ConvertToUserTime(q.ToDateUtc.Value, DateTimeKind.Utc);
                    return ads;
                }),
                Total = adsbanners.TotalCount
            };
            return Json(gridModel);
        }


        public void PrepareAdsBannerModel(AdsBannerModel model)
        {
            model.AvailableWidgetZones = _widgetService.LoaddAllWidgetZones().Select(q=>new SelectListItem
            {
                Text = q.Name,
                Value = q.Id.ToString()
            }).ToList();
        }
        [NonAction]
        protected virtual void UpdatePictureSeoNames(AdsBannerModel adsBanner)
        {
            var picture = _pictureService.GetPictureById(adsBanner.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(adsBanner.Name));
        }
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();
            var model = new AdsBannerModel();
            PrepareAdsBannerModel(model);
            return View("~/Plugins/Widgets.AdsBanner/Views/WidgetsAdsBanner/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(AdsBannerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                var adsbanner = model.ToEntity();
                if (model.FromDate != null)
                    adsbanner.FromDateUtc = _dateTimeHelper.ConvertToUtcTime(model.FromDate.Value);
                if (model.ToDate != null)
                    adsbanner.ToDateUtc = _dateTimeHelper.ConvertToUtcTime(model.ToDate.Value);

                _adsBannerService.InsertAdsBanner(adsbanner);

                UpdatePictureSeoNames(model);

                SuccessNotification(_localizationService.GetResource("Plugins.Widgets.AdsBanner.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = adsbanner.Id }) : RedirectToAction("ConfigureWidget", "Widget", new { area = "Admin", systemName = "Widgets.AdsBanner" });
            }


            PrepareAdsBannerModel(model);
            return View("~/Plugins/Widgets.AdsBanner/Views/WidgetsAdsBanner/Create.cshtml", model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();
            var ads = _adsBannerService.GetById(id);
            if (ads == null)
                return RedirectToAction("ConfigureWidget", "Widget",
                    new {area = "Admin", systemName = "Widgets.AdsBanner"});

            var model = ads.ToModel();
            if (ads.FromDateUtc != null) model.FromDate = _dateTimeHelper.ConvertToUserTime(ads.FromDateUtc.Value, DateTimeKind.Utc);
            if (ads.ToDateUtc != null) model.ToDate = _dateTimeHelper.ConvertToUserTime(ads.ToDateUtc.Value, DateTimeKind.Utc);
            PrepareAdsBannerModel(model);

            return View("~/Plugins/Widgets.AdsBanner/Views/WidgetsAdsBanner/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(AdsBannerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();
            var adsbanner = _adsBannerService.GetById(model.Id);
            if (adsbanner == null)
                return RedirectToAction("ConfigureWidget", "Widget",
                    new { area = "Admin", systemName = "Widgets.AdsBanner" });
            if (ModelState.IsValid)
            {

                adsbanner = model.ToEntity(adsbanner);

                int prevPictureId = adsbanner.PictureId;

                _adsBannerService.UpdateAdsBanner(adsbanner);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != adsbanner.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                SuccessNotification(_localizationService.GetResource("Plugins.Widgets.AdsBanner.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = adsbanner.Id }) : RedirectToAction("ConfigureWidget", "Widget", new { area = "Admin", systemName = "Widgets.AdsBanner" });
            }

            PrepareAdsBannerModel(model);

            return View("~/Plugins/Widgets.AdsBanner/Views/WidgetsAdsBanner/Edit.cshtml", model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();
            var adsbanner = _adsBannerService.GetById(id);
            if (adsbanner == null)
                return RedirectToAction("ConfigureWidget", "Widget",
                    new { area = "Admin", systemName = "Widgets.AdsBanner" });

            int pictureId = adsbanner.PictureId;

            _adsBannerService.DeleteAdsBanner(adsbanner);

            _pictureService.DeletePicture(_pictureService.GetPictureById(pictureId));

            SuccessNotification(_localizationService.GetResource("Plugins.Widgets.AdsBanner.Deleted"));
            return RedirectToAction("ConfigureWidget", "Widget", new { area = "Admin", systemName = "Widgets.AdsBanner" });
        }

        public List<ShowAdsBannerModel> PrepareShowAdsBannerModel(IList<AdsBannerRecord> adsBanners )
        {
            return adsBanners.Select(q =>
            {
                var model = new ShowAdsBannerModel
                {
                    PictureUrl = _pictureService.GetPictureUrl(q.PictureId),
                    Link = q.Url
                };
                return model;
            }).ToList();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {

            string cacheKey = string.Format(AdsBannerPlugin.SEARCH_ACTIVEFROMNOW_ADSBANNERS_MODEL_KEY,
                widgetZone);
            var adsBannerModels = _cacheManager.Get(cacheKey, () =>
            {
                var wz = _widgetService.GetAllWidgetZones().FirstOrDefault(q => q.SystemName == widgetZone);
                var adsbanner = _adsBannerService.GetAllAdsBannersActiveFromNow(widgetZoneId: wz.Id).ToList();
                var now = DateTime.UtcNow;
                return PrepareShowAdsBannerModel(adsbanner.Where(q => q.FromDateUtc == null || q.FromDateUtc <= now).ToList());
            });

            var model = new PublicInfoModel {AdsBanners = adsBannerModels};


            return View("~/Plugins/Widgets.AdsBanner/Views/WidgetsAdsBanner/PublicInfo.cshtml", model);
        }
    }
}