using System;
using System.Collections.Generic;
using TinyCms.Core.Domain.Localization;
using TinyCms.Core.Domain.Security;
using TinyCms.Core.Domain.Seo;

namespace TinyCms.Core.Domain.Posts
{
    /// <summary>
    /// Represents a category
    /// </summary>
    public partial class CategoryType : BaseEntity
    {

        public string SystemName { get; set; }
        
        public string Name { get; set; }

        public int DisplayOrder { get; set; }

    }
}
