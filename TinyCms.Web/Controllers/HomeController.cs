using System.Web.Mvc;
using TinyCms.Web.Framework.Security;

namespace TinyCms.Web.Controllers
{
    [NopHttpsRequirement(SslRequirement.No)]
    public class HomeController : BasePublicController
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}