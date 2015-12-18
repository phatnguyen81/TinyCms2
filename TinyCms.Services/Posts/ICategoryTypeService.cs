using System.Collections.Generic;
using TinyCms.Core;
using TinyCms.Core.Domain.Posts;

namespace TinyCms.Services.Posts
{
    /// <summary>
    /// CategoryType service interface
    /// </summary>
    public partial interface ICategoryTypeService
    {
        void DeleteCategoryType(CategoryType categoryType);

        IList<CategoryType> GetAllCategoryTypes();

        CategoryType GetCategoryTypeById(int categoryTypeId);

        CategoryType GetCategoryTypeBySystemName(string systemName);

        void InsertCategoryType(CategoryType categoryType);

        void UpdateCategoryType(CategoryType categoryType);
        

    }
}
