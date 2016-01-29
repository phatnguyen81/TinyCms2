using System;
using TinyCms.Core;
using TinyCms.Core.Domain.Media;

namespace Nop.Plugin.Widgets.AdsBanner.Domain
{
    public class AdsBannerRecord : BaseEntity
    {
        public string Name { get; set; }
        public int PictureId { get; set; }
        public int WidgetZoneId { get; set; }
        public string Url { get; set; }
        public DateTime? FromDateUtc { get; set; }
        public DateTime? ToDateUtc { get; set; }
        public Picture Picture { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
    }
}