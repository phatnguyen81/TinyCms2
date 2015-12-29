using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TinyCms.Core;
using TinyCms.Core.Caching;
using TinyCms.Core.Domain.Media;
using TinyCms.Core.Domain.Posts;
using TinyCms.Services.Helpers;
using TinyCms.Services.Localization;
using TinyCms.Services.Media;
using TinyCms.Services.Posts;
using TinyCms.Services.Security;
using TinyCms.Services.Seo;
using TinyCms.Web.Infrastructure.Cache;
using TinyCms.Web.Models.Media;
using TinyCms.Web.Models.Posts;

namespace TinyCms.Web.Extensions
{
    //here we have some methods shared between controllers
    public static class ControllerExtensions
    {
      

        public static IEnumerable<PostOverviewModel> PreparePostOverviewModels(this Controller controller,
            IWorkContext workContext,
            ICategoryService categoryService,
            IPostService postService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IWebHelper webHelper,
            ICacheManager cacheManager,
            IDateTimeHelper _dateTimeHelper,
            CatalogSettings catalogSettings,
            MediaSettings mediaSettings,
            IEnumerable<Post> posts,
            bool preparePictureModel = true,
            int? postThumbPictureSize = null
            )
        {
            if (posts == null)
                throw new ArgumentNullException("posts");

            var models = new List<PostOverviewModel>();
            foreach (var post in posts)
            {
                var model = new PostOverviewModel
                {
                    Id = post.Id,
                    Name = post.GetLocalized(x => x.Name),
                    ShortDescription = post.GetLocalized(x => x.ShortDescription),
                    FullDescription = post.GetLocalized(x => x.FullDescription),
                    SeName = post.GetSeName(),
                    ViewCount = post.ViewCount,
                    ShareCount = post.ShareCount,
                    CommentCount = post.CommentCount,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(post.CreatedOnUtc),
                    Publish = post.Published
                };
               

                //picture
                if (preparePictureModel)
                {
                    #region Prepare post picture

                    //If a size has been set in the view, we use it in priority
                    int pictureSize = postThumbPictureSize.HasValue ? postThumbPictureSize.Value : mediaSettings.PostThumbPictureSize;
                    //prepare picture model
                    var defaultPostPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, post.Id, pictureSize, true, workContext.WorkingLanguage.Id, webHelper.IsCurrentConnectionSecured());
                    model.DefaultPictureModel = cacheManager.Get(defaultPostPictureCacheKey, () =>
                    {
                        var picture = pictureService.GetPicturesByPostId(post.Id, 1).FirstOrDefault();
                        var pictureModel = new PictureModel
                        {
                            ImageUrl = pictureService.GetPictureUrl(picture, pictureSize),
                            FullSizeImageUrl = pictureService.GetPictureUrl(picture)
                        };
                        //"title" attribute
                        pictureModel.Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute)) ?
                            picture.TitleAttribute :
                            string.Format(localizationService.GetResource("Media.Post.ImageLinkTitleFormat"), model.Name);
                        //"alt" attribute
                        pictureModel.AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute)) ?
                            picture.AltAttribute :
                            string.Format(localizationService.GetResource("Media.Post.ImageAlternateTextFormat"), model.Name);
                        
                        return pictureModel;
                    });

                    #endregion
                }

            

                models.Add(model);
            }
            return models;
        }
    }
}