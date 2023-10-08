using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class PublicController : ControllerBase
{
    private readonly IPluginService _pluginService;

    public PublicController(IPluginService pluginService)
    {
        _pluginService = pluginService;
    }

    [AllowAnonymous]
    [HttpGet("Version")]
    public object Version()
    {
        return new { Version = "1.00", PluginSays = _pluginService.Test() };
    }
}