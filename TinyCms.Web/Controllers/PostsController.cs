using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Domain.Posts;
using TinyCms.Services.Customers;
using TinyCms.Services.Localization;
using TinyCms.Services.Media;
using TinyCms.Services.Posts;
using TinyCms.Services.Seo;
using TinyCms.Services.Topics;
using TinyCms.Web.Infrastructure.Cache;
using TinyCms.Web.Models.Posts;

namespace TinyCms.Web.Controllers
{
    public class PostsController : BasePublicController
    {
        private readonly ICategoryService _categoryService;
        private readonly IPostService _postService;
        private readonly IPictureService _picture;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly ITopicService _topicService;

        public PostsController(ICategoryService categoryService, 
            IPostService postService, 
            IPictureService picture, IWorkContext workContext, ICacheManager cacheManager, ITopicService topicService)
        {
            _categoryService = categoryService;
            _postService = postService;
            _picture = picture;
            _workContext = workContext;
            _cacheManager = cacheManager;
            _topicService = topicService;
        }

        public ActionResult HomePageHeadLine()
        {
            return PartialView();
        }

        public ActionResult HomePagePicturesAndVideos()
        {
            return PartialView();
        }

        public ActionResult HomePageCategories()
        {
            return PartialView();
        }

        public ActionResult DiscoveryCategory()
        {
            return PartialView();
        }
        public ActionResult TravelViaPictureCategory()
        {
            return PartialView();
        }

        public void PrepareNewPostModel(NewPostModel model)
        {
            model.AvailableCategories = _categoryService.GetAllCategories().Select(q=> new SelectListItem
            {
                Text = q.Name,
                Value = q.Id.ToString()
            }).ToList();
        }
        public ActionResult Write()
        {
            var model = new NewPostModel();

            PrepareNewPostModel(model);

            return View(model);
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <param name="allCategories">All available categories; pass null to load them internally</param>
        /// <returns>Category models</returns>
        [NonAction]
        protected virtual IList<CategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId,
            bool loadSubCategories = true, IList<Category> allCategories = null)
        {
            var result = new List<CategorySimpleModel>();

            //little hack for performance optimization.
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once
            //if you don't like this implementation if you can uncomment the line below (old behavior) and comment several next lines (before foreach)
            //var categories = _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId);
            if (allCategories == null)
            {
                //load categories if null passed
                //we implemeneted it this way for performance optimization - recursive iterations (below)
                //this way all categories are loaded only once
                allCategories = _categoryService.GetAllCategories();
            }
            var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();
            foreach (var category in categories)
            {
                var categoryModel = new CategorySimpleModel
                {
                    Id = category.Id,
                    Name = category.GetLocalized(x => x.Name),
                    SeName = category.GetSeName(),
                    IncludeInTopMenu = category.IncludeInTopMenu
                };

           
                if (loadSubCategories)
                {
                    var subCategories = PrepareCategorySimpleModels(category.Id, loadSubCategories, allCategories);
                    categoryModel.SubCategories.AddRange(subCategories);
                }
                result.Add(categoryModel);
            }

            return result;
        }

        [ChildActionOnly]
        public ActionResult SearchBox()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult TopMenu()
        {
            //categories
            string categoryCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_MENU_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedCategoriesModel = _cacheManager.Get(categoryCacheKey, () => PrepareCategorySimpleModels(0));

            //top menu topics
            string topicCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_TOP_MENU_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedTopicModel = _cacheManager.Get(topicCacheKey, () =>
                _topicService.GetAllTopics()
                .Where(t => t.IncludeInTopMenu)
                .Select(t => new TopMenuModel.TopMenuTopicModel
                {
                    Id = t.Id,
                    Name = t.GetLocalized(x => x.Title),
                    SeName = t.GetSeName()
                })
                .ToList()
            );
            var model = new TopMenuModel
            {
                Categories = cachedCategoriesModel,
                Topics = cachedTopicModel,
            };
            return PartialView(model);
        }
    }
}