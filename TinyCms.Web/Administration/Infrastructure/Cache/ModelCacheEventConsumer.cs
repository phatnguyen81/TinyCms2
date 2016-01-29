using TinyCms.Core.Caching;
using TinyCms.Core.Domain.Configuration;
using TinyCms.Core.Events;
using TinyCms.Core.Infrastructure;
using TinyCms.Services.Events;

namespace TinyCms.Admin.Infrastructure.Cache
{
    /// <summary>
    ///     Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public class ModelCacheEventConsumer :
        //settings
        IConsumer<EntityUpdated<Setting>>
        //specification attributes
    {
        /// <summary>
        ///     Key for nopCommerce.com news cache
        /// </summary>
        public const string OFFICIAL_NEWS_MODEL_KEY = "Nop.pres.admin.official.news";

        public const string OFFICIAL_NEWS_PATTERN_KEY = "Nop.pres.admin.official.news";
        private readonly ICacheManager _cacheManager;

        public ModelCacheEventConsumer()
        {
            //TODO inject static cache manager using constructor
            _cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        }

        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            //clear models which depend on settings
            _cacheManager.RemoveByPattern(OFFICIAL_NEWS_PATTERN_KEY);
                //depends on CommonSettings.HideAdvertisementsOnAdminArea
        }
    }
}