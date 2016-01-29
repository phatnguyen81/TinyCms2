using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using TinyCms.Admin.Extensions;
using TinyCms.Admin.Models.Messages;
using TinyCms.Core.Domain.Messages;
using TinyCms.Services.Localization;
using TinyCms.Services.Messages;
using TinyCms.Services.Security;
using TinyCms.Web.Framework.Controllers;
using TinyCms.Web.Framework.Kendoui;

namespace TinyCms.Admin.Controllers
{
    public class MessageTemplateController : BaseAdminController
    {
        #region Constructors

        public MessageTemplateController(IMessageTemplateService messageTemplateService,
            IEmailAccountService emailAccountService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            IMessageTokenProvider messageTokenProvider,
            IPermissionService permissionService,
            IWorkflowMessageService workflowMessageService,
            EmailAccountSettings emailAccountSettings)
        {
            _messageTemplateService = messageTemplateService;
            _emailAccountService = emailAccountService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _localizationService = localizationService;
            _messageTokenProvider = messageTokenProvider;
            _permissionService = permissionService;
            _workflowMessageService = workflowMessageService;
            _emailAccountSettings = emailAccountSettings;
        }

        #endregion

        #region Fields

        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IPermissionService _permissionService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly EmailAccountSettings _emailAccountSettings;

        #endregion Fields

        #region Utilities

        private string FormatTokens(string[] tokens)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                sb.Append(token);
                if (i != tokens.Length - 1)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        [NonAction]
        protected virtual void UpdateLocales(MessageTemplate mt, MessageTemplateModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(mt,
                    x => x.BccEmailAddresses,
                    localized.BccEmailAddresses,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                    x => x.Subject,
                    localized.Subject,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                    x => x.Body,
                    localized.Body,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                    x => x.EmailAccountId,
                    localized.EmailAccountId,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var model = new MessageTemplateListModel();
            //stores

            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, MessageTemplateListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplates = _messageTemplateService.GetAllMessageTemplates();
            var gridModel = new DataSourceResult
            {
                Data = messageTemplates.Select(x =>
                {
                    var templateModel = x.ToModel();
                    return templateModel;
                }),
                Total = messageTemplates.Count
            };

            return Json(gridModel);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            var model = messageTemplate.ToModel();
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens());
            //available email accounts
            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
                model.AvailableEmailAccounts.Add(ea.ToModel());
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.BccEmailAddresses = messageTemplate.GetLocalized(x => x.BccEmailAddresses, languageId, false,
                    false);
                locale.Subject = messageTemplate.GetLocalized(x => x.Subject, languageId, false, false);
                locale.Body = messageTemplate.GetLocalized(x => x.Body, languageId, false, false);

                var emailAccountId = messageTemplate.GetLocalized(x => x.EmailAccountId, languageId, false, false);
                locale.EmailAccountId = emailAccountId > 0
                    ? emailAccountId
                    : _emailAccountSettings.DefaultEmailAccountId;
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(MessageTemplateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                messageTemplate = model.ToEntity(messageTemplate);
                //attached file
                if (!model.HasAttachedDownload)
                    messageTemplate.AttachedDownloadId = 0;
                _messageTemplateService.UpdateMessageTemplate(messageTemplate);
                //locales
                UpdateLocales(messageTemplate, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();

                    return RedirectToAction("Edit", new {id = messageTemplate.Id});
                }
                return RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens());
            //available email accounts
            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
                model.AvailableEmailAccounts.Add(ea.ToModel());
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            _messageTemplateService.DeleteMessageTemplate(messageTemplate);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("message-template-copy")]
        public ActionResult CopyTemplate(MessageTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            try
            {
                var newMessageTemplate = _messageTemplateService.CopyMessageTemplate(messageTemplate);
                SuccessNotification("The message template has been copied successfully");
                return RedirectToAction("Edit", new {id = newMessageTemplate.Id});
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new {id = model.Id});
            }
        }

        public ActionResult TestTemplate(int id, int languageId = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            var model = new TestMessageTemplateModel
            {
                Id = messageTemplate.Id,
                LanguageId = languageId
            };
            var tokens = _messageTokenProvider.GetListOfAllowedTokens().Distinct().ToList();
            //filter them to the current template
            var subject = messageTemplate.GetLocalized(mt => mt.Subject, languageId);
            var body = messageTemplate.GetLocalized(mt => mt.Body, languageId);
            //subject = messageTemplate.Subject;
            //body = messageTemplate.Body;
            tokens = tokens.Where(x => subject.Contains(x) || body.Contains(x)).ToList();
            model.Tokens = tokens;

            return View(model);
        }

        [HttpPost, ActionName("TestTemplate")]
        [FormValueRequired("send-test")]
        [ValidateInput(false)]
        public ActionResult TestTemplate(TestMessageTemplateModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            var tokens = new List<Token>();
            foreach (var formKey in form.AllKeys)
                if (formKey.StartsWith("token_", StringComparison.InvariantCultureIgnoreCase))
                {
                    var tokenKey = formKey.Substring("token_".Length).Replace("%", "");
                    var tokenValue = form[formKey];
                    tokens.Add(new Token(tokenKey, tokenValue));
                }

            _workflowMessageService.SendTestEmail(messageTemplate.Id, model.SendTo, tokens, model.LanguageId);

            if (ModelState.IsValid)
            {
                SuccessNotification(
                    _localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Test.Success"));
            }

            return RedirectToAction("Edit", new {id = messageTemplate.Id});
        }

        #endregion
    }
}