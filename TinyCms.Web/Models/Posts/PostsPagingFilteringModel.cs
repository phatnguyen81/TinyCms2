using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TinyCms.Web.Framework.Mvc;
using TinyCms.Web.Framework.UI.Paging;

namespace TinyCms.Web.Models.Posts
{
    /// <summary>
    /// Filtering and paging model for catalog
    /// </summary>
    public partial class PostsPagingFilteringModel : BasePageableModel
    {
        #region Ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public PostsPagingFilteringModel()
        {
            this.PageSizeOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

     
        /// <summary>
        /// Available page size options
        /// </summary>
        public IList<SelectListItem> PageSizeOptions { get; set; }


        
        #endregion

    }
}