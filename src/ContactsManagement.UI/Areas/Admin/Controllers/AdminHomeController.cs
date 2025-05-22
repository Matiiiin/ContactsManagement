 using Microsoft.AspNetCore.Authorization;
 using Microsoft.AspNetCore.Mvc;

namespace ContactsManagement.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    // [Authorize(Policy = "AdminOnly")]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

    }
}
