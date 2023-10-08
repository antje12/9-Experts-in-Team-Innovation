using Microsoft.AspNetCore.Mvc;

using Interfaces;

namespace Plugin
{
    [ApiController]
    [Route("[controller]")]
    public class PluginController : ControllerBase
    {
        private IPluginService ps;

        public PluginController(IPluginService ps)
        {
            this.ps = ps;
        }

        [HttpGet("Version")]
        public object Version()
        {
            return $"Plugin Controller v 1.0 {ps.Test()}";
        }

        [HttpGet("Produce")]
        public object Produce()
        {
            return $"Plugin Controller v 1.0 {ps.Produce()}";
        }

        [HttpGet("Consume")]
        public object Consume()
        {
            return $"Plugin Controller v 1.0 {ps.Consume()}";
        }
    }
}