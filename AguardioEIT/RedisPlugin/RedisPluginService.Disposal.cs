namespace RedisPlugin;

/**
 * This partial class handles the disposal of the RedisPluginService.
 * This is just an implementation of the Dispose pattern:
 * https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/dispose-pattern
 */
public sealed partial class RedisPluginService : IDisposable
{
    private bool _disposed;

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _redis.Dispose();
        }

        _disposed = true;
    }
    
    ~RedisPluginService()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        _redis.Dispose();
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
