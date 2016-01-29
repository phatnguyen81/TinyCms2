using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TinyCms.Admin.Models.Logging;
using TinyCms.Core;
using TinyCms.Core.Domain.Logging;
using TinyCms.Services.Helpers;
using TinyCms.Services.Localization;
using TinyCms.Services.Logging;
using TinyCms.Services.Security;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Controllers;
using TinyCms.Web.Framework.Kendoui;

namespace TinyCms.Admin.Controllers
{
    public class LogController : BaseAdminController
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        public LogController(ILogger logger, IWorkContext workContext,
            ILocalizationService localizationService, IDateTimeHelper dateTimeHelper,
            IPermissionService permissionService)
        {
            _logger = logger;
            _workContext = workContext;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _permissionService = permissionService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var model = new LogListModel();
            model.AvailableLogLevels = LogLevel.Debug.ToSelectList(false).ToList();
            model.AvailableLogLevels.Insert(0,
                new SelectListItem {Text = _localizationService.GetResource("Admin.Common.All"), Value = "0"});

            return View(model);
        }

        [HttpPost]
        public ActionResult LogList(DataSourceRequest command, LogListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var createdOnFromValue = (model.CreatedOnFrom == null)
                ? null
                : (DateTime?)
                    _dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            var createdToFromValue = (model.CreatedOnTo == null)
                ? null
                : (DateTime?)
                    _dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone)
                        .AddDays(1);

            var logLevel = model.LogLevelId > 0 ? (LogLevel?) (model.LogLevelId) : null;


            var logItems = _logger.GetAllLogs(createdOnFromValue, createdToFromValue, model.Message,
                logLevel, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = logItems.Select(x => new LogModel
                {
                    Id = x.Id,
                    LogLevel = x.LogLevel.GetLocalizedEnum(_localizationService, _workContext),
                    ShortMessage = x.ShortMessage,
                    //little hack here:
                    //ensure that FullMessage is not returned
                    //otherwise, we can get the following error if log records have too long FullMessage:
                    //"Error during serialization or deserialization using the JSON JavaScriptSerializer. The length of the string exceeds the value set on the maxJsonLength property. "
                    //also it improves performance
                    //FullMessage = x.FullMessage,
                    FullMessage = "",
                    IpAddress = x.IpAddress,
                    CustomerId = x.CustomerId,
                    CustomerEmail = x.Customer != null ? x.Customer.Email : null,
                    PageUrl = x.PageUrl,
                    ReferrerUrl = x.ReferrerUrl,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                }),
                Total = logItems.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("clearall")]
        public ActionResult ClearAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            _logger.ClearLog();

            SuccessNotification(_localizationService.GetResource("Admin.System.Log.Cleared"));
            return RedirectToAction("List");
        }

        public ActionResult View(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var log = _logger.GetLogById(id);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction("List");

            var model = new LogModel
            {
                Id = log.Id,
                LogLevel = log.LogLevel.GetLocalizedEnum(_localizationService, _workContext),
                ShortMessage = log.ShortMessage,
                FullMessage = log.FullMessage,
                IpAddress = log.IpAddress,
                CustomerId = log.CustomerId,
                CustomerEmail = log.Customer != null ? log.Customer.Email : null,
                PageUrl = log.PageUrl,
                ReferrerUrl = log.ReferrerUrl,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(log.CreatedOnUtc, DateTimeKind.Utc)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var log = _logger.GetLogById(id);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction("List");

            _logger.DeleteLog(log);


            SuccessNotification(_localizationService.GetResource("Admin.System.Log.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var logItems = _logger.GetLogByIds(selectedIds.ToArray());
                foreach (var logItem in logItems)
                    _logger.DeleteLog(logItem);
            }

            return Json(new {Result = true});
        }
    }
}