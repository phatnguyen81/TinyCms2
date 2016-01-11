using System.Web.Mvc;
using Nop.Plugin.ExternalAuth.Facebook.Core;
using Nop.Plugin.ExternalAuth.Facebook.Models;
using TinyCms.Core;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Plugins;
using TinyCms.Services.Authentication.External;
using TinyCms.Services.Configuration;
using TinyCms.Services.Localization;
using TinyCms.Services.Security;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Controllers;

namespace Nop.Plugin.ExternalAuth.Facebook.Controllers
{
    public class ExternalAuthFacebookController : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly IOAuthProviderFacebookAuthorizer _oAuthProviderFacebookAuthorizer;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;

        public ExternalAuthFacebookController(ISettingService settingService,
            IOAuthProviderFacebookAuthorizer oAuthProviderFacebookAuthorizer,
            IOpenAuthenticationService openAuthenticationService,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IPermissionService permissionService,
            IWorkContext workContext,
            IPluginFinder pluginFinder,
            ILocalizationService localizationService)
        {
            this._settingService = settingService;
            this._oAuthProviderFacebookAuthorizer = oAuthProviderFacebookAuthorizer;
            this._openAuthenticationService = openAuthenticationService;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return Content("Access denied");

            //load settings for a chosen store scope
            //var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var facebookExternalAuthSettings = _settingService.LoadSetting<FacebookExternalAuthSettings>();

            var model = new ConfigurationModel();
            model.ClientKeyIdentifier = facebookExternalAuthSettings.ClientKeyIdentifier;
            model.ClientSecret = facebookExternalAuthSettings.ClientSecret;

            return View("~/Plugins/ExternalAuth.Facebook/Views/ExternalAuthFacebook/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return Content("Access denied");

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            //var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var facebookExternalAuthSettings = _settingService.LoadSetting<FacebookExternalAuthSettings>();

            //save settings
            facebookExternalAuthSettings.ClientKeyIdentifier = model.ClientKeyIdentifier;
            facebookExternalAuthSettings.ClientSecret = model.ClientSecret;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            //if (model.ClientKeyIdentifier_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(facebookExternalAuthSettings, x => x.ClientKeyIdentifier, storeScope, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(facebookExternalAuthSettings, x => x.ClientKeyIdentifier, storeScope);

            //if (model.ClientSecret_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(facebookExternalAuthSettings, x => x.ClientSecret, storeScope, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(facebookExternalAuthSettings, x => x.ClientSecret, storeScope);


            _settingService.SaveSetting(facebookExternalAuthSettings, x => x.ClientKeyIdentifier, false);
            _settingService.SaveSetting(facebookExternalAuthSettings, x => x.ClientSecret, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo()
        {
            return View("~/Plugins/ExternalAuth.Facebook/Views/ExternalAuthFacebook/PublicInfo.cshtml");
        }

        [NonAction]
        private ActionResult LoginInternal(string returnUrl, bool verifyResponse)
        {
            var processor = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName("ExternalAuth.Facebook");
            if (processor == null ||
                !processor.IsMethodActive(_externalAuthenticationSettings) ||
                !processor.PluginDescriptor.Installed ||
                !_pluginFinder.AuthenticateStore(processor.PluginDescriptor))
                throw new NopException("Facebook module cannot be loaded");

            var viewModel = new LoginModel();
            TryUpdateModel(viewModel);

            var result = _oAuthProviderFacebookAuthorizer.Authorize(returnUrl, verifyResponse);
            switch (result.AuthenticationStatus)
            {
                case OpenAuthenticationStatus.Error:
                    {
                        if (!result.Success)
                            foreach (var error in result.Errors)
                                ExternalAuthorizerHelper.AddErrorsToDisplay(error);

                        return new RedirectResult(Url.LogOn(returnUrl));
                    }
                case OpenAuthenticationStatus.AssociateOnLogon:
                    {
                        return new RedirectResult(Url.LogOn(returnUrl));
                    }
                case OpenAuthenticationStatus.AutoRegisteredEmailValidation:
                    {
                        //result
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                    }
                case OpenAuthenticationStatus.AutoRegisteredAdminApproval:
                    {
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                    }
                case OpenAuthenticationStatus.AutoRegisteredStandard:
                    {
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                    }
                case OpenAuthenticationStatus.Authenticated:
                    {
                        SuccessNotification("Đăng nhập facebook thành công");
                        break;
                    }
                default:
                    break;
            }

            if (result.Result != null) return result.Result;

            return HttpContext.Request.IsAuthenticated ? new RedirectResult(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "~/") : new RedirectResult(Url.LogOn(returnUrl));
        }

        public ActionResult Login(string returnUrl)
        {
            return LoginInternal(returnUrl, false);
        }

        public ActionResult LoginCallback(string returnUrl)
        {
            return LoginInternal(returnUrl, true);
        }
    }
}