using System;
using System.Linq;
using System.Web.Mvc;
using TinyCms.Admin.Extensions;
using TinyCms.Admin.Models.Messages;
using TinyCms.Core;
using TinyCms.Core.Domain;
using TinyCms.Core.Domain.Messages;
using TinyCms.Core.Infrastructure;
using TinyCms.Services.Configuration;
using TinyCms.Services.Localization;
using TinyCms.Services.Messages;
using TinyCms.Services.Security;
using TinyCms.Web.Framework.Controllers;
using TinyCms.Web.Framework.Kendoui;

namespace TinyCms.Admin.Controllers
{
    public class EmailAccountController : BaseAdminController
    {
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailSender _emailSender;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        public EmailAccountController(IEmailAccountService emailAccountService,
            ILocalizationService localizationService, ISettingService settingService,
            IEmailSender emailSender,
            EmailAccountSettings emailAccountSettings, IPermissionService permissionService)
        {
            _emailAccountService = emailAccountService;
            _localizationService = localizationService;
            _emailAccountSettings = emailAccountSettings;
            _emailSender = emailSender;
            _settingService = settingService;
            _permissionService = permissionService;
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccountModels = _emailAccountService.GetAllEmailAccounts()
                .Select(x => x.ToModel())
                .ToList();
            foreach (var eam in emailAccountModels)
                eam.IsDefaultEmailAccount = eam.Id == _emailAccountSettings.DefaultEmailAccountId;

            var gridModel = new DataSourceResult
            {
                Data = emailAccountModels,
                Total = emailAccountModels.Count()
            };

            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult MarkAsDefaultEmail(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var defaultEmailAccount = _emailAccountService.GetEmailAccountById(id);
            if (defaultEmailAccount != null)
            {
                _emailAccountSettings.DefaultEmailAccountId = defaultEmailAccount.Id;
                _settingService.SaveSetting(_emailAccountSettings);
            }
            return RedirectToAction("List");
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var model = new EmailAccountModel();
            //default values
            model.Port = 25;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(EmailAccountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var emailAccount = model.ToEntity();
                //set password manually
                emailAccount.Password = model.Password;
                _emailAccountService.InsertEmailAccount(emailAccount);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.EmailAccounts.Added"));
                return continueEditing ? RedirectToAction("Edit", new {id = emailAccount.Id}) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccount = _emailAccountService.GetEmailAccountById(id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            return View(emailAccount.ToModel());
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(EmailAccountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccount = _emailAccountService.GetEmailAccountById(model.Id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                emailAccount = model.ToEntity(emailAccount);
                _emailAccountService.UpdateEmailAccount(emailAccount);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.EmailAccounts.Updated"));
                return continueEditing ? RedirectToAction("Edit", new {id = emailAccount.Id}) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public ActionResult ChangePassword(EmailAccountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccount = _emailAccountService.GetEmailAccountById(model.Id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            //do not validate model
            emailAccount.Password = model.Password;
            _emailAccountService.UpdateEmailAccount(emailAccount);
            SuccessNotification(
                _localizationService.GetResource("Admin.Configuration.EmailAccounts.Fields.Password.PasswordChanged"));
            return RedirectToAction("Edit", new {id = emailAccount.Id});
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("sendtestemail")]
        public ActionResult SendTestEmail(EmailAccountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccount = _emailAccountService.GetEmailAccountById(model.Id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            try
            {
                if (String.IsNullOrWhiteSpace(model.SendTestEmailTo))
                    throw new NopException("Enter test email address");
                var storeSettings = EngineContext.Current.Resolve<StoreInformationSettings>();
                var subject = storeSettings.Name + ". Testing email functionality.";
                var body = "Email works fine.";
                _emailSender.SendEmail(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName,
                    model.SendTestEmailTo, null);
                SuccessNotification(
                    _localizationService.GetResource("Admin.Configuration.EmailAccounts.SendTestEmail.Success"), false);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message, false);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageEmailAccounts))
                return AccessDeniedView();

            var emailAccount = _emailAccountService.GetEmailAccountById(id);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            try
            {
                _emailAccountService.DeleteEmailAccount(emailAccount);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.EmailAccounts.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new {id = emailAccount.Id});
            }
        }
    }
}