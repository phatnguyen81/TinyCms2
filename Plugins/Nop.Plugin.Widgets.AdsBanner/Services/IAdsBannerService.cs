using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.AdsBanner.Domain;
using TinyCms.Core;

namespace Nop.Plugin.Widgets.AdsBanner.Services
{
    public interface IAdsBannerService
    {
        IPagedList<AdsBannerRecord> GetAllAdsBanners(string name = "", int? widgetZoneId = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);


        IPagedList<AdsBannerRecord> GetAllAdsBannersActiveFromNow(string name = "", int? widgetZoneId = null,
            int pageIndex = 0, int pageSize = int.MaxValue);


        AdsBannerRecord GetById(int id);

        void InsertAdsBanner(AdsBannerRecord adsBannerRecord);

        void UpdateAdsBanner(AdsBannerRecord adsBannerRecord);

        void DeleteAdsBanner(AdsBannerRecord adsBannerRecord);
    }
}
