using System.Linq;
using System.Threading.Tasks;
using ContactsManagement.Core.Domain.IdentityEntities;
using ContactsManagement.Core.DTO.Identities;
using ContactsManagement.UI.Filters.ActionFilters.Account;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ContactsManagement.UI.Controllers;
[Route("[controller]/[action]")]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
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
        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Persons");
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    [AccountSubmitLoginActionFilter]
    public async Task<IActionResult> SubmitLogin([FromForm] LoginDTO loginDTO)
    {
        
        var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, false, false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt , Check your email and password again.");
            return View("Login", loginDTO);
        }
        return RedirectToAction("Index" ,"Persons");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index","Persons");
    }
}