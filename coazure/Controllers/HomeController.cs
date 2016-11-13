using System.Configuration;
using System.Web.Mvc;

namespace coazure.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Environment = ConfigurationManager.AppSettings["Environment"];
            return View();
        }
    }
}