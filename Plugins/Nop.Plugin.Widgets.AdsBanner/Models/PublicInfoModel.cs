using System.Collections.Generic;
using TinyCms.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.AdsBanner.Models
{
    public class PublicInfoModel : BaseNopModel
    {
        public List<ShowAdsBannerModel> AdsBanners { get; set; }

    }

    public class ShowAdsBannerModel : BaseNopModel
    {
        public string PictureUrl { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }

    }
}