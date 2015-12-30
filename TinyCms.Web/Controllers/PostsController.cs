using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Newtonsoft.Json;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Domain.Catalog;
using TinyCms.Core.Domain.Common;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Media;
using TinyCms.Core.Domain.Posts;
using TinyCms.Core.Domain.Seo;
using TinyCms.Services.Common;
using TinyCms.Services.Customers;
using TinyCms.Services.Events;
using TinyCms.Services.Helpers;
using TinyCms.Services.Localization;
using TinyCms.Services.Logging;
using TinyCms.Services.Media;
using TinyCms.Services.Posts;
using TinyCms.Services.Security;
using TinyCms.Services.Seo;
using TinyCms.Services.Topics;
using TinyCms.Web.Framework.Security;
using TinyCms.Web.Infrastructure.Cache;
using TinyCms.Web.Models.Media;
using TinyCms.Web.Models.Posts;
using TinyCms.Web.Extensions;
using TinyCms.Web.Framework.Events;

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
        private readonly IAclService _aclService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IPostTemplateService _postTemplateService;
        private readonly SeoSettings _seoSettings;
        private readonly ISearchTermService _searchTermService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDateTimeHelper _dateTimeHelper;
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
            ICategoryTypeService categoryTypeService, 
            IAclService aclService, 
            ICategoryTemplateService categoryTemplateService,
            ICustomerActivityService customerActivityService, 
            SeoSettings seoSettings, 
            IPostTemplateService postTemplateService, 
            ISearchTermService searchTermService,
            IEventPublisher eventPublisher, 
            IDateTimeHelper dateTimeHelper)
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
            _aclService = aclService;
            _categoryTemplateService = categoryTemplateService;
            _customerActivityService = customerActivityService;
            _seoSettings = seoSettings;
            _postTemplateService = postTemplateService;
            _searchTermService = searchTermService;
            _eventPublisher = eventPublisher;
            _dateTimeHelper = dateTimeHelper;
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
                _dateTimeHelper,
                _catalogSettings, 
                _mediaSettings, 
                posts,
                preparePictureModel,
                postThumbPictureSize);
        }



        public void PrepareNewPostModel(NewPostModel model)
        {
            var allCategories = _categoryService.GetAllCategories();
            foreach (var category in allCategories)
            {
                model.AvailableCategories.Add(new SelectListItem
                {
                    Text = category.GetFormattedBreadCrumb(allCategories),
                    Value = category.Id.ToString()
                });
            }

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

        #endregion



        #region Searching

        [NopHttpsRequirement(SslRequirement.No)]
        [ValidateInput(false)]
        public ActionResult Search(SearchModel model, PostsPagingFilteringModel command)
        {
          
            if (model == null)
                model = new SearchModel();

            var searchTerms = model.q;
            if (searchTerms == null)
                searchTerms = "";
            searchTerms = searchTerms.Trim();



            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                10);


            string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var categories = _cacheManager.Get(cacheKey, () =>
            {
                var categoriesModel = new List<SearchModel.CategoryModel>();
                //all categories
                var allCategories = _categoryService.GetAllCategories();
                foreach (var c in allCategories)
                {
                    //generate full category name (breadcrumb)
                    string categoryBreadcrumb = "";
                    var breadcrumb = c.GetCategoryBreadCrumb(allCategories, _aclService);
                    for (int i = 0; i <= breadcrumb.Count - 1; i++)
                    {
                        categoryBreadcrumb += breadcrumb[i].GetLocalized(x => x.Name);
                        if (i != breadcrumb.Count - 1)
                            categoryBreadcrumb += " >> ";
                    }
                    categoriesModel.Add(new SearchModel.CategoryModel
                    {
                        Id = c.Id,
                        Breadcrumb = categoryBreadcrumb
                    });
                }
                return categoriesModel;
            });
          
            IPagedList<Post> posts = new PagedList<Post>(new List<Post>(), 0, 1);
            // only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
            if (Request.Params["q"] != null)
            {
                if (searchTerms.Length < _catalogSettings.PostSearchTermMinimumLength)
                {
                    model.Warning = string.Format(_localizationService.GetResource("Search.SearchTermMinimumLengthIsNCharacters"), _catalogSettings.PostSearchTermMinimumLength);
                }
                else
                {

                    //posts
                    posts = _postService.SearchPosts(
                        keywords: searchTerms,
                        searchDescriptions: true,
                        searchPostTags: true,
                        languageId: _workContext.WorkingLanguage.Id,
                        pageIndex: command.PageNumber - 1,
                        pageSize: command.PageSize);
                    model.Posts = PreparePostOverviewModels(posts).ToList();

                    model.NoResults = !model.Posts.Any();

                    //search term statistics
                    if (!String.IsNullOrEmpty(searchTerms))
                    {
                        var searchTerm = _searchTermService.GetSearchTermByKeyword(searchTerms);
                        if (searchTerm != null)
                        {
                            searchTerm.Count++;
                            _searchTermService.UpdateSearchTerm(searchTerm);
                        }
                        else
                        {
                            searchTerm = new SearchTerm
                            {
                                Keyword = searchTerms,
                                Count = 1
                            };
                            _searchTermService.InsertSearchTerm(searchTerm);
                        }
                    }

                    //event
                    _eventPublisher.Publish(new PostSearchEvent
                    {
                        SearchTerm = searchTerms,
                        SearchInDescriptions = true,
                        WorkingLanguageId = _workContext.WorkingLanguage.Id
                    });
                }
            }

            model.PagingFilteringContext.LoadPagedList(posts);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult SearchBox()
        {
            return PartialView();
        }
        #endregion





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

        [ChildActionOnly]
        public ActionResult BottomMenu()
        {
            //categories
            string categoryCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_MENU_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedCategoriesModel = _cacheManager.Get(categoryCacheKey, () => PrepareCategorySimpleModels(0,loadSubCategories:true));

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

        #region Categories

        [NonAction]
        protected virtual void PreparePageSizeOptions(PostsPagingFilteringModel pagingFilteringModel, PostsPagingFilteringModel command,
            int fixedPageSize)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            if (command.PageNumber <= 0)
            {
                command.PageNumber = 1;
            }
            command.PageSize = fixedPageSize;

            //ensure pge size is specified
            if (command.PageSize <= 0)
            {
                command.PageSize = fixedPageSize;
            }
        }

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY,
                parentCategoryId,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            return _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id));
                }
                return categoriesIds;
            });
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
                    catModel.Posts = PreparePostOverviewModels(_postService.SearchPosts(pageSize: 4, orderBy: PostSortingEnum.CreatedOn, categoryIds: new[] { catModel.Id }).ToList()).ToList();

                    return catModel;
                })
                .ToList()
            );

            if (model.Count == 0)
                return Content("");

            return PartialView(model);
        }
        [ChildActionOnly]
        public ActionResult CategoryBox(int categoryId, int numberPost, string template, int? postThumbPictureSize)
        {

            var category = _categoryService.GetCategoryById(categoryId);

            if (category == null || string.IsNullOrWhiteSpace(template))
                return Content("");


            var model = new CategoryBoxModel { Id = category.Id, SeName = category.GetSeName() };

            var posts = _postService.SearchPosts(pageSize: numberPost, categoryIds: new[] { categoryId }).ToList();
            model.Posts = PreparePostOverviewModels(posts, postThumbPictureSize != null && postThumbPictureSize > 0, postThumbPictureSize).ToList();
            return PartialView(template, model);
        }


        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult Category(int categoryId, PostsPagingFilteringModel command)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted)
                return InvokeHttp404();

            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a category before publishing
            if (!category.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return InvokeHttp404();

            //ACL (access control list)
            if (!_aclService.Authorize(category))
                return InvokeHttp404();

            var model = category.ToModel();

            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command, 10);
            if (category.ParentCategoryId > 0)
            {
                model.ParentCategory = _categoryService.GetCategoryById(category.ParentCategoryId).ToModel();
                string subCategoriesOfParentCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_SUBCATEGORIES_KEY,
                    model.ParentCategory.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _workContext.WorkingLanguage.Id,
                    _webHelper.IsCurrentConnectionSecured());
                model.ParentCategory.SubCategories = _cacheManager.Get(subCategoriesOfParentCacheKey, () =>
                    _categoryService.GetAllCategoriesByParentCategoryId(category.ParentCategoryId)
                        .Select(x =>
                        {
                            var subCatModel = new CategoryModel.SubCategoryModel
                            {
                                Id = x.Id,
                                Name = x.GetLocalized(y => y.Name),
                                SeName = x.GetSeName(),
                                Description = x.GetLocalized(y => y.Description)
                            };

                            //prepare picture model
                            int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                            var categoryPictureCacheKey =
                                string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize,
                                    true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured());
                            subCatModel.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                            {
                                var picture = _pictureService.GetPictureById(x.PictureId);
                                var pictureModel = new PictureModel
                                {
                                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                                    Title =
                                        string.Format(
                                            _localizationService.GetResource("Media.Category.ImageLinkTitleFormat"),
                                            subCatModel.Name),
                                    AlternateText =
                                        string.Format(
                                            _localizationService.GetResource("Media.Category.ImageAlternateTextFormat"),
                                            subCatModel.Name)
                                };
                                return pictureModel;
                            });

                            return subCatModel;
                        })
                        .ToList()
                    );
            }

            //subcategories
            string subCategoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_SUBCATEGORIES_KEY,
                categoryId,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _workContext.WorkingLanguage.Id,
                _webHelper.IsCurrentConnectionSecured());
            model.SubCategories = _cacheManager.Get(subCategoriesCacheKey, () =>
                _categoryService.GetAllCategoriesByParentCategoryId(categoryId)
                .Select(x =>
                {
                    var subCatModel = new CategoryModel.SubCategoryModel
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                        SeName = x.GetSeName(),
                        Description = x.GetLocalized(y => y.Description)
                    };

                    //prepare picture model
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured());
                    subCatModel.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(x.PictureId);
                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), subCatModel.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), subCatModel.Name)
                        };
                        return pictureModel;
                    });

                    return subCatModel;
                })
                .ToList()
            );

            var categoryIds = new List<int>();
            categoryIds.Add(category.Id);
            if (_catalogSettings.ShowPostsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(category.Id));
            }
            //posts
            //IList<int> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_webHelper);
            IList<int> filterableSpecificationAttributeOptionIds;
            var posts = _postService.SearchPosts(
                categoryIds: categoryIds,
                featuredPosts: _catalogSettings.IncludeFeaturedPostsInNormalLists ? null : (bool?)false,
                orderBy: PostSortingEnum.CreatedOn,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Posts = PreparePostOverviewModels(posts).ToList();

            model.PagingFilteringContext.LoadPagedList(posts);

            //template
            var templateCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_TEMPLATE_MODEL_KEY, category.CategoryTemplateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _categoryTemplateService.GetCategoryTemplateById(category.CategoryTemplateId);
                if (template == null)
                    template = _categoryTemplateService.GetAllCategoryTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory", _localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name);

            return View(templateViewPath, model);
        }
        #endregion

        #region Post

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
                ShortDescription = model.Short,
                Published = false
            };
            _postService.InsertPost(post);

            var seName = post.ValidateSeName(string.Empty, post.Name, true);
            _urlRecordService.SaveSlug(post, seName, 0);

            //tags
            SavePostTags(post, ParsePostTags(model.PostTags));

            if (model.PictureIds != null && model.PictureIds.Any())
            {
                SavePostPictures(post,
                    model.PictureIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse));
            }
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
        protected virtual PostDetailsModel PreparePostDetailsPageModel(Post post)
        {
            if (post == null)
                throw new ArgumentNullException("post");

            #region Standard properties

            var model = new PostDetailsModel
            {
                Id = post.Id,
                Name = post.GetLocalized(x => x.Name),
                ShortDescription = post.GetLocalized(x => x.ShortDescription),
                FullDescription = post.GetLocalized(x => x.FullDescription),
                MetaKeywords = post.GetLocalized(x => x.MetaKeywords),
                MetaDescription = post.GetLocalized(x => x.MetaDescription),
                MetaTitle = post.GetLocalized(x => x.MetaTitle),
                SeName = post.GetSeName(),
                CreatedOn = _dateTimeHelper.ConvertToUserTime(post.CreatedOnUtc)
            };

            //automatically generate post description?
            if (_seoSettings.GeneratePostMetaDescription && String.IsNullOrEmpty(model.MetaDescription))
            {
                //based on short description
                model.MetaDescription = model.ShortDescription;
            }

            #endregion


            #region Breadcrumb

            //do not prepare this model for the associated posts. anyway it's not used
            if (_catalogSettings.CategoryBreadcrumbEnabled)
            {
                var breadcrumbCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_BREADCRUMB_MODEL_KEY,
                    post.Id,
                    _workContext.WorkingLanguage.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
                model.Breadcrumb = _cacheManager.Get(breadcrumbCacheKey, () =>
                {
                    var breadcrumbModel = new PostDetailsModel.PostBreadcrumbModel
                    {
                        Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                        PostId = post.Id,
                        PostName = post.GetLocalized(x => x.Name),
                        PostSeName = post.GetSeName()
                    };
                    var postCategories = _categoryService.GetPostCategoriesByPostId(post.Id);
                    if (postCategories.Count > 0)
                    {
                        var category = postCategories[0].Category;
                        if (category != null)
                        {
                            foreach (var catBr in category.GetCategoryBreadCrumb(_categoryService, _aclService))
                            {
                                breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                                {
                                    Id = catBr.Id,
                                    Name = catBr.GetLocalized(x => x.Name),
                                    SeName = catBr.GetSeName(),
                                    IncludeInTopMenu = catBr.IncludeInTopMenu
                                });
                            }
                        }
                    }
                    return breadcrumbModel;
                });
            }

            #endregion

            #region Post tags

            //do not prepare this model for the associated posts. anyway it's not used
              var postTagsCacheKey = string.Format(ModelCacheEventConsumer.PRODUCTTAG_BY_PRODUCT_MODEL_KEY, post.Id, _workContext.WorkingLanguage.Id);
                model.PostTags = _cacheManager.Get(postTagsCacheKey, () =>
                    post.PostTags
                        //filter by store
                    .Where(x => _postTagService.GetPostCount(x.Id) > 0)
                    .Select(x => new PostTagModel
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                        SeName = x.GetSeName(),
                        PostCount = _postTagService.GetPostCount(x.Id)
                    })
                    .ToList());

            #endregion

            #region Templates

            var templateCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_TEMPLATE_MODEL_KEY, post.PostTemplateId);
            model.PostTemplateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _postTemplateService.GetPostTemplateById(post.PostTemplateId);
                if (template == null)
                    template = _postTemplateService.GetAllPostTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });

            #endregion

            #region Pictures

            //default picture
            var defaultPictureSize = _mediaSettings.PostDetailsPictureSize;
            //prepare picture models
            var postPicturesCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DETAILS_PICTURES_MODEL_KEY, post.Id, defaultPictureSize, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured());
            var cachedPictures = _cacheManager.Get(postPicturesCacheKey, () =>
            {
                var pictures = _pictureService.GetPicturesByPostId(post.Id);
                var defaultPicture = pictures.FirstOrDefault();
                var defaultPictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(defaultPicture, defaultPictureSize),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(defaultPicture, 0),
                    Title = string.Format(_localizationService.GetResource("Media.Post.ImageLinkTitleFormat.Details"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Post.ImageAlternateTextFormat.Details"), model.Name),
                };
                //"title" attribute
                defaultPictureModel.Title = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.TitleAttribute)) ?
                    defaultPicture.TitleAttribute :
                    string.Format(_localizationService.GetResource("Media.Post.ImageLinkTitleFormat.Details"), model.Name);
                //"alt" attribute
                defaultPictureModel.AlternateText = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.AltAttribute)) ?
                    defaultPicture.AltAttribute :
                    string.Format(_localizationService.GetResource("Media.Post.ImageAlternateTextFormat.Details"), model.Name);

                //all pictures
                var pictureModels = new List<PictureModel>();
                foreach (var picture in pictures)
                {
                    var pictureModel = new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.PostThumbPictureSizeOnPostDetailsPage),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        Title = string.Format(_localizationService.GetResource("Media.Post.ImageLinkTitleFormat.Details"), model.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Post.ImageAlternateTextFormat.Details"), model.Name),
                    };
                    //"title" attribute
                    pictureModel.Title = !string.IsNullOrEmpty(picture.TitleAttribute) ?
                        picture.TitleAttribute :
                        string.Format(_localizationService.GetResource("Media.Post.ImageLinkTitleFormat.Details"), model.Name);
                    //"alt" attribute
                    pictureModel.AlternateText = !string.IsNullOrEmpty(picture.AltAttribute) ?
                        picture.AltAttribute :
                        string.Format(_localizationService.GetResource("Media.Post.ImageAlternateTextFormat.Details"), model.Name);

                    pictureModels.Add(pictureModel);
                }

                return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
            });
            model.DefaultPictureModel = cachedPictures.DefaultPictureModel;
            model.PictureModels = cachedPictures.PictureModels;

            #endregion


            return model;
        }

        [ChildActionOnly]
        public ActionResult TopViewPosts(int? categoryId, int? postThumbPictureSize)
        {
            var posts = _postService.SearchPosts(pageSize:5,categoryIds: categoryId == null? null : new []{categoryId.Value});
            var model = PreparePostOverviewModels(posts, true, postThumbPictureSize);
            return PartialView(model);
        }

        public ActionResult PostDetails(int postId)
        {
            var post = _postService.GetPostById(postId);
            if (post == null || post.Deleted)
                return InvokeHttp404();

            //published?
            if (!_catalogSettings.AllowViewUnpublishedPostPage)
            {
                //Check whether the current user has a "Manage catalog" permission
                //It allows him to preview a post before publishing
                if (!post.Published && !_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                    return InvokeHttp404();
            }

            //ACL (access control list)
            if (!_aclService.Authorize(post))
                return InvokeHttp404();
            var model = PreparePostDetailsPageModel(post);

            post.ViewCount += 1;

            _postService.UpdatePost(post);

            //save as recently viewed
           // _recentlyViewedPostsService.AddPostToRecentlyViewedList(post.Id);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewPost", _localizationService.GetResource("ActivityLog.PublicStore.ViewPost"), post.Name);

            return View(model.PostTemplateViewPath, model);
        }
        [ChildActionOnly]
        public ActionResult HomePagePosts(int? postThumbPictureSize)
        {
            var posts = _postService.GetAllPostsDisplayedOnHomePage();
            var model = PreparePostOverviewModels(posts, true, postThumbPictureSize);
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult RelatedPosts(int postId)
        {
            //load and cache report
            var postIds = _cacheManager.Get(string.Format(ModelCacheEventConsumer.PRODUCTS_RELATED_IDS_KEY, postId),
                () =>
                    _postService.GetRelatedPostsByPostId1(postId).Select(x => x.PostId2).ToArray()
                    );

            //load posts
            var posts = _postService.GetPostsByIds(postIds);
            //ACL and store mapping
            posts = posts.Where(p => _aclService.Authorize(p)).ToList();

            if (posts.Count == 0)
                return Content("");

            var model = PreparePostOverviewModels(posts, false).ToList();
            return PartialView(model);
        }
        [ChildActionOnly]
        public ActionResult OtherPosts(int postId, int? postThumbPictureSize)
        {
            var posts = _postService.GetOrtherPosts(postId, 2);
            var model = PreparePostOverviewModels(posts, true, postThumbPictureSize);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult UpdatePost(int postId)
        {
            var post = _postService.GetPostById(postId);
            if (post == null) return Content("");
            var web = new WebClient();
            var url = string.Format("https://api.facebook.com/method/links.getStats?format=json&urls={0}", @Url.RouteUrl("Post", new { SeName = post.GetSeName() }, this.Request.Url.Scheme));
            //var url = "https://api.facebook.com/method/links.getStats?format=json&urls=http://vietnam4u.vn/mot-dieu-thuoc-la-vut-di-gia-300-euro-o-italy";
            var response = web.DownloadString(url);
            var updateDb = false;
            var result = JsonConvert.DeserializeObject<List<FacebookStats>>(response);
            if(result != null && result.Count > 0)
            {
                if (result[0].commentsbox_count != post.CommentCount)
                {
                    post.CommentCount = result[0].commentsbox_count;
                    updateDb = true;
                }
                if (result[0].share_count != post.ShareCount)
                {
                    post.ShareCount = result[0].share_count;
                    updateDb = true;
                }
            }
                
            if(updateDb) _postService.UpdatePost(post);

            return Content(url);
        }

        [ChildActionOnly]
        public ActionResult GetRandomPost(int postId, int numberPost, string template, int? postThumbPictureSize)
        {

            var post = _postService.GetPostById(postId);
            var model =
                PreparePostOverviewModels(_postService.GetRandomPosts(numberPost, templateId: 0,
                    excludePostId: post == null ? 0 : post.Id));
            return PartialView(template, model);
        }
        #endregion

        #region Post tag

        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult PostsByTag(int postTagId, PostsPagingFilteringModel command)
        {
            var postTag = _postTagService.GetPostTagById(postTagId);
            if (postTag == null)
                return InvokeHttp404();

            var model = new PostsByTagModel
            {
                Id = postTag.Id,
                TagName = postTag.GetLocalized(y => y.Name),
                TagSeName = postTag.GetSeName()
            };

            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command, 10);


            //posts
            var posts = _postService.SearchPosts(
                postTagId: postTag.Id,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Posts = PreparePostOverviewModels(posts).ToList();

            model.PagingFilteringContext.LoadPagedList(posts);
            return View(model);
        }

        #endregion
    }
}