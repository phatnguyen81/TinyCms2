﻿using System;
using System.Linq;
using System.Web.Mvc;
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

        public WidgetsAdsBannerController(IWorkContext workContext,
            IPictureService pictureService,
            ISettingService settingService,
            ICacheManager cacheManager,
            ILocalizationService localizationService, IAdsBannerService adsBannerService, IWidgetService widgetService, IDateTimeHelper dateTimeHelper)
        {
            this._workContext = workContext;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._localizationService = localizationService;
            _adsBannerService = adsBannerService;
            _widgetService = widgetService;
            _dateTimeHelper = dateTimeHelper;
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
            var adsbanners = _adsBannerService.GetAllAdsBanners(model.SearchAdsBannerName,null,null,
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
            var model = new AdsBannerModel();
            PrepareAdsBannerModel(model);
            return View("~/Plugins/Widgets.AdsBanner/Views/WidgetsAdsBanner/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(AdsBannerModel model, bool continueEditing)
        {
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
            var adsbanner = _adsBannerService.GetById(model.Id);
            if (adsbanner == null)
                return RedirectToAction("ConfigureWidget", "Widget",
                    new { area = "Admin", systemName = "Widgets.AdsBanner" });
            if (ModelState.IsValid)
            {
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
   
        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {

            var model = new PublicInfoModel();


            return View("~/Plugins/Widgets.AdsBanner/Views/WidgetsAdsBanner/PublicInfo.cshtml", model);
        }
    }
}