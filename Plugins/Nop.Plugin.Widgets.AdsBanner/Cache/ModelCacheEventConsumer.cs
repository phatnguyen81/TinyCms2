using Nop.Plugin.Widgets.AdsBanner.Domain;
using TinyCms.Core.Caching;
using TinyCms.Core.Events;
using TinyCms.Services.Events;

namespace Nop.Plugin.Widgets.AdsBanner.Cache
{
    public class ModelCacheEventConsumer :
        IConsumer<EntityInserted<AdsBannerRecord>>,
        IConsumer<EntityUpdated<AdsBannerRecord>>,
        IConsumer<EntityDeleted<AdsBannerRecord>>
    {
        public const string SEARCH_ALL_ADSBANNERS_MODEL_KEY = "Cms.pres.search.adsbanners";
        public const string SEARCH_ACTIVEFROMNOW_ADSBANNERS_MODEL_KEY = "Cms.pres.search.adsbanners-{0}-{0}-{0}-{0}";
        private readonly ICacheManager _cacheManager;

        public ModelCacheEventConsumer(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityDeleted<AdsBannerRecord> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_ALL_ADSBANNERS_MODEL_KEY);
        }

        public void HandleEvent(EntityInserted<AdsBannerRecord> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_ALL_ADSBANNERS_MODEL_KEY);
        }

        public void HandleEvent(EntityUpdated<AdsBannerRecord> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_ALL_ADSBANNERS_MODEL_KEY);
        }
    }
}