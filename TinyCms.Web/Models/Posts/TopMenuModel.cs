using System.Collections.Generic;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Web.Models.Posts
{
    public partial class TopMenuModel : BaseNopModel
    {
        public TopMenuModel()
        {
            Categories = new List<CategorySimpleModel>();
            Topics = new List<TopMenuTopicModel>();
        }

        public IList<CategorySimpleModel> Categories { get; set; }
        public IList<TopMenuTopicModel> Topics { get; set; }


        #region Nested classes

        public class TopMenuTopicModel : BaseNopEntityModel
        {
            public string Name { get; set; }
            public string SeName { get; set; }
        }

        #endregion
    }
}