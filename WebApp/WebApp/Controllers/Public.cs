using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class Public : ControllerBase
{
    private IPluginService ps;

    public Public(IPluginService ps)
    {
        this.ps = ps;
    }

    [HttpGet(Name = "Version")]
    public object Version()
    {
        return new { Version = "1.00", PluginSays = ps.Test() };
    }
}