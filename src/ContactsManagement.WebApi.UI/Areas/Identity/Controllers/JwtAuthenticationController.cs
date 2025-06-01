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
using Microsoft.IdentityModel.Tokens;
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
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="JwtAuthenticationController"/> class.
        /// </summary>
        /// <param name="jwtService">Service for generating JWT tokens.</param>
        /// <param name="userManager">User manager for identity operations.</param>
        /// <param name="signInManager">Sign-in manager for authentication.</param>

        public JwtAuthenticationController(IConfiguration configuration,IJwtService jwtService,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
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
        public async Task<ActionResult> CreateToken([FromBody] LoginDTO loginDTO)
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
                await _signInManager.SignInAsync(user, false);
                return Ok(new JwtUserResponseDTO()
                {
                    Email = user.Email!,
                    UserName = user.UserName!,
                    Token = _jwtService.GenerateToken(user),
                    RefreshToken = user.RefreshToken!
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
                    RefreshToken = _jwtService.GenerateRefreshToken(),
                    RefreshTokenExpiresAt = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["RefreshToken:ExpirationMinutes"]))
                };
                var createdUser = await _userManager.CreateAsync(user, registerDto.Password!);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, UserRoleEnum.User.ToString());
                    if (roleResult.Succeeded)
                    {
                        return Ok(new JwtUserResponseDTO()
                        {
                            Email = user.Email!,
                            UserName = user.UserName!,
                            Token = _jwtService.GenerateToken(user),
                            RefreshToken = user.RefreshToken!
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

        /// <summary>
        /// Refreshes the JWT access token using a valid refresh token.
        /// </summary>
        /// <param name="refreshTokenDTO">The DTO containing the expired access token and the refresh token.</param>
        /// <returns>
        /// A new JWT access token and refresh token if the provided refresh token is valid; otherwise, an error response.
        /// </returns>
        /// <response code="200">Returns the new JWT access token and refresh token.</response>
        /// <response code="400">If the provided token or refresh token is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("[action]")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(errors);
                }
                var handler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateActor = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]!))
                };
                handler.ValidateToken(
                    refreshTokenDTO.Token,
                    validationParameters,
                    out SecurityToken validatedToken
                );

                var userId = (validatedToken as JwtSecurityToken)?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId)?.Value;
                if (userId == null)
                {
                    return BadRequest("Invalid token");
                }
                var user = _userManager.FindByIdAsync(userId!).Result;

                if (user == null)
                {
                    return BadRequest("Invalid token");
                }
                var userRefreshToken = user!.RefreshToken;
                if (userRefreshToken != refreshTokenDTO.Refreshtoken)
                {
                    return BadRequest("Invalid refresh token");
                }
                var newToken = await _jwtService.GenerateTokenFromRefreshToken(user , refreshTokenDTO.Refreshtoken!);
                
                return Ok(new JwtUserResponseDTO()
                {
                    Email = user.Email!,
                    UserName = user.UserName!,
                    Token = newToken.Token,
                    RefreshToken = newToken.RefreshToken
                });
            }
            catch (Exception e)
            {
                return Problem("Something went wrong", e.ToString() ,500,"Registration Failed");
            }
        }
        
    }
}
