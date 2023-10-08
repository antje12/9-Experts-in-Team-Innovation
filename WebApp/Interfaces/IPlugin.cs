using Microsoft.Extensions.DependencyInjection;

namespace Interfaces
{
    public interface IPlugin 
    {
        void Initialize(IServiceCollection services);
    }
}