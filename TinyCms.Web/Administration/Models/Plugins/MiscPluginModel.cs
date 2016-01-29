using System.Web.Routing;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Plugins
{
    public class MiscPluginModel : BaseNopModel
    {
        public string FriendlyName { get; set; }
        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}