using System.Web.Routing;

namespace TinyCms.Web.Framework.Mvc.Routes
{
    public interface IRouteProvider
    {
        int Priority { get; }
        void RegisterRoutes(RouteCollection routes);
    }
}