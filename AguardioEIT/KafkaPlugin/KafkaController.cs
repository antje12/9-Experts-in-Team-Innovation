using Microsoft.AspNetCore.Mvc;
using Interfaces;

namespace KafkaPlugin;

[ApiController]
[Route("[controller]")]
public class KafkaController : ControllerBase
{
    private IKafkaPluginService ps;

    public KafkaController(IKafkaPluginService ps)
    {
        this.ps = ps;
    }

    [HttpGet("Version")]
    public object Version()
    {
        return $"Plugin Controller v 1.0 {ps.Test()}";
    }

    [HttpGet("Status")]
    public object Status()
    {
        return $"Plugin Controller v 1.0 {ps.Status()}";
    }

    [HttpGet("Produce")]
    public object Produce()
    {
        return $"Plugin Controller v 1.0 {ps.Produce()}";
    }

    [HttpGet("ConsumeStart")]
    public object ConsumeStart()
    {
        return $"Plugin Controller v 1.0 {ps.ConsumeStart()}";
    }

    [HttpGet("ConsumeStop")]
    public object ConsumeStop()
    {
        return $"Plugin Controller v 1.0 {ps.ConsumeStop()}";
    }
}