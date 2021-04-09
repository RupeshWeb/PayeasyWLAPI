using System.Web.Mvc;

namespace WebPlatApi.Controllers
{
    public class PrivacypolicyController : Controller
    {
        [HttpGet]
        public ActionResult Policy()
        {
            return View();
        }

    }
}
