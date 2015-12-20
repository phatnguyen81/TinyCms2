using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TinyCms.Services.Media;
using TinyCms.Services.Posts;
using TinyCms.Web.Models.Posts;

namespace TinyCms.Web.Controllers
{
    public class PostsController : BasePublicController
    {
        private readonly ICategoryService _categoryService;
        private readonly IPostService _postService;
        private readonly IPictureService _picture;

        public PostsController(ICategoryService categoryService, 
            IPostService postService, 
            IPictureService picture)
        {
            _categoryService = categoryService;
            _postService = postService;
            _picture = picture;
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
        public ActionResult TopMenu()
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
    }
}