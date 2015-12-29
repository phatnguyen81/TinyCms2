using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.AdsBanner.Domain;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Data;
using TinyCms.Core.Domain.Cms;
using TinyCms.Services.Events;

namespace Nop.Plugin.Widgets.AdsBanner.Services
{
    public class AdsBannerService : IAdsBannerService
    {

        private const string SEARCH_ALL_ADSBANNERS_MODEL_KEY = "Cms.pres.search.adsbanners";
        private const string SEARCH_ACTIVEFROMNOW_ADSBANNERS_MODEL_KEY = "Cms.pres.search.adsbanners-{0}";

        private readonly IRepository<AdsBannerRecord> _adsBannerRecordRepository;
        private readonly IRepository<WidgetZone> _widgetZoneRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        public AdsBannerService(IRepository<AdsBannerRecord> adsBannerRecordRepository, IRepository<WidgetZone> widgetZoneRepository, IEventPublisher eventPublisher, ICacheManager cacheManager)
        {
            _adsBannerRecordRepository = adsBannerRecordRepository;
            _widgetZoneRepository = widgetZoneRepository;
            _eventPublisher = eventPublisher;
            _cacheManager = cacheManager;
        }

        public IPagedList<AdsBannerRecord> GetAllAdsBanners(string name = "", int? widgetZoneId = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {

            var query = _adsBannerRecordRepository.Table;
            if (!showHidden)
            {
                var now = DateTime.UtcNow;
                query =
                    query.Where(
                        c =>
                            c.Published && (c.FromDateUtc == null || c.FromDateUtc <= now) &&
                            (c.ToDateUtc == null || c.ToDateUtc >= now));
            }
            if (!String.IsNullOrWhiteSpace(name))
                query = query.Where(c => c.Name.Contains(name));
            if (widgetZoneId != null)
            {
                query = query.Where(q => q.WidgetZoneId == widgetZoneId.Value);

            }
           
            var results = query.OrderBy(q=>q.DisplayOrder).ToList();

            //paging
            return new PagedList<AdsBannerRecord>(results, pageIndex, pageSize);
        }

        public IPagedList<AdsBannerRecord> GetAllAdsBannersActiveFromNow(string name = "", int? widgetZoneId = null, int pageIndex = 0,
            int pageSize = Int32.MaxValue)
        {
            var cacheKey = string.Format(SEARCH_ACTIVEFROMNOW_ADSBANNERS_MODEL_KEY, widgetZoneId);

            return _cacheManager.Get<IPagedList<AdsBannerRecord>>(cacheKey, () =>
            {

                var query = _adsBannerRecordRepository.Table;
                var now = DateTime.UtcNow;
                query =
                    query.Where(
                        c =>
                            c.Published && (c.ToDateUtc == null || c.ToDateUtc >= now));
                if (!String.IsNullOrWhiteSpace(name))
                    query = query.Where(c => c.Name.Contains(name));
                if (widgetZoneId != null)
                {
                    query = query.Where(q => q.WidgetZoneId == widgetZoneId.Value);

                }

                var results = query.OrderBy(q => q.DisplayOrder).ToList();

                //paging
                return new PagedList<AdsBannerRecord>(results, pageIndex, pageSize);
            });


        }

        public AdsBannerRecord GetById(int id)
        {
            return _adsBannerRecordRepository.GetById(id);
        }

        public void InsertAdsBanner(AdsBannerRecord adsBannerRecord)
        {
            _adsBannerRecordRepository.Insert(adsBannerRecord);
            _cacheManager.RemoveByPattern(SEARCH_ALL_ADSBANNERS_MODEL_KEY);
        }

        public void UpdateAdsBanner(AdsBannerRecord adsBannerRecord)
        {
            if (adsBannerRecord == null)
                throw new ArgumentNullException("adsBannerRecord");

            _adsBannerRecordRepository.Update(adsBannerRecord);
            _cacheManager.RemoveByPattern(SEARCH_ALL_ADSBANNERS_MODEL_KEY);
        }

        public void DeleteAdsBanner(AdsBannerRecord adsBannerRecord)
        {
            if (adsBannerRecord == null)
                throw new ArgumentNullException("adsBannerRecord");

            _adsBannerRecordRepository.Delete(adsBannerRecord);
            _cacheManager.RemoveByPattern(SEARCH_ALL_ADSBANNERS_MODEL_KEY);
        }
    }
}
