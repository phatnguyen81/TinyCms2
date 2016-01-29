using System;
using System.Linq;
using TinyCms.Core;
using TinyCms.Core.Domain;
using TinyCms.Core.Domain.Customers;
using TinyCms.Services.Common;

namespace TinyCms.Web.Framework.Themes
{
    /// <summary>
    ///     Theme context
    /// </summary>
    public class ThemeContext : IThemeContext
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IThemeProvider _themeProvider;
        private readonly IWorkContext _workContext;
        private string _cachedThemeName;
        private bool _themeIsCached;

        public ThemeContext(IWorkContext workContext,
            IGenericAttributeService genericAttributeService,
            StoreInformationSettings storeInformationSettings,
            IThemeProvider themeProvider)
        {
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
            _storeInformationSettings = storeInformationSettings;
            _themeProvider = themeProvider;
        }

        /// <summary>
        ///     Get or set current theme system name
        /// </summary>
        public string WorkingThemeName
        {
            get
            {
                if (_themeIsCached)
                    return _cachedThemeName;

                var theme = "";
                if (_storeInformationSettings.AllowCustomerToSelectTheme)
                {
                    if (_workContext.CurrentCustomer != null)
                        theme =
                            _workContext.CurrentCustomer.GetAttribute<string>(
                                SystemCustomerAttributeNames.WorkingThemeName, _genericAttributeService);
                }

                //default store theme
                if (string.IsNullOrEmpty(theme))
                    theme = _storeInformationSettings.DefaultStoreTheme;

                //ensure that theme exists
                if (!_themeProvider.ThemeConfigurationExists(theme))
                {
                    var themeInstance = _themeProvider.GetThemeConfigurations()
                        .FirstOrDefault();
                    if (themeInstance == null)
                        throw new Exception("No theme could be loaded");
                    theme = themeInstance.ThemeName;
                }

                //cache theme
                _cachedThemeName = theme;
                _themeIsCached = true;
                return theme;
            }
            set
            {
                if (!_storeInformationSettings.AllowCustomerToSelectTheme)
                    return;

                if (_workContext.CurrentCustomer == null)
                    return;

                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.WorkingThemeName, value);

                //clear cache
                _themeIsCached = false;
            }
        }
    }
}