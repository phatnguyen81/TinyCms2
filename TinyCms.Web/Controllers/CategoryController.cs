using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TinyCms.Web.Controllers
{
    public class CategoryController : BasePublicController
    {
        // GET: Category
        public ActionResult Category()
        {
            return View();
        }
    }
}