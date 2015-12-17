using System;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Logging;

namespace TinyCms.Core.Domain.Cms
{
    /// <summary>
    /// Represents a log record
    /// </summary>
    public partial class WidgetZone : BaseEntity
    {
        public string SystemName { get; set; }
        public string Name { get; set; }
    }
}
