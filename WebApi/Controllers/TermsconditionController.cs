using System.Web.Mvc;

namespace WebPlatApi.Controllers
{
    public class TermsconditionController : Controller
    {
        [HttpGet]
        public ActionResult Terms()
        {
            return View();
        }

    }
}
