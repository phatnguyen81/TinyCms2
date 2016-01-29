using System;
using System.Web.Mvc;
using TinyCms.Core;
using TinyCms.Core.Data;
using TinyCms.Core.Domain;
using TinyCms.Core.Domain.Security;
using TinyCms.Core.Infrastructure;

namespace TinyCms.Web.Framework.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NopHttpsRequirementAttribute : FilterAttribute, IAuthorizationFilter
    {
        public NopHttpsRequirementAttribute(SslRequirement sslRequirement)
        {
            SslRequirement = sslRequirement;
        }

        public SslRequirement SslRequirement { get; set; }

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            // only redirect for GET requests, 
            // otherwise the browser might not propagate the verb and request body correctly.
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;
            var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();
            if (securitySettings.ForceSslForAllPages)
                //all pages are forced to be SSL no matter of the specified value
                SslRequirement = SslRequirement.Yes;

            switch (SslRequirement)
            {
                case SslRequirement.Yes:
                {
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    var currentConnectionSecured = webHelper.IsCurrentConnectionSecured();
                    if (!currentConnectionSecured)
                    {
                        var storeSettings = EngineContext.Current.Resolve<StoreInformationSettings>();
                        if (storeSettings.SslEnabled)
                        {
                            //redirect to HTTPS version of page
                            //string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                            var url = webHelper.GetThisPageUrl(true, true);

                            //301 (permanent) redirection
                            filterContext.Result = new RedirectResult(url, true);
                        }
                    }
                }
                    break;
                case SslRequirement.No:
                {
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    var currentConnectionSecured = webHelper.IsCurrentConnectionSecured();
                    if (currentConnectionSecured)
                    {
                        //redirect to HTTP version of page
                        //string url = "http://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                        var url = webHelper.GetThisPageUrl(true, false);
                        //301 (permanent) redirection
                        filterContext.Result = new RedirectResult(url, true);
                    }
                }
                    break;
                case SslRequirement.NoMatter:
                {
                    //do nothing
                }
                    break;
                default:
                    throw new NopException("Not supported SslProtected parameter");
            }
        }
    }
}