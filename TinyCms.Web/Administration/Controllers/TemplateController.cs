using System;
using System.Linq;
using System.Web.Mvc;
using TinyCms.Admin.Extensions;
using TinyCms.Admin.Models.Templates;
using TinyCms.Core.Domain.Posts;
using TinyCms.Core.Domain.Topics;
using TinyCms.Services.Posts;
using TinyCms.Services.Security;
using TinyCms.Services.Topics;
using TinyCms.Web.Framework.Kendoui;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Controllers
{
    public partial class TemplateController : BaseAdminController
    {
        #region Fields

        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IPostTemplateService _postTemplateService;
        private readonly ITopicTemplateService _topicTemplateService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Constructors

        public TemplateController(ICategoryTemplateService categoryTemplateService,
            IPostTemplateService postTemplateService,
            ITopicTemplateService topicTemplateService,
            IPermissionService permissionService)
        {
            this._categoryTemplateService = categoryTemplateService;
            this._postTemplateService = postTemplateService;
            this._topicTemplateService = topicTemplateService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Category templates

        public ActionResult CategoryTemplates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult CategoryTemplates(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var templatesModel = _categoryTemplateService.GetAllCategoryTemplates()
                .Select(x => x.ToModel())
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = templatesModel,
                Total = templatesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult CategoryTemplateUpdate(CategoryTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var template = _categoryTemplateService.GetCategoryTemplateById(model.Id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");
            template = model.ToEntity(template);
            _categoryTemplateService.UpdateCategoryTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult CategoryTemplateAdd([Bind(Exclude = "Id")] CategoryTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var template = new CategoryTemplate();
            template = model.ToEntity(template);
            _categoryTemplateService.InsertCategoryTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult CategoryTemplateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var template = _categoryTemplateService.GetCategoryTemplateById(id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");

            _categoryTemplateService.DeleteCategoryTemplate(template);

            return new NullJsonResult();
        }

        #endregion

     

        //#region Post templates

        //public ActionResult PostTemplates()
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
        //        return AccessDeniedView();

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult PostTemplates(DataSourceRequest command)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
        //        return AccessDeniedView();

        //    var templatesModel = _postTemplateService.GetAllPostTemplates()
        //        .Select(x => x.ToModel())
        //        .ToList();
        //    var model = new DataSourceResult
        //    {
        //        Data = templatesModel,
        //        Total = templatesModel.Count
        //    };

        //    return new JsonResult
        //    {
        //        Data = model
        //    };
        //}

        //[HttpPost]
        //public ActionResult PostTemplateUpdate(PostTemplateModel model)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
        //        return AccessDeniedView();

        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
        //    }

        //    var template = _postTemplateService.GetPostTemplateById(model.Id);
        //    if (template == null)
        //        throw new ArgumentException("No template found with the specified id");
        //    template = model.ToEntity(template);
        //    _postTemplateService.UpdatePostTemplate(template);

        //    return new NullJsonResult();
        //}

        //[HttpPost]
        //public ActionResult PostTemplateAdd([Bind(Exclude = "Id")] PostTemplateModel model)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
        //        return AccessDeniedView();

        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
        //    }

        //    var template = new PostTemplate();
        //    template = model.ToEntity(template);
        //    _postTemplateService.InsertPostTemplate(template);

        //    return new NullJsonResult();
        //}

        //[HttpPost]
        //public ActionResult PostTemplateDelete(int id)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
        //        return AccessDeniedView();

        //    var template = _postTemplateService.GetPostTemplateById(id);
        //    if (template == null)
        //        throw new ArgumentException("No template found with the specified id");

        //    _postTemplateService.DeletePostTemplate(template);

        //    return new NullJsonResult();
        //}

        //#endregion

        #region Topic templates

        public ActionResult TopicTemplates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult TopicTemplates(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var templatesModel = _topicTemplateService.GetAllTopicTemplates()
                .Select(x => x.ToModel())
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = templatesModel,
                Total = templatesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult TopicTemplateUpdate(TopicTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var template = _topicTemplateService.GetTopicTemplateById(model.Id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");
            template = model.ToEntity(template);
            _topicTemplateService.UpdateTopicTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult TopicTemplateAdd([Bind(Exclude = "Id")] TopicTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var template = new TopicTemplate();
            template = model.ToEntity(template);
            _topicTemplateService.InsertTopicTemplate(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult TopicTemplateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var template = _topicTemplateService.GetTopicTemplateById(id);
            if (template == null)
                throw new ArgumentException("No template found with the specified id");

            _topicTemplateService.DeleteTopicTemplate(template);

            return new NullJsonResult();
        }

        #endregion
    }
}
