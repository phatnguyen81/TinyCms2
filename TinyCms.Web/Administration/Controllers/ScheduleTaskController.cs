using System;
using System.Linq;
using System.Web.Mvc;
using TinyCms.Admin.Models.Tasks;
using TinyCms.Core.Domain.Tasks;
using TinyCms.Services.Helpers;
using TinyCms.Services.Localization;
using TinyCms.Services.Security;
using TinyCms.Services.Tasks;
using TinyCms.Web.Framework.Kendoui;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Controllers
{
    public class ScheduleTaskController : BaseAdminController
    {
        #region Constructors

        public ScheduleTaskController(IScheduleTaskService scheduleTaskService,
            IPermissionService permissionService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService)
        {
            _scheduleTaskService = scheduleTaskService;
            _permissionService = permissionService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
        }

        #endregion

        #region Utility

        [NonAction]
        protected virtual ScheduleTaskModel PrepareScheduleTaskModel(ScheduleTask task)
        {
            var model = new ScheduleTaskModel
            {
                Id = task.Id,
                Name = task.Name,
                Seconds = task.Seconds,
                Enabled = task.Enabled,
                StopOnError = task.StopOnError,
                LastStartUtc =
                    task.LastStartUtc.HasValue
                        ? _dateTimeHelper.ConvertToUserTime(task.LastStartUtc.Value, DateTimeKind.Utc).ToString("G")
                        : "",
                LastEndUtc =
                    task.LastEndUtc.HasValue
                        ? _dateTimeHelper.ConvertToUserTime(task.LastEndUtc.Value, DateTimeKind.Utc).ToString("G")
                        : "",
                LastSuccessUtc =
                    task.LastSuccessUtc.HasValue
                        ? _dateTimeHelper.ConvertToUserTime(task.LastSuccessUtc.Value, DateTimeKind.Utc).ToString("G")
                        : ""
            };
            return model;
        }

        #endregion

        #region Fields

        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            var models = _scheduleTaskService.GetAllTasks(true)
                .Select(PrepareScheduleTaskModel)
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = models,
                Total = models.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult TaskUpdate(ScheduleTaskModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult {Errors = ModelState.SerializeErrors()});
            }

            var scheduleTask = _scheduleTaskService.GetTaskById(model.Id);
            if (scheduleTask == null)
                return Content("Schedule task cannot be loaded");

            scheduleTask.Name = model.Name;
            scheduleTask.Seconds = model.Seconds;
            scheduleTask.Enabled = model.Enabled;
            scheduleTask.StopOnError = model.StopOnError;
            _scheduleTaskService.UpdateTask(scheduleTask);

            return new NullJsonResult();
        }

        public ActionResult RunNow(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            try
            {
                var scheduleTask = _scheduleTaskService.GetTaskById(id);
                if (scheduleTask == null)
                    throw new Exception("Schedule task cannot be loaded");

                var task = new Task(scheduleTask);
                //ensure that the task is enabled
                task.Enabled = true;
                //do not dispose. otherwise, we can get exception that DbContext is disposed
                task.Execute(true, false, false);
                SuccessNotification(_localizationService.GetResource("Admin.System.ScheduleTasks.RunNow.Done"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }

        #endregion
    }
}