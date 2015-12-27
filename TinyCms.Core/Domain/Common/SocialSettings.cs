using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCms.Core.Configuration;

namespace TinyCms.Core.Domain.Common
{
    public class SocialSettings : ISettings
    {
        public string FacebookAppId { get; set; }
        public string FacebookAppSecret { get; set; }
    }
}
