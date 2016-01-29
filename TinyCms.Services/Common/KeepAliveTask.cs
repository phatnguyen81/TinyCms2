using System.Net;
using TinyCms.Core.Domain;
using TinyCms.Core.Infrastructure;
using TinyCms.Services.Tasks;

namespace TinyCms.Services.Common
{
    /// <summary>
    ///     Represents a task for keeping the site alive
    /// </summary>
    public class KeepAliveTask : ITask
    {
        /// <summary>
        ///     Executes a task
        /// </summary>
        public void Execute()
        {
            var storeSettings = EngineContext.Current.Resolve<StoreInformationSettings>();
            var url = storeSettings.Url + "keepalive/index";
            using (var wc = new WebClient())
            {
                wc.DownloadString(url);
            }
        }
    }
}