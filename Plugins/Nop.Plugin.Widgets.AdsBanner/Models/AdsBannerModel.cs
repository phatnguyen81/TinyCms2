using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.AdsBanner.Models
{
    public class AdsBannerModel : BaseNopEntityModel
    {
        public AdsBannerModel()
        {
            AvailableWidgetZones = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Widgets.AdsBanner.Name")]
        public string Name { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Plugins.Widgets.AdsBanner.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AdsBanner.WidgetZone")]
        public int WidgetZoneId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AdsBanner.Url")]
        public string Url { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AdsBanner.FromDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? FromDate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AdsBanner.ToDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? ToDate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AdsBanner.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AdsBanner.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public List<SelectListItem> AvailableWidgetZones { get; set; }
    }

    public class AdsBannerListModel
    {
        [NopResourceDisplayName("Plugins.Widgets.AdsBanner.List.SearchAdsBannerName")]
        public string SearchAdsBannerName { get; set; }
    }
}
