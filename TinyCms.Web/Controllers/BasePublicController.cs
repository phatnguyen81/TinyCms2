using System.Web.Mvc;
using System.Web.Routing;
using TinyCms.Core.Infrastructure;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Controllers;
using TinyCms.Web.Framework.Security;
using TinyCms.Web.Framework.Seo;

namespace TinyCms.Web.Controllers
{
    [StoreClosed]
    //[PublicStoreAllowNavigation]
    [LanguageSeoCode]
    [NopHttpsRequirement(SslRequirement.NoMatter)]
    [WwwRequirement]
    public abstract partial class BasePublicController : BaseController
    {
        protected virtual ActionResult InvokeHttp404()
        {
            // Call target Controller and pass the routeData.
            IController errorController = EngineContext.Current.Resolve<CommonController>();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Common");
            routeData.Values.Add("action", "PageNotFound");

            errorController.Execute(new RequestContext(this.HttpContext, routeData));

            return new EmptyResult();
        }

    }
}
