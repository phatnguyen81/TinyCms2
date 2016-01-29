using System;
using System.Linq;
using System.Web.Mvc;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Domain.Topics;
using TinyCms.Services.Customers;
using TinyCms.Services.Localization;
using TinyCms.Services.Security;
using TinyCms.Services.Seo;
using TinyCms.Services.Topics;
using TinyCms.Web.Framework.Security;
using TinyCms.Web.Infrastructure.Cache;
using TinyCms.Web.Models.Topics;

namespace TinyCms.Web.Controllers
{
    public class TopicController : BasePublicController
    {
        #region Constructors

        public TopicController(ITopicService topicService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICacheManager cacheManager,
            IAclService aclService,
            ITopicTemplateService topicTemplateService)
        {
            _topicService = topicService;
            _workContext = workContext;
            _localizationService = localizationService;
            _cacheManager = cacheManager;
            _aclService = aclService;
            _topicTemplateService = topicTemplateService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual TopicModel PrepareTopicModel(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            var model = new TopicModel
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Title),
                Body = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Body),
                MetaKeywords = topic.GetLocalized(x => x.MetaKeywords),
                MetaDescription = topic.GetLocalized(x => x.MetaDescription),
                MetaTitle = topic.GetLocalized(x => x.MetaTitle),
                SeName = topic.GetSeName(),
                TopicTemplateId = topic.TopicTemplateId
            };
            return model;
        }

        #endregion

        #region Fields

        private readonly ITopicService _topicService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICacheManager _cacheManager;
        private readonly IAclService _aclService;
        private readonly ITopicTemplateService _topicTemplateService;

        #endregion

        #region Methods

        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult TopicDetails(int topicId)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_BY_ID_KEY,
                topicId,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var topic = _topicService.GetTopicById(topicId);
                if (topic == null)
                    return null;
                //ACL (access control list)
                if (!_aclService.Authorize(topic))
                    return null;
                return PrepareTopicModel(topic);
            }
                );

            if (cacheModel == null)
                return RedirectToRoute("HomePage");

            //template
            var templateCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_TEMPLATE_MODEL_KEY,
                cacheModel.TopicTemplateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _topicTemplateService.GetTopicTemplateById(cacheModel.TopicTemplateId);
                if (template == null)
                    template = _topicTemplateService.GetAllTopicTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });

            return View(templateViewPath, cacheModel);
        }

        public ActionResult TopicDetailsPopup(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_BY_SYSTEMNAME_KEY,
                systemName,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                //load by store
                var topic = _topicService.GetTopicBySystemName(systemName);
                if (topic == null)
                    return null;
                //ACL (access control list)
                if (!_aclService.Authorize(topic))
                    return null;
                return PrepareTopicModel(topic);
            });

            if (cacheModel == null)
                return RedirectToRoute("HomePage");

            //template
            var templateCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_TEMPLATE_MODEL_KEY,
                cacheModel.TopicTemplateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _topicTemplateService.GetTopicTemplateById(cacheModel.TopicTemplateId);
                if (template == null)
                    template = _topicTemplateService.GetAllTopicTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });

            ViewBag.IsPopup = true;
            return View(templateViewPath, cacheModel);
        }

        [ChildActionOnly]
        public ActionResult TopicBlock(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_BY_SYSTEMNAME_KEY,
                systemName,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                //load by store
                var topic = _topicService.GetTopicBySystemName(systemName);
                if (topic == null)
                    return null;
                //ACL (access control list)
                if (!_aclService.Authorize(topic))
                    return null;
                return PrepareTopicModel(topic);
            });

            if (cacheModel == null)
                return Content("");

            return PartialView(cacheModel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Authenticate(int id, string password)
        {
            var authResult = false;
            var title = string.Empty;
            var body = string.Empty;
            var error = string.Empty;

            var topic = _topicService.GetTopicById(id);
            if (topic != null &&
                //password protected?
                topic.IsPasswordProtected &&
                //ACL (access control list)
                _aclService.Authorize(topic))
            {
                if (topic.Password != null && topic.Password.Equals(password))
                {
                    authResult = true;
                    title = topic.GetLocalized(x => x.Title);
                    body = topic.GetLocalized(x => x.Body);
                }
                else
                {
                    error = _localizationService.GetResource("Topic.WrongPassword");
                }
            }
            return Json(new {Authenticated = authResult, Title = title, Body = body, Error = error});
        }

        #endregion
    }
}