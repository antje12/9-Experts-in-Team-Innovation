namespace api.Logging;

public static class ExperimentLogger
{
    private static readonly string _logFilePath = Environment.CurrentDirectory + $@"/log.txt";
    public static async Task LogAsync(long message)
    {
        await using StreamWriter writer = File.AppendText(_logFilePath);
        await writer.WriteLineAsync($"{message}");
    }
}