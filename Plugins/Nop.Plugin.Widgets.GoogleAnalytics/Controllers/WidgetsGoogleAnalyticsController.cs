

using System;
using System.Web.Mvc;
using System.Web.UI;
using Nop.Plugin.Widgets.GoogleAnalytics.Models;
using TinyCms.Core;
using TinyCms.Services.Configuration;
using TinyCms.Services.Localization;
using TinyCms.Services.Logging;
using TinyCms.Services.Posts;
using TinyCms.Web.Framework.Controllers;
using TinyCms.Core.Domain.Logging;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Controllers
{
    public class WidgetsGoogleAnalyticsController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;
        private readonly ICategoryService _categoryService;
        private readonly ILocalizationService _localizationService;

        public WidgetsGoogleAnalyticsController(IWorkContext workContext,
            ISettingService settingService, 
            ILogger logger, 
            ICategoryService categoryService,
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._settingService = settingService;
            this._logger = logger;
            this._categoryService = categoryService;
            this._localizationService = localizationService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>();
            var model = new ConfigurationModel();
            model.GoogleId = googleAnalyticsSettings.GoogleId;
            model.TrackingScript = googleAnalyticsSettings.TrackingScript;
            model.EcommerceScript = googleAnalyticsSettings.EcommerceScript;
            model.EcommerceDetailScript = googleAnalyticsSettings.EcommerceDetailScript;
            model.IncludingTax = googleAnalyticsSettings.IncludingTax;

            return View("~/Plugins/Widgets.GoogleAnalytics/Views/WidgetsGoogleAnalytics/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            //load settings for a chosen store scope
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>();
            googleAnalyticsSettings.GoogleId = model.GoogleId;
            googleAnalyticsSettings.TrackingScript = model.TrackingScript;
            googleAnalyticsSettings.EcommerceScript = model.EcommerceScript;
            googleAnalyticsSettings.EcommerceDetailScript = model.EcommerceDetailScript;
            googleAnalyticsSettings.IncludingTax = model.IncludingTax;

            _settingService.SaveSetting(googleAnalyticsSettings);
            
            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            string globalScript = "";
            var routeData = ((Page)this.HttpContext.CurrentHandler).RouteData;

            try
            {
                var controller = routeData.Values["controller"];
                var action = routeData.Values["action"];

                if (controller == null || action == null)
                    return Content("");

                globalScript += GetTrackingScript();
            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, "Error creating scripts for google ecommerce tracking", ex.ToString());
            }
            return Content(globalScript);
        }

        //<script type="text/javascript"> 

        //var _gaq = _gaq || []; 
        //_gaq.push(['_setAccount', 'UA-XXXXX-X']); 
        //_gaq.push(['_trackPageview']); 

        //(function() { 
        //var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true; 
        //ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js'; 
        //var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s); 
        //})(); 

        //</script>
        private string GetTrackingScript()
        {
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>();
            var analyticsTrackingScript = googleAnalyticsSettings.TrackingScript + "\n";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", googleAnalyticsSettings.GoogleId);
            analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", "");
            return analyticsTrackingScript;
        }
        
     

        private string FixIllegalJavaScriptChars(string text)
        {
            if (String.IsNullOrEmpty(text))
                return text;

            //replace ' with \' (http://stackoverflow.com/questions/4292761/need-to-url-encode-labels-when-tracking-events-with-google-analytics)
            text = text.Replace("'", "\\'");
            return text;
        }
    }
}