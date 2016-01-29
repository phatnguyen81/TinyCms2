using System.Collections.Generic;

namespace TinyCms.Web.Framework.Themes
{
    public interface IThemeProvider
    {
        ThemeConfiguration GetThemeConfiguration(string themeName);
        IList<ThemeConfiguration> GetThemeConfigurations();
        bool ThemeConfigurationExists(string themeName);
    }
}