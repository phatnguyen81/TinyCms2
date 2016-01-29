using System.Collections.Generic;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Web.Models.Posts
{
    public class CategorySimpleModel : BaseNopEntityModel
    {
        public CategorySimpleModel()
        {
            SubCategories = new List<CategorySimpleModel>();
        }

        public string Name { get; set; }
        public string SeName { get; set; }
        public int? NumberOfProducts { get; set; }
        public bool IncludeInTopMenu { get; set; }
        public List<CategorySimpleModel> SubCategories { get; set; }
    }
}