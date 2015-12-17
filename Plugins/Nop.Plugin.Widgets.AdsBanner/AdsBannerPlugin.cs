using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Web.Routing;
using Nop.Plugin.Widgets.AdsBanner.Data;
using TinyCms.Core;
using TinyCms.Core.Plugins;
using TinyCms.Services.Cms;
using TinyCms.Services.Configuration;
using TinyCms.Services.Localization;
using TinyCms.Services.Media;


namespace Nop.Plugin.Widgets.AdsBanner
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class AdsBannerPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly AdsBannerObjectContext _context;

        public AdsBannerPlugin(IPictureService pictureService, 
            ISettingService settingService, IWebHelper webHelper, AdsBannerObjectContext context)
        {
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._webHelper = webHelper;
            _context = context;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "home_page_top" };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsAdsBanner";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.AdsBanner.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsAdsBanner";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Widgets.AdsBanner.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _context.Install();

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Name", "Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Picture", "Picture");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.WidgetZone", "WidgetZone");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Url", "Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.FromDate", "From date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.ToDate", "To date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Published", "Published");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.DisplayOrder", "Display order");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.List.SearchAdsBannerName", "Search banner name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Manage", "Banner management");
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _context.Uninstall();

            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Name");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Picture");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.WidgetZone");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Url");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.FromDate");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.ToDate");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Published");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.DisplayOrder");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.List.SearchAdsBannerName");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Manage");

            base.Uninstall();
        }
    }
}
