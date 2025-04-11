namespace ConstantTalk.Server.Settings
{
    public class RequestLoggingSettings
    {
        public string[] ExcludedPaths { get; set; } = Array.Empty<string>();
        public string LogFilePath { get; set; } = "Logs/requests_log.txt";
    }
}
