using System.Web.Routing;
using TinyCms.Web.Framework.Localization;
using TinyCms.Web.Framework.Mvc.Routes;
using TinyCms.Web.Framework.Seo;

namespace TinyCms.Web.Infrastructure
{
    public class GenericUrlRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //generic URLs
            routes.MapGenericPathRoute("GenericUrl",
                "{generic_se_name}",
                new {controller = "Common", action = "GenericUrl"},
                new[] {"TinyCms.Web.Controllers"});

            //define this routes to use in UI views (in case if you want to customize some of them later)
            routes.MapLocalizedRoute("Post",
                "{SeName}",
                new {controller = "Post", action = "PostDetails"},
                new[] {"TinyCms.Web.Controllers"});

            routes.MapLocalizedRoute("Category",
                "{SeName}",
                new {controller = "Posts", action = "Category"},
                new[] {"TinyCms.Web.Controllers"});


            routes.MapLocalizedRoute("Topic",
                "{SeName}",
                new {controller = "Topic", action = "TopicDetails"},
                new[] {"TinyCms.Web.Controllers"});


            //the last route. it's used when none of registered routes could be used for the current request
            //but it this case we cannot process non-registered routes (/controller/action)
            //routes.MapLocalizedRoute(
            //    "PageNotFound-Wildchar",
            //    "{*url}",
            //    new { controller = "Common", action = "PageNotFound" },
            //    new[] { "TinyCms.Web.Controllers" });
        }

        public int Priority
        {
            get
            {
                //it should be the last route
                //we do not set it to -int.MaxValue so it could be overridden (if required)
                return -1000000;
            }
        }
    }
}