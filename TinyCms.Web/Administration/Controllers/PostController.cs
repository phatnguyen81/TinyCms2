using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TinyCms.Admin.Extensions;
using TinyCms.Admin.Infrastructure.Cache;
using TinyCms.Admin.Models.Posts;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Domain.Posts;
using TinyCms.Services.Customers;
using TinyCms.Services.ExportImport;
using TinyCms.Services.Helpers;
using TinyCms.Services.Localization;
using TinyCms.Services.Logging;
using TinyCms.Services.Media;
using TinyCms.Services.Posts;
using TinyCms.Services.Security;
using TinyCms.Services.Seo;
using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Controllers;
using TinyCms.Web.Framework.Kendoui;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Controllers
{
    public partial class PostController : BaseAdminController
    {
        #region Fields

        private readonly IPostService _postService;
        private readonly IPostTagService _postTagService;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly ICacheManager _cacheManager;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPostTemplateService _postTemplateService;

        #endregion

		#region Constructors

        public PostController(IPostService postService, 
            ICategoryService categoryService, 
            ICustomerService customerService,
            IUrlRecordService urlRecordService, 
            IWorkContext workContext, 
            ILanguageService languageService, 
            ILocalizationService localizationService, 
            ILocalizedEntityService localizedEntityService,
            IPictureService pictureService,
            IExportManager exportManager, 
            IImportManager importManager,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService, 
            IAclService aclService,
            ICacheManager cacheManager,
            IDateTimeHelper dateTimeHelper, 
            IPostTagService postTagService, IPostTemplateService postTemplateService)
        {
            this._postService = postService;
            this._categoryService = categoryService;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._pictureService = pictureService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._cacheManager = cacheManager;
            this._dateTimeHelper = dateTimeHelper;
            this._postTagService = postTagService;
            _postTemplateService = postTemplateService;
        }

        #endregion 

        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(Post post, PostModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(post,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(post,
                                                               x => x.ShortDescription,
                                                               localized.ShortDescription,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(post,
                                                               x => x.FullDescription,
                                                               localized.FullDescription,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(post,
                                                               x => x.MetaKeywords,
                                                               localized.MetaKeywords,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(post,
                                                               x => x.MetaDescription,
                                                               localized.MetaDescription,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(post,
                                                               x => x.MetaTitle,
                                                               localized.MetaTitle,
                                                               localized.LanguageId);

                //search engine name
                var seName = post.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(post, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(PostTag postTag, PostTagModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(postTag,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }



        [NonAction]
        protected virtual void UpdatePictureSeoNames(Post post)
        {
            foreach (var pp in post.PostPictures)
                _pictureService.SetSeoFilename(pp.PictureId, _pictureService.GetPictureSeName(post.Name));
        }
        
        [NonAction]
        protected virtual void PrepareAclModel(PostModel model, Post post, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableCustomerRoles = _customerService
                .GetAllCustomerRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (post != null)
                {
                    model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(post);
                }
            }
        }

        [NonAction]
        protected virtual void SavePostAcl(Post post, PostModel model)
        {
            var existingAclRecords = _aclService.GetAclRecords(post);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds != null && model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(post, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }


        [NonAction]
        protected virtual string[] ParsePostTags(string postTags)
        {
            var result = new List<string>();
            if (!String.IsNullOrWhiteSpace(postTags))
            {
                string[] values = postTags.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
        protected virtual void PreparePostModel(PostModel model, Post post,
            bool setPredefinedValues, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (post != null)
            {
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(post.CreatedOnUtc, DateTimeKind.Utc);
                model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(post.UpdatedOnUtc, DateTimeKind.Utc);
            }

            //little performance hack here
            //there's no need to load attributes, categories, manufacturers when creating a new post
            //anyway they're not used (you need to save a post before you map add them)
            if (post != null)
            {
            
                //categories
                var allCategories = _categoryService.GetAllCategories(showHidden: true);
                foreach (var category in allCategories)
                {
                    model.AvailableCategories.Add(new SelectListItem
                    {
                        Text = category.GetFormattedBreadCrumb(allCategories),
                        Value = category.Id.ToString()
                    });
                }

            }


            //copy post
            if (post != null)
            {
                model.CopyPostModel.Id = post.Id;
                model.CopyPostModel.Name = "Copy of " + post.Name;
                model.CopyPostModel.Published = true;
                model.CopyPostModel.CopyImages = true;
            }

            //templates
            var templates = _postTemplateService.GetAllPostTemplates();
            foreach (var template in templates)
            {
                model.AvailablePostTemplates.Add(new SelectListItem
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
       
            //post tags
            if (post != null)
            {
                var result = new StringBuilder();
                for (int i = 0; i < post.PostTags.Count; i++)
                {
                    var pt = post.PostTags.ToList()[i];
                    result.Append(pt.Name);
                    if (i != post.PostTags.Count - 1)
                        result.Append(", ");
                }
                model.PostTags = result.ToString();
            }

          

            //default values
            if (setPredefinedValues)
            {
                model.Published = true;
            }
        }

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            var categoriesIds = new List<int>();
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, true);
            foreach (var category in categories)
            {
                categoriesIds.Add(category.Id);
                categoriesIds.AddRange(GetChildCategoryIds(category.Id));
            }
            return categoriesIds;
        }


        #endregion

        #region Methods

        #region Post list / create / edit / delete

        //list posts
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var model = new PostListModel();

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = _categoryService.GetAllCategories(showHidden: true);
            foreach (var c in categories)
                model.AvailableCategories.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });

            //"published" property
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Posts.List.SearchPublished.All"), Value = "0" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Posts.List.SearchPublished.PublishedOnly"), Value = "1" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Posts.List.SearchPublished.UnpublishedOnly"), Value = "2" });

            return View(model);
        }

        [HttpPost]
        public ActionResult PostList(DataSourceRequest command, PostListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();


            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var posts = _postService.SearchPosts(
                categoryIds: categoryIds,
                keywords: model.SearchPostName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true,
                overridePublished: overridePublished
            );
            var gridModel = new DataSourceResult();
            gridModel.Data = posts.Select(x =>
            {
                var postModel = x.ToModel();
                //little hack here:
                //ensure that post full descriptions are not returned
                //otherwise, we can get the following error if posts have too long descriptions:
                //"Error during serialization or deserialization using the JSON JavaScriptSerializer. The length of the string exceeds the value set on the maxJsonLength property. "
                //also it improves performance
                postModel.FullDescription = "";

                //picture
                var defaultPostPicture = _pictureService.GetPicturesByPostId(x.Id, 1).FirstOrDefault();
                postModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultPostPicture, 75, true);
                return postModel;
            });
            gridModel.Total = posts.TotalCount;

            return Json(gridModel);
        }


        //create post
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var model = new PostModel();
            PreparePostModel(model, null, true, true);
            AddLocales(_languageService, model.Locales);
            PrepareAclModel(model, null, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(PostModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
             

                //post
                var post = model.ToEntity();
                post.CreatedOnUtc = DateTime.UtcNow;
                post.UpdatedOnUtc = DateTime.UtcNow;
                _postService.InsertPost(post);
                //search engine name
                model.SeName = post.ValidateSeName(model.SeName, post.Name, true);
                _urlRecordService.SaveSlug(post, model.SeName, 0);
                //locales
                UpdateLocales(post, model);
                //ACL (customer roles)
                SavePostAcl(post, model);
                //tags
                SavePostTags(post, ParsePostTags(model.PostTags));

                //activity log
                _customerActivityService.InsertActivity("AddNewPost", _localizationService.GetResource("ActivityLog.AddNewPost"), post.Name);
                
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Posts.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = post.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PreparePostModel(model, null, false, true);
            PrepareAclModel(model, null, true);
            return View(model);
        }

        //edit post
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var post = _postService.GetPostById(id);
            if (post == null || post.Deleted)
                //No post found with the specified id
                return RedirectToAction("List");

            var model = post.ToModel();
            PreparePostModel(model, post, false, false);
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
                {
                    locale.Name = post.GetLocalized(x => x.Name, languageId, false, false);
                    locale.ShortDescription = post.GetLocalized(x => x.ShortDescription, languageId, false, false);
                    locale.FullDescription = post.GetLocalized(x => x.FullDescription, languageId, false, false);
                    locale.MetaKeywords = post.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = post.GetLocalized(x => x.MetaDescription, languageId, false, false);
                    locale.MetaTitle = post.GetLocalized(x => x.MetaTitle, languageId, false, false);
                    locale.SeName = post.GetSeName(languageId, false, false);
                });

            PrepareAclModel(model, post, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(PostModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var post = _postService.GetPostById(model.Id);
            if (post == null || post.Deleted)
                //No post found with the specified id
                return RedirectToAction("List");

        
            if (ModelState.IsValid)
            {
                //post
                post = model.ToEntity(post);
                post.UpdatedOnUtc = DateTime.UtcNow;
                _postService.UpdatePost(post);
                //search engine name
                model.SeName = post.ValidateSeName(model.SeName, post.Name, true);
                _urlRecordService.SaveSlug(post, model.SeName, 0);
                //locales
                UpdateLocales(post, model);
                //tags
                SavePostTags(post, ParsePostTags(model.PostTags));
                //ACL (customer roles)
                SavePostAcl(post, model);
                //picture seo names
                UpdatePictureSeoNames(post);
                
                //activity log
                _customerActivityService.InsertActivity("EditPost", _localizationService.GetResource("ActivityLog.EditPost"), post.Name);
                
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Posts.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();

                    return RedirectToAction("Edit", new {id = post.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PreparePostModel(model, post, false, true);
            PrepareAclModel(model, post, true);
            return View(model);
        }

        //delete post
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var post = _postService.GetPostById(id);
            if (post == null)
                //No post found with the specified id
                return RedirectToAction("List");

            _postService.DeletePost(post);

            //activity log
            _customerActivityService.InsertActivity("DeletePost", _localizationService.GetResource("ActivityLog.DeletePost"), post.Name);
                
            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Posts.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var posts = new List<Post>();
            if (selectedIds != null)
            {
                posts.AddRange(_postService.GetPostsByIds(selectedIds.ToArray()));

                for (int i = 0; i < posts.Count; i++)
                {
                    var post = posts[i];

                 

                    _postService.DeletePost(post);
                }
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Required posts

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LoadPostFriendlyNames(string postIds)
        {
            var result = "";

            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return Json(new { Text = result });

            if (!String.IsNullOrWhiteSpace(postIds))
            {
                var ids = new List<int>();
                var rangeArray = postIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                foreach (string str1 in rangeArray)
                {
                    int tmp1;
                    if (int.TryParse(str1, out tmp1))
                        ids.Add(tmp1);
                }

                var posts = _postService.GetPostsByIds(ids.ToArray());
                for (int i = 0; i <= posts.Count - 1; i++)
                {
                    result += posts[i].Name;
                    if (i != posts.Count - 1)
                        result += ", ";
                }
            }

            return Json(new { Text = result });
        }

        #endregion
        
        #region Post categories

        [HttpPost]
        public ActionResult PostCategoryList(DataSourceRequest command, int postId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

        
            var postCategories = _categoryService.GetPostCategoriesByPostId(postId, true);
            var postCategoriesModel = postCategories
                .Select(x => new PostModel.PostCategoryModel
                {
                    Id = x.Id,
                    Category = _categoryService.GetCategoryById(x.CategoryId).GetFormattedBreadCrumb(_categoryService),
                    PostId = x.PostId,
                    CategoryId = x.CategoryId,
                    IsFeaturedPost = x.IsFeaturedPost,
                    DisplayOrder  = x.DisplayOrder
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = postCategoriesModel,
                Total = postCategoriesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult PostCategoryInsert(PostModel.PostCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var postId = model.PostId;
            var categoryId = model.CategoryId;

        

            var existingPostCategories = _categoryService.GetPostCategoriesByCategoryId(categoryId, showHidden: true);
            if (existingPostCategories.FindPostCategory(postId, categoryId) == null)
            {
                var postCategory = new PostCategory
                {
                    PostId = postId,
                    CategoryId = categoryId,
                    DisplayOrder = model.DisplayOrder
                };
              
                _categoryService.InsertPostCategory(postCategory);
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult PostCategoryUpdate(PostModel.PostCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var postCategory = _categoryService.GetPostCategoryById(model.Id);
            if (postCategory == null)
                throw new ArgumentException("No post category mapping found with the specified id");

          
            postCategory.CategoryId = model.CategoryId;
            postCategory.DisplayOrder = model.DisplayOrder;
           
            _categoryService.UpdatePostCategory(postCategory);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult PostCategoryDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var postCategory = _categoryService.GetPostCategoryById(id);
            if (postCategory == null)
                throw new ArgumentException("No post category mapping found with the specified id");

            var postId = postCategory.PostId;

            _categoryService.DeletePostCategory(postCategory);

            return new NullJsonResult();
        }

        #endregion


        #region Post pictures

        [ValidateInput(false)]
        public ActionResult PostPictureAdd(int pictureId, int displayOrder, 
            string overrideAltAttribute, string overrideTitleAttribute,
            int postId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            var post = _postService.GetPostById(postId);
            if (post == null)
                throw new ArgumentException("No post found with the specified id");
            
            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _postService.InsertPostPicture(new PostPicture
            {
                PictureId = pictureId,
                PostId = postId,
                DisplayOrder = displayOrder,
            });

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename, 
                overrideAltAttribute, 
                overrideTitleAttribute);

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(post.Name));

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PostPictureList(DataSourceRequest command, int postId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var postPictures = _postService.GetPostPicturesByPostId(postId);
            var postPicturesModel = postPictures
                .Select(x =>
                        {
                            var picture = _pictureService.GetPictureById(x.PictureId);
                            if (picture == null)
                                throw new Exception("Picture cannot be loaded");
                            var m = new PostModel.PostPictureModel
                                    {
                                        Id = x.Id,
                                        PostId = x.PostId,
                                        PictureId = x.PictureId,
                                        PictureUrl = _pictureService.GetPictureUrl(picture),
                                        OverrideAltAttribute = picture.AltAttribute,
                                        OverrideTitleAttribute = picture.TitleAttribute,
                                        DisplayOrder = x.DisplayOrder
                                    };
                            return m;
                        })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = postPicturesModel,
                Total = postPicturesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult PostPictureUpdate(PostModel.PostPictureModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var postPicture = _postService.GetPostPictureById(model.Id);
            if (postPicture == null)
                throw new ArgumentException("No post picture found with the specified id");

      

            postPicture.DisplayOrder = model.DisplayOrder;
            _postService.UpdatePostPicture(postPicture);

            var picture = _pictureService.GetPictureById(postPicture.PictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute, 
                model.OverrideTitleAttribute);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult PostPictureDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePosts))
                return AccessDeniedView();

            var postPicture = _postService.GetPostPictureById(id);
            if (postPicture == null)
                throw new ArgumentException("No post picture found with the specified id");

            var postId = postPicture.PostId;

            var pictureId = postPicture.PictureId;
            _postService.DeletePostPicture(postPicture);

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");
            _pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }

        #endregion

   
        #region Post tags

        public ActionResult PostTags()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePostTags))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult PostTags(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePostTags))
                return AccessDeniedView();

            var tags = _postTagService.GetAllPostTags()
                //order by post count
                .OrderByDescending(x => _postTagService.GetPostCount(x.Id, 0))
                .Select(x => new PostTagModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    PostCount = _postTagService.GetPostCount(x.Id, 0)
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = tags.PagedForCommand(command),
                Total = tags.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult PostTagDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePostTags))
                return AccessDeniedView();

            var tag = _postTagService.GetPostTagById(id);
            if (tag == null)
                throw new ArgumentException("No post tag found with the specified id");
            _postTagService.DeletePostTag(tag);

            return new NullJsonResult();
        }

        //edit
        public ActionResult EditPostTag(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePostTags))
                return AccessDeniedView();

            var postTag = _postTagService.GetPostTagById(id);
            if (postTag == null)
                //No post tag found with the specified id
                return RedirectToAction("List");

            var model = new PostTagModel
            {
                Id = postTag.Id,
                Name = postTag.Name,
                PostCount = _postTagService.GetPostCount(postTag.Id, 0)
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = postTag.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult EditPostTag(string btnId, string formId, PostTagModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePostTags))
                return AccessDeniedView();

            var postTag = _postTagService.GetPostTagById(model.Id);
            if (postTag == null)
                //No post tag found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                postTag.Name = model.Name;
                _postTagService.UpdatePostTag(postTag);
                //locales
                UpdateLocales(postTag, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #endregion
    }
}
