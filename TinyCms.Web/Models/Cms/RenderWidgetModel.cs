using System.Web.Routing;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Web.Models.Cms
{
    public class RenderWidgetModel : BaseNopModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}