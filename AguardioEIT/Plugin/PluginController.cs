using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Plugin
{
    [ApiController]
    [Route("[controller]")]
    public class PluginController : ControllerBase
    {
        private readonly IPluginService _pluginService;
        private readonly IPlugin2Service _plugin2Service;
        private readonly IApplicationService _applicationService;
        public PluginController(IPluginService pluginService, IApplicationService applicationService, IPlugin2Service plugin2Service)
        {
            _pluginService = pluginService;
            _applicationService = applicationService;
            _plugin2Service = plugin2Service;
        }

        [HttpGet("Version")]
        public object Version()
        {
            return $"Plugin Controller v 1.0 {_pluginService.Test()} {_applicationService.Test()} " +
                   $"and 1 + 2 = {_plugin2Service.Add(1,2)}";
        }
    }
}