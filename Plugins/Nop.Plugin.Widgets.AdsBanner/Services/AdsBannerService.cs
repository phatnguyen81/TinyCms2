using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.AdsBanner.Domain;
using TinyCms.Core;
using TinyCms.Core.Data;

namespace Nop.Plugin.Widgets.AdsBanner.Services
{
    public class AdsBannerService : IAdsBannerService
    {
        private readonly IRepository<AdsBannerRecord> _adsBannerRecordRepository;

        public AdsBannerService(IRepository<AdsBannerRecord> adsBannerRecordRepository)
        {
            _adsBannerRecordRepository = adsBannerRecordRepository;
        }

        public IPagedList<AdsBannerRecord> GetAllAdsBanners(string name = "", DateTime? fromDateTimeUtc = null, DateTime? toDateTimeUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _adsBannerRecordRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!String.IsNullOrWhiteSpace(name))
                query = query.Where(c => c.Name.Contains(name));

            if (fromDateTimeUtc != null)
                query = query.Where(q => q.FromDateUtc >= fromDateTimeUtc);
            if (toDateTimeUtc != null)
                query = query.Where(q => q.ToDateUtc <= toDateTimeUtc);

            var results = query.ToList();

            //paging
            return new PagedList<AdsBannerRecord>(results, pageIndex, pageSize);
        }

        public AdsBannerRecord GetById(int id)
        {
            return _adsBannerRecordRepository.GetById(id);
        }

        public void InsertAdsBanner(AdsBannerRecord adsBannerRecord)
        {
            _adsBannerRecordRepository.Insert(adsBannerRecord);
        }

        public void UpdateAdsBanner(AdsBannerRecord adsBannerRecord)
        {
            _adsBannerRecordRepository.Update(adsBannerRecord);
        }

        public void DeleteAdsBanner(AdsBannerRecord adsBannerRecord)
        {
            _adsBannerRecordRepository.Delete(adsBannerRecord);
        }
    }
}
