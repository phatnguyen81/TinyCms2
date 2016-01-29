using System.Collections.Generic;
using System.Web.Mvc;
using TinyCms.Web.Framework.UI.Paging;

namespace TinyCms.Web.Models.Posts
{
    /// <summary>
    ///     Filtering and paging model for catalog
    /// </summary>
    public class PostsPagingFilteringModel : BasePageableModel
    {
        #region Ctor

        /// <summary>
        ///     Constructor
        /// </summary>
        public PostsPagingFilteringModel()
        {
            PageSizeOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Available page size options
        /// </summary>
        public IList<SelectListItem> PageSizeOptions { get; set; }

        #endregion
    }
}