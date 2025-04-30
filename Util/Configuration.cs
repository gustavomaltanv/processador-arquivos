namespace ExampleProcessor.Util;

public class Configuration
{
    private static readonly Lazy<Configuration> _instance = new(() => new Configuration());
    private readonly IConfigurationRoot _configuration;

    private Configuration()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static Configuration Instance => _instance.Value;

    public string GetValue(string key)
    {
        return _configuration[key] ?? throw new KeyNotFoundException($"Key '{key}' not found in configuration.");
    }

    public T GetValue<T>(string key)
    {
        return _configuration.GetValue<T>(key);
    }
}