using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManagement.WebApi.UI.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class JwtAuthenticationController : ControllerBase
    {
        [HttpGet("[action]")]
        public ActionResult Me()
        {
            var user = new {name = User.Identity!.Name};
            return Ok(user);
        }
    }
}
