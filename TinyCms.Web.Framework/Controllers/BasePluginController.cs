using System.Web.Mvc;

namespace TinyCms.Web.Framework.Controllers
{
    /// <summary>
    /// Base controller for plugins
    /// </summary>
    public abstract class BasePluginController : BaseController
    {
        protected ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });
        }
    }
}
