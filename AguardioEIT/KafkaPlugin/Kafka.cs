using Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaPlugin;

public class Kafka : IPlugin
{
    public void Initialize(IServiceCollection services)
    {
        services.AddSingleton<KafkaService>();
    }
}