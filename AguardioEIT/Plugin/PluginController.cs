using Microsoft.AspNetCore.Mvc;

namespace Plugin
{
    [ApiController]
    [Route("[controller]")]
    public class PluginController : ControllerBase
    {
        public PluginController()
        {
        }

        [HttpGet("Version")]
        public object Version()
        {
            return "Plugin Controller v 1.0";
        }
    }
}