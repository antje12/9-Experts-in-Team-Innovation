namespace RedisPlugin.DTO;

public class SetCacheDto
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public int? ExpirationSeconds { get; set; }
}
