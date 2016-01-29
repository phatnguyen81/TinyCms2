using System;
using System.Collections.Generic;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Web.Models.Customer
{
    public class CustomerDownloadablePostsModel : BaseNopModel
    {
        public CustomerDownloadablePostsModel()
        {
            Items = new List<DownloadablePostsModel>();
        }

        public IList<DownloadablePostsModel> Items { get; set; }

        #region Nested classes

        public class DownloadablePostsModel : BaseNopModel
        {
            public Guid OrderItemGuid { get; set; }
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string PostseName { get; set; }
            public string ProductAttributes { get; set; }
            public int DownloadId { get; set; }
            public int LicenseId { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        #endregion
    }

    public class UserAgreementModel : BaseNopModel
    {
        public Guid OrderItemGuid { get; set; }
        public string UserAgreementText { get; set; }
    }
}