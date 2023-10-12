using Interfaces;

namespace api.Service;

public class ApplicationService : IApplicationService
{
    public string Test()
    {
        return "Application Service Tested!";
    }
}