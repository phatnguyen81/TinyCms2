using System;
using System.Collections.Generic;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Common
{
    public class SystemInfoModel : BaseNopModel
    {
        public SystemInfoModel()
        {
            ServerVariables = new List<ServerVariableModel>();
            LoadedAssemblies = new List<LoadedAssembly>();
        }

        [NopResourceDisplayName("Admin.System.SystemInfo.ASPNETInfo")]
        public string AspNetInfo { get; set; }

        [NopResourceDisplayName("Admin.System.SystemInfo.IsFullTrust")]
        public string IsFullTrust { get; set; }

        [NopResourceDisplayName("Admin.System.SystemInfo.NopVersion")]
        public string NopVersion { get; set; }

        [NopResourceDisplayName("Admin.System.SystemInfo.OperatingSystem")]
        public string OperatingSystem { get; set; }

        [NopResourceDisplayName("Admin.System.SystemInfo.ServerLocalTime")]
        public DateTime ServerLocalTime { get; set; }

        [NopResourceDisplayName("Admin.System.SystemInfo.ServerTimeZone")]
        public string ServerTimeZone { get; set; }

        [NopResourceDisplayName("Admin.System.SystemInfo.UTCTime")]
        public DateTime UtcTime { get; set; }

        [NopResourceDisplayName("Admin.System.SystemInfo.HTTPHOST")]
        public string HttpHost { get; set; }

        [NopResourceDisplayName("Admin.System.SystemInfo.ServerVariables")]
        public IList<ServerVariableModel> ServerVariables { get; set; }

        [NopResourceDisplayName("Admin.System.SystemInfo.LoadedAssemblies")]
        public IList<LoadedAssembly> LoadedAssemblies { get; set; }

        public class ServerVariableModel : BaseNopModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class LoadedAssembly : BaseNopModel
        {
            public string FullName { get; set; }
            public string Location { get; set; }
        }
    }
}