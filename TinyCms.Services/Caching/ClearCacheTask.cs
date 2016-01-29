using TinyCms.Core.Caching;
using TinyCms.Core.Infrastructure;
using TinyCms.Services.Tasks;

namespace TinyCms.Services.Caching
{
    /// <summary>
    ///     Clear cache schedueled task implementation
    /// </summary>
    public class ClearCacheTask : ITask
    {
        /// <summary>
        ///     Executes a task
        /// </summary>
        public void Execute()
        {
            var cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
            cacheManager.Clear();
        }
    }
}