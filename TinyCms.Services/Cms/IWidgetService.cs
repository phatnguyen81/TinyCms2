using System.Collections.Generic;
using TinyCms.Core.Domain.Cms;

namespace TinyCms.Services.Cms
{
    /// <summary>
    ///     Widget service interface
    /// </summary>
    public interface IWidgetService
    {
        /// <summary>
        ///     Load active widgets
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
        IList<IWidgetPlugin> LoadActiveWidgets();

        /// <summary>
        ///     Load active widgets
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
        IList<IWidgetPlugin> LoadActiveWidgetsByWidgetZone(string widgetZone);

        /// <summary>
        ///     Load widget by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found widget</returns>
        IWidgetPlugin LoadWidgetBySystemName(string systemName);

        /// <summary>
        ///     Load all widgets
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
        IList<IWidgetPlugin> LoadAllWidgets();

        IList<WidgetZone> LoaddAllWidgetZones();
        WidgetZone GetWidgetZoneBySystemName(string systemName);
        IList<WidgetZone> GetAllWidgetZones();
    }
}