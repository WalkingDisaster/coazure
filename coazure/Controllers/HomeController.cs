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
            Trace.TraceInformation("Index");
            ViewBag.Environment = ConfigurationManager.AppSettings["Environment"];

            var principal = new PrincipalHelper(ClaimsPrincipal.Current);
            principal.DoIf(authenticated: t =>
            {
                ViewBag.User = principal.GetFullName();
                Trace.TraceInformation($"Logged in as {ViewBag.User}");

                if (principal.IsAzureActiveDirectoryUser)
                    ViewBag.UserType = "Organization User";
                else
                    ViewBag.UserType = "Social User: " + principal.AuthenticationType;
            },
                notAuthenticated: () =>
                {
                    Trace.TraceWarning("User not logged in!");
                    ViewBag.User = "Unknown";
                    ViewBag.UserType = "Unknown";
                });

            return View();
        }

        public JsonResult GetIdentity()
        {
            Trace.TraceInformation("Displaying Identity");

            var principal = new PrincipalHelper(ClaimsPrincipal.Current);
            var result = principal.MakeSerializable();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}