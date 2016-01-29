//Contributor:  Nicholas Mayne

using System.Web.Routing;
using TinyCms.Core.Plugins;

namespace TinyCms.Services.Authentication.External
{
    /// <summary>
    ///     Provides an interface for creating external authentication methods
    /// </summary>
    public interface IExternalAuthenticationMethod : IPlugin
    {
        /// <summary>
        ///     Gets a route for plugin configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetConfigurationRoute(out string actionName, out string controllerName,
            out RouteValueDictionary routeValues);

        /// <summary>
        ///     Gets a route for displaying plugin in public store
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetPublicInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);
    }
}