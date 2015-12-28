using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Nop.Plugin.Widgets.AdsBanner.Data;
using Nop.Plugin.Widgets.AdsBanner.Domain;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Events;
using TinyCms.Core.Plugins;
using TinyCms.Services.Cms;
using TinyCms.Services.Configuration;
using TinyCms.Services.Events;
using TinyCms.Services.Localization;
using TinyCms.Services.Media;


namespace Nop.Plugin.Widgets.AdsBanner
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class AdsBannerPlugin : BasePlugin, IWidgetPlugin, IConsumer<EntityUpdated<AdsBannerRecord>>, IConsumer<EntityInserted<AdsBannerRecord>>, IConsumer<EntityDeleted<AdsBannerRecord>>
    {
        public const string SEARCH_ALL_ADSBANNERS_MODEL_KEY = "Cms.pres.search.adsbanners";
        public const string SEARCH_ACTIVEFROMNOW_ADSBANNERS_MODEL_KEY = "Cms.pres.search.adsbanners-{0}";


        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IWidgetService _widgetService;
        private readonly ICacheManager _cacheManager;
        private readonly AdsBannerObjectContext _context;

        public AdsBannerPlugin(IPictureService pictureService, 
            ISettingService settingService, IWebHelper webHelper, AdsBannerObjectContext context, IWidgetService widgetService, ICacheManager cacheManager)
        {
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._webHelper = webHelper;
            _context = context;
            _widgetService = widgetService;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return _widgetService.LoaddAllWidgetZones().Select(q => q.SystemName).ToList();
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
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Name.Hint", "Name of banner");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Picture", "Picture");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Picture.Hint", "Picture of banner");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.WidgetZone", "Widget Zone");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.WidgetZone.Hint", "Zone to show banner");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Url", "Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Url.Hint", "Redirect to link when click to banner");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.FromDate", "From date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.FromDate.Hint", "Show banner after from date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.ToDate", "To date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.ToDate.Hint", "Show banner before from date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Published", "Published");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Published.Hint", "Published");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.DisplayOrder", "Display order");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.DisplayOrder.Hint", "Display order");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.List.SearchAdsBannerName", "Search banner name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.List.SearchAdsBannerName.Hint", "Search banner name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Manage", "Banner management");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.AddNew", "Add new Ads banner");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.EditAdsBannerDetail", "Edit Ads banner");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Added", "Add new Ads Banner successful");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Updated", "Update Ads Banner successful");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.Deleted", "Delete Ads Banner successful");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AdsBanner.BackToList", "Back to ads banner list");
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _context.Uninstall();

            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Name");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Name.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Picture");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Picture.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.WidgetZone");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.WidgetZone.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Url");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Url.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.FromDate");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.FromDate.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.ToDate");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.ToDate.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Published");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Published.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.DisplayOrder");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.DisplayOrder.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.List.SearchAdsBannerName");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.List.SearchAdsBannerName.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Manage");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.AddNew");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.EditAdsBannerDetail");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Added");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Updated");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.Deleted");
            this.DeletePluginLocaleResource("Plugins.Widgets.AdsBanner.BackToList");

            base.Uninstall();
        }

        public void HandleEvent(EntityUpdated<AdsBannerRecord> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_ALL_ADSBANNERS_MODEL_KEY);
        }

        public void HandleEvent(EntityInserted<AdsBannerRecord> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_ALL_ADSBANNERS_MODEL_KEY);
        }

        public void HandleEvent(EntityDeleted<AdsBannerRecord> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_ALL_ADSBANNERS_MODEL_KEY);
        }
    }
}
