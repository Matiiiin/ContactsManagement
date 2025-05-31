using Asp.Versioning;
using ContactsManagement.Core.Domain.IdentityEntities;
using ContactsManagement.Core.DTO.Identities;
using ContactsManagement.Core.Enums;
using ContactsManagement.Core.ServiceContracts.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace ContactsManagement.WebApi.UI.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("api/[area]/[controller]")]
    [ApiVersion(1.0)]
    [ApiController]
    public class JwtAuthenticationController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public JwtAuthenticationController(IJwtService jwtService,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet("[action]")]
        public ActionResult Me()
        {
            var user = new {name = User.Identity!.Name};
            return Ok(user);
        }
        
        [HttpPost("[action]")]
        public ActionResult Login()
        {
            throw new NotImplementedException();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Register([FromBody]RegisterDTO registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(errors);
                }

                var user = new ApplicationUser()
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FullName = registerDto.Email,
                };
                var createdUser = await _userManager.CreateAsync(user, registerDto.Password!);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, UserRoleEnum.User.ToString());
                    if (roleResult.Succeeded)
                    {
                        return Ok(new JwtRegisterResponseDTO()
                        {
                            Email = user.Email,
                            UserName = user.UserName,
                            Token = _jwtService.GenerateToken(user)
                        });
                    }
                    else
                    {
                        return BadRequest(roleResult.Errors);
                    }
                }
                else
                {
                    return BadRequest(createdUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message , ex.ToString() ,500,"Registration Failed");
            }
        }
        
    }
}
