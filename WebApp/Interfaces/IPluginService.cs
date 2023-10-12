namespace Interfaces
{
    public interface IPluginService
    {
        string Test();
        string Status();
        string Produce();
        string ConsumeStart();
        string ConsumeStop();
    }
}