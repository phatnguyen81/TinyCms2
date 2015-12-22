using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Domain.Catalog;
using TinyCms.Core.Domain.Media;
using TinyCms.Core.Domain.Posts;
using TinyCms.Services.Customers;
using TinyCms.Services.Localization;
using TinyCms.Services.Media;
using TinyCms.Services.Posts;
using TinyCms.Services.Security;
using TinyCms.Services.Seo;
using TinyCms.Services.Topics;
using TinyCms.Web.Infrastructure.Cache;
using TinyCms.Web.Models.Media;
using TinyCms.Web.Models.Posts;
using TinyCms.Web.Extensions;

namespace TinyCms.Web.Controllers
{
    public class PostsController : BasePublicController
    {
        private readonly ICategoryService _categoryService;
        private readonly IPostService _postService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly ITopicService _topicService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPostTagService _postTagService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly CatalogSettings _catalogSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly ICategoryTypeService _categoryTypeService;

        public PostsController(ICategoryService categoryService, 
            IPostService postService, 
            IPictureService pictureService, 
            IWorkContext workContext, 
            ICacheManager cacheManager, 
            ITopicService topicService, 
            IUrlRecordService urlRecordService, 
            IPostTagService postTagService, 
            IPermissionService permissionService, 
            ILocalizationService localizationService,
            IWebHelper webHelper, 
            CatalogSettings catalogSettings,
            MediaSettings mediaSettings, 
            ICategoryTypeService categoryTypeService)
        {
            _categoryService = categoryService;
            _postService = postService;
            _pictureService = pictureService;
            _workContext = workContext;
            _cacheManager = cacheManager;
            _topicService = topicService;
            _urlRecordService = urlRecordService;
            _postTagService = postTagService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _catalogSettings = catalogSettings;
            _mediaSettings = mediaSettings;
            _categoryTypeService = categoryTypeService;
        }

        #region Methods
        [NonAction]
        protected virtual IEnumerable<PostOverviewModel> PreparePostOverviewModels(IEnumerable<Post> posts,
            bool preparePictureModel = true,
            int? postThumbPictureSize = null)
        {
            return this.PreparePostOverviewModels(_workContext,
                _categoryService, 
                _postService, 
                _permissionService,
                _localizationService, 
                _pictureService, 
                _webHelper, 
                _cacheManager,
                _catalogSettings, 
                _mediaSettings, 
                posts,
                preparePictureModel,
                postThumbPictureSize);
        }

        #endregion

        public ActionResult HomePageHotNews(int? postThumbPictureSize)
        {
            //var posts = _postService.GetAllPostsDisplayedOnHomePage();
            var categoryType = _categoryTypeService.GetCategoryTypeBySystemName("HotNews");
            var model = new List<PostOverviewModel>();
            if (categoryType != null)
            {
                var posts =
                    _postService.SearchPosts(
                        categoryIds:
                            _categoryService.GetCategoryByCategoryTypeSystemName("HotNews").Select(q => q.Id).ToArray()).ToList();
                model = PreparePostOverviewModels(posts, true, postThumbPictureSize).ToList();
            }
            return PartialView(model);
        }
        public ActionResult HomePagePosts(int? postThumbPictureSize)
        {
           var posts = _postService.GetAllPostsDisplayedOnHomePage();
            var model = PreparePostOverviewModels(posts, true, postThumbPictureSize);
            return PartialView(model);
        }
        public ActionResult HomePagePicturesAndVideos()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult HomePageCategories()
        {
            string categoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HOMEPAGE_KEY,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _workContext.WorkingLanguage.Id,
                _webHelper.IsCurrentConnectionSecured());

            var model = _cacheManager.Get(categoriesCacheKey, () =>
                _categoryService.GetAllCategoriesDisplayedOnHomePage()
                .Select(x =>
                {
                    var catModel = x.ToModel();

                    //prepare picture model
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured());
                    catModel.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(x.PictureId);
                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), catModel.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), catModel.Name)
                        };
                        return pictureModel;
                    });

                    //prepare post model
                    catModel.Posts = PreparePostOverviewModels(_postService.SearchPosts(pageSize: 3, orderBy: PostSortingEnum.CreatedOn,categoryIds:new []{catModel.Id}).ToList()).ToList();

                    return catModel;
                })
                .ToList()
            );

            if (model.Count == 0)
                return Content("");

            return PartialView(model);
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
        [NonAction]
        protected virtual string[] ParsePostTags(string postTags)
        {
            var result = new List<string>();
            if (!String.IsNullOrWhiteSpace(postTags))
            {
                string[] values = postTags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string val1 in values)
                    if (!String.IsNullOrEmpty(val1.Trim()))
                        result.Add(val1.Trim());
            }
            return result.ToArray();
        }

        [NonAction]
        protected virtual void SavePostTags(Post post, string[] postTags)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            //post tags
            var existingPostTags = post.PostTags.ToList();
            var postTagsToRemove = new List<PostTag>();
            foreach (var existingPostTag in existingPostTags)
            {
                bool found = false;
                foreach (string newPostTag in postTags)
                {
                    if (existingPostTag.Name.Equals(newPostTag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    postTagsToRemove.Add(existingPostTag);
                }
            }
            foreach (var postTag in postTagsToRemove)
            {
                post.PostTags.Remove(postTag);
                _postService.UpdatePost(post);
            }
            foreach (string postTagName in postTags)
            {
                PostTag postTag;
                var postTag2 = _postTagService.GetPostTagByName(postTagName);
                if (postTag2 == null)
                {
                    //add new post tag
                    postTag = new PostTag
                    {
                        Name = postTagName
                    };
                    _postTagService.InsertPostTag(postTag);
                }
                else
                {
                    postTag = postTag2;
                }
                if (!post.PostTagExists(postTag.Id))
                {
                    post.PostTags.Add(postTag);
                    _postService.UpdatePost(post);
                }
            }
        }

        [NonAction]
        protected virtual void SavePostPictures(Post post, IEnumerable<int> pictureIds)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            //post pictures

            foreach (int pictureId in pictureIds)
            {
                var postPicture = new PostPicture
                {
                    PictureId = pictureId,
                    PostId = post.Id
                };
                _postService.InsertPostPicture(postPicture);
            }
            
        }

        [NonAction]
        protected virtual void UpdatePictureSeoNames(Post post)
        {
            foreach (var pp in post.PostPictures)
                _pictureService.SetSeoFilename(pp.PictureId, _pictureService.GetPictureSeName(post.Name));
        }

        public ActionResult Write()
        {
            var model = new NewPostModel();

            PrepareNewPostModel(model);

            return View(model);
        }

        [HttpPost]
        public JsonResult Write(NewPostModel model)
        {
            var post = new Post
            {
                Name = model.Title,
                FullDescription = model.Body,
                UpdatedOnUtc = DateTime.UtcNow,
                CreatedOnUtc = DateTime.UtcNow,
                CreatedBy = _workContext.CurrentCustomer.Id,
                Published = false
            };
            _postService.InsertPost(post);

            var seName = post.ValidateSeName(string.Empty, post.Name, true);
            _urlRecordService.SaveSlug(post, seName, 0);

            //tags
            SavePostTags(post, ParsePostTags(model.PostTags));

            SavePostPictures(post,
                model.PictureIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse));

            UpdatePictureSeoNames(post);

            var postCategory = new PostCategory
            {
                PostId = post.Id,
                CategoryId = model.CategoryId
            };
            _categoryService.InsertPostCategory(postCategory);

            return Json(new
            {
                success = true
            });
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