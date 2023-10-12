namespace Interfaces;

public interface IHDFS_Service
{
    Task GetFile(string name);
    Task SaveFile(string name);
}