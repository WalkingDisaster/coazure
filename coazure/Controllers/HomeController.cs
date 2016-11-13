using System.Configuration;
using System.Diagnostics;
using System.Security.Claims;
using System.Web.Mvc;
using coazure.Services;

namespace coazure.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Environment = ConfigurationManager.AppSettings["Environment"];

            var principal = new PrincipalHelper(ClaimsPrincipal.Current);
            principal.DoIf(authenticated: t =>
            {
                ViewBag.User = principal.GetFullName();

                if (principal.IsAzureActiveDirectoryUser)
                    ViewBag.UserType = "Organization User";
                else
                    ViewBag.UserType = "Social User: " + principal.AuthenticationType;
            },
                notAuthenticated: () =>
                {
                    ViewBag.User = "Unknown";
                    ViewBag.UserType = "Unknown";
                });

            return View();
        }

        public JsonResult GetIdentity()
        {

            var principal = new PrincipalHelper(ClaimsPrincipal.Current);
            var result = principal.MakeSerializable();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}