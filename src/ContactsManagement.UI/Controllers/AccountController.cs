using ContactsManagement.Core.Domain.IdentityEntities;
using ContactsManagement.Core.DTO.Identities;
using ContactsManagement.UI.Filters.ActionFilters.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManagement.UI.Controllers;
[Route("[controller]")]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    [Route("[action]")]
    public IActionResult Register()
    {
        return View();
    }
    
    [Route("[action]")]
    [HttpPost]
    [AccountSubmitRegisterActionFilter]
    public async Task<IActionResult> SubmitRegister([FromForm] RegisterDTO registerDTO)
    {
        return Json(registerDTO);
    }
    public IActionResult Login()
    {
        throw new NotImplementedException();
    }
}