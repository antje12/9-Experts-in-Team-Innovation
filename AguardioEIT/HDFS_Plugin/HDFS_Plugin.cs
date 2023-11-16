using Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HDFS_Plugin;

public class HDFS_Plugin : IPlugin
{
    public void Initialize(IServiceCollection services)
    {
        services.AddSingleton<IHDFS_Service, HDFS_Service>();
    }
}