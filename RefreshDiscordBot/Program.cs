using System.Text.Json;
using NotEnoughLogs;
using NotEnoughLogs.Behaviour;
using NotEnoughLogs.Sinks;
using RefreshDiscordBot;
using RefreshDiscordBot.Configuration;

Logger logger = new([new ConsoleSink()], new LoggerConfiguration
{
    Behaviour = new DirectLoggingBehaviour(),
#if DEBUG
    MaxLevel = LogLevel.Trace,
#else
    MaxLevel = LogLevel.Info,
#endif
});

BotConfiguration? config = null;

if (bool.TryParse(Environment.GetEnvironmentVariable("RDB_USE_ENV"), out bool useEnv) && useEnv)
{
    config = new BotConfiguration()
    {
        DiscordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? throw new Exception("DISCORD_TOKEN was not provided"),
        ApiBaseUrl = Environment.GetEnvironmentVariable("RDB_API_BASE_URL") ?? throw new Exception("RDB_API_BASE_URL was not provided"),
        WebUrl = Environment.GetEnvironmentVariable("RDB_WEB_URL") ?? throw new Exception("RDB_WEB_URL was not provided"),
        UptimeKumaUrl = Environment.GetEnvironmentVariable("RDB_UPTIME_KUMA_URL") ?? "",
        PlayersOnlineChannel = ulong.Parse(Environment.GetEnvironmentVariable("RDB_PLAYERS_ONLINE_CHANNEL") ?? throw new Exception("RDB_PLAYERS_ONLINE_CHANNEL was not provided")),
        ServerStatusChannel = ulong.Parse(Environment.GetEnvironmentVariable("RDB_SERVER_STATUS_CHANNEL") ?? "0"),
    };
}
else if (File.Exists("config.json"))
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