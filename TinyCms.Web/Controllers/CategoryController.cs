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