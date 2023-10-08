using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Plugin
{
    [ApiController]
    [Route("[controller]")]
    public class PluginController : ControllerBase
    {
        private readonly IPluginService _pluginService;
        private readonly IApplicationService _applicationService;
        public PluginController(IPluginService pluginService, IApplicationService applicationService)
        {
            _pluginService = pluginService;
            _applicationService = applicationService;
        }

        [HttpGet("Version")]
        public object Version()
        {
            return $"Plugin Controller v 1.0 {_pluginService.Test()} {_applicationService.Test()}";
        }
    }
}