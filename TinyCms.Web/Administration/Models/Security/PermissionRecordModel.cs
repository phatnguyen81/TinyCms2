using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Security
{
    public class PermissionRecordModel : BaseNopModel
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
    }
}