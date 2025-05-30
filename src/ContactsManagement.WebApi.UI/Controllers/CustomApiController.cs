using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManagement.WebApi.UI.Controllers;
/// <summary>
/// Base Controller for apis version 1.0
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class CustomApiController : ControllerBase
{

}