using System.Text.Json;
using NotEnoughLogs;
using NotEnoughLogs.Behaviour;
using NotEnoughLogs.Sinks;
using RefreshDiscordBot;
using RefreshDiscordBot.Configuration;

Logger logger = new([new ConsoleSink()], new LoggerConfiguration
{
#if DEBUG
    MaxLevel = LogLevel.Trace,
#else
    MaxLevel = LogLevel.Info
#endif
    Behaviour = new DirectLoggingBehaviour(),
});

BotConfiguration? config = null;

if (File.Exists("config.json"))
{
    config = JsonSerializer.Deserialize<BotConfiguration>(File.ReadAllText("config.json"));
}
else
{
    string json = JsonSerializer.Serialize(new BotConfiguration(), new JsonSerializerOptions
    {
        WriteIndented = true,
    });
    
    File.WriteAllText("config.json", json);
    logger.LogError("Configuration", "You need to have a configuration. An empty one was generated for you.");
    Environment.Exit(1);
}

if (config == null)
    throw new Exception("config failed to load");

using Bot bot = new(logger, config);

await bot.Start();
await Task.Delay(-1);