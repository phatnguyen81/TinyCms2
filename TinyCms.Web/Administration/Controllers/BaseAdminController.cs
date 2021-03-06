﻿using System.Web.Mvc;
using System.Web.Routing;
using TinyCms.Core;
using TinyCms.Core.Infrastructure;
using TinyCms.Web.Framework.Controllers;
using TinyCms.Web.Framework.Security;

namespace TinyCms.Admin.Controllers
{
    [NopHttpsRequirement(SslRequirement.Yes)]
    [AdminValidateIpAddress]
    [AdminAuthorize]
    [AdminAntiForgery]
    public abstract class BaseAdminController : BaseController
    {
        /// <summary>
        ///     Initialize controller
        /// </summary>
        /// <param name="requestContext">Request context</param>
        protected override void Initialize(RequestContext requestContext)
        {
            //set work context to admin mode
            EngineContext.Current.Resolve<IWorkContext>().IsAdmin = true;

            base.Initialize(requestContext);
        }

        /// <summary>
        ///     On exception
        /// </summary>
        /// <param name="filterContext">Filter context</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
                LogException(filterContext.Exception);
            base.OnException(filterContext);
        }

        /// <summary>
        ///     Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            return RedirectToAction("AccessDenied", "Security", new {pageUrl = Request.RawUrl});
        }

        /// <summary>
        ///     Save selected TAB index
        /// </summary>
        /// <param name="index">Idnex to save; null to automatically detect it</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected void SaveSelectedTabIndex(int? index = null, bool persistForTheNextRequest = true)
        {
            //keep this method synchronized with
            //"GetSelectedTabIndex" method of \Nop.Web.Framework\ViewEngines\Razor\WebViewPage.cs
            if (!index.HasValue)
            {
                int tmp;
                if (int.TryParse(Request.Form["selected-tab-index"], out tmp))
                {
                    index = tmp;
                }
            }
            if (index.HasValue)
            {
                var dataKey = "nop.selected-tab-index";
                if (persistForTheNextRequest)
                {
                    TempData[dataKey] = index;
                }
                else
                {
                    ViewData[dataKey] = index;
                }
            }
        }
    }
}