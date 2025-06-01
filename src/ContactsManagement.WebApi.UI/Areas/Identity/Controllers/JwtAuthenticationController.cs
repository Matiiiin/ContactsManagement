using System.IdentityModel.Tokens.Jwt;
using Asp.Versioning;
using ContactsManagement.Core.Domain.IdentityEntities;
using ContactsManagement.Core.DTO.Identities;
using ContactsManagement.Core.Enums;
using ContactsManagement.Core.ServiceContracts.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace ContactsManagement.WebApi.UI.Areas.Identity.Controllers
{
    /// <summary>
    /// Controller for handling JWT-based authentication operations such as login, registration, and user info retrieval.
    /// </summary>
    [Area("Identity")]
    [Route("api/[area]/[controller]")]
    [ApiVersion(1.0)]
    [ApiController]
    public class JwtAuthenticationController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="JwtAuthenticationController"/> class.
        /// </summary>
        /// <param name="jwtService">Service for generating JWT tokens.</param>
        /// <param name="userManager">User manager for identity operations.</param>
        /// <param name="signInManager">Sign-in manager for authentication.</param>

        public JwtAuthenticationController(IJwtService jwtService,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // <summary>
        /// Gets the currently authenticated user's information.
        /// </summary>
        /// <returns>The authenticated user's details or an error response.</returns>

        [HttpGet("[action]")]
        [Authorize]
        public async Task<ActionResult> Me()
        {
            // Get user ID from claims
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            // Fetch the user from the database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Now you have the ApplicationUser object
            return Ok(new{user=user , id = userId});
        }
        
        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="loginDTO">The login credentials.</param>
        /// <returns>A JWT token and user information if authentication is successful; otherwise, an error response.</returns>

        [HttpPost("[action]")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(errors);
                }
                var user = _userManager.FindByEmailAsync(loginDTO.Email!).Result;
                if (user == null)
                {
                    return Unauthorized("Invalid Email");
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password!, false);
                if (!result.Succeeded)
                {
                    return Unauthorized("Invalid Password or Email");
                }

                return Ok(new JwtUserResponseDTO()
                {
                    Email = user.Email!,
                    UserName = user.UserName!,
                    Token = _jwtService.GenerateToken(user)
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Registers a new user and returns a JWT token if successful.
        /// </summary>
        /// <param name="registerDto">The registration details.</param>
        /// <returns>A JWT token and user information if registration is successful; otherwise, an error response.</returns>

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
                        return Ok(new JwtUserResponseDTO()
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
