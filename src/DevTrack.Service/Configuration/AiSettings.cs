namespace DevTrack.Service.Configuration;

public class AiSettings
{
    public const string SectionName = "Ai";
    public string BaseUrl { get; set; } = "http://host.docker.internal:1234/v1";
    public string Model { get; set; } = "llama-3.1-8b-instruct";
    public string ApiKey { get; set; } = "lm-studio";
    public int TimeoutSeconds { get; set; } = 180;
    public double Temperature { get; set; } = 0.2;
}
