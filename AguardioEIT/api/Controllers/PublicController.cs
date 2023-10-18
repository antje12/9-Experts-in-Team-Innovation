using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class PublicController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("Version")]
    public object Version()
    {
        return new { Version = "1.00" };
    }
}