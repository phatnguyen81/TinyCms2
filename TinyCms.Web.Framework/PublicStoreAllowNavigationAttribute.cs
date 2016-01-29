using System;
using System.Web.Mvc;
using TinyCms.Core.Data;
using TinyCms.Core.Infrastructure;
using TinyCms.Services.Security;

namespace TinyCms.Web.Framework
{
    public class PublicStoreAllowNavigationAttribute : ActionFilterAttribute
    {
        private readonly bool _ignore;

        /// <summary>
        ///     Contructor
        /// </summary>
        /// <param name="ignore">Pass false in order to ignore this functionality for a certain action method</param>
        public PublicStoreAllowNavigationAttribute(bool ignore = false)
        {
            _ignore = ignore;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            //search the solution by "[PublicStoreAllowNavigation(true)]" keyword 
            //in order to find method available even when a store is closed
            if (_ignore)
                return;

            var request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            var actionName = filterContext.ActionDescriptor.ActionName;
            if (String.IsNullOrEmpty(actionName))
                return;

            var controllerName = filterContext.Controller.ToString();
            if (String.IsNullOrEmpty(controllerName))
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            var permissionService = EngineContext.Current.Resolve<IPermissionService>();
            var publicStoreAllowNavigation =
                permissionService.Authorize(StandardPermissionProvider.PublicStoreAllowNavigation);
            if (publicStoreAllowNavigation)
                return;

            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}