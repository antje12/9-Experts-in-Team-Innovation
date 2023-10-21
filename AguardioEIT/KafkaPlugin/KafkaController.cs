using Microsoft.AspNetCore.Mvc;
using Interfaces;

namespace KafkaPlugin;

[ApiController]
[Route("[controller]")]
public class KafkaController : ControllerBase
{
    private IKafkaPluginService _ps;

    public KafkaController(IKafkaPluginService ps)
    {
        this._ps = ps;
    }

    [HttpGet("Version")]
    public object Version()
    {
        return $"Plugin Controller v 1.0 {_ps.Test()}";
    }

    [HttpGet("Status")]
    public object Status()
    {
        return $"Plugin Controller v 1.0 {_ps.Status()}";
    }

    [HttpGet("Produce")]
    public object Produce()
    {
        return $"Plugin Controller v 1.0 {_ps.Produce()}";
    }

    [HttpGet("ConsumeStart")]
    public object ConsumeStart()
    {
        return $"Plugin Controller v 1.0 {_ps.ConsumeStart()}";
    }

    [HttpGet("ConsumeStop")]
    public object ConsumeStop()
    {
        return $"Plugin Controller v 1.0 {_ps.ConsumeStop()}";
    }
}