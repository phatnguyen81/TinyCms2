using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCms.Core;
using TinyCms.Core.Data;
using TinyCms.Core.Domain.Posts;

namespace TinyCms.Services.Posts
{
    public class CategoryTypeService : ICategoryTypeService
    {
        private readonly IRepository<CategoryType> _categoryTypeRepository;

        public CategoryTypeService(IRepository<CategoryType> categoryTypeRepository)
        {
            _categoryTypeRepository = categoryTypeRepository;
        }

        public void DeleteCategoryType(CategoryType categoryType)
        {
            _categoryTypeRepository.Delete(categoryType);
        }

        public IList<CategoryType> GetAllCategoryTypes()
        {
            return _categoryTypeRepository.Table.OrderBy(q => q.DisplayOrder).ToList();
        }

        public CategoryType GetCategoryTypeById(int categoryTypeId)
        {
            return _categoryTypeRepository.GetById(categoryTypeId);
        }

        public CategoryType GetCategoryTypeBySystemName(string systemName)
        {
            return _categoryTypeRepository.Table.FirstOrDefault(q => q.SystemName == systemName);
        }

        public void InsertCategoryType(CategoryType categoryType)
        {
            _categoryTypeRepository.Insert(categoryType);
        }

        public void UpdateCategoryType(CategoryType categoryType)
        {
            _categoryTypeRepository.Update(categoryType);
        }
    }
}
