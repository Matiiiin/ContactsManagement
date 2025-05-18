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
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    [Route("[action]")]
    [HttpPost]
    [AccountSubmitRegisterActionFilter]
    public async Task<IActionResult> SubmitRegister([FromForm] RegisterDTO registerDTO)
    {
        var user = new ApplicationUser()
        {
            UserName = registerDTO.Email,
            Email = registerDTO.Email,
            EmailConfirmed = false,
            FullName = registerDTO.PersonName,
            PhoneNumber = registerDTO.Phone,
            PhoneNumberConfirmed = false,
        };
        var result = await _userManager.CreateAsync(user, registerDTO.Password!);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            ViewBag.Errors = errors;
            return View("Register" , registerDTO);
        }
        return RedirectToAction("Index", "Persons");
    }
    public IActionResult Login()
    {
        throw new NotImplementedException();
    }
}