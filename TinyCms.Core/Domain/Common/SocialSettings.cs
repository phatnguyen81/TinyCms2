using TinyCms.Core.Configuration;

namespace TinyCms.Core.Domain.Common
{
    public class SocialSettings : ISettings
    {
        public string FacebookAppId { get; set; }
        public string FacebookAppSecret { get; set; }
    }
}