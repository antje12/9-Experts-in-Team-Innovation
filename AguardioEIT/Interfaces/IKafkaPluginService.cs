namespace Interfaces;

public interface IKafkaPluginService
{ 
    string Test(); 
    string Status(); 
    string Produce(); 
    string ConsumeStart(); 
    string ConsumeStop(); 
}