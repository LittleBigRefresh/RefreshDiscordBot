using Discord;
using Discord.WebSocket;
using NotEnoughLogs;
using RefreshDiscordBot.Api;
using RefreshDiscordBot.Configuration;

namespace RefreshDiscordBot;

public class Bot : IDisposable
{
    private readonly BotConfiguration _config;
    private readonly Logger _logger;
    
    private readonly DiscordSocketClient _client;
    private readonly RefreshApi _api;

    public Bot(Logger logger, BotConfiguration config)
    {
        this._config = config;
        this._logger = logger;
        
        DiscordSocketClient client = new(new DiscordSocketConfig
        {
            AlwaysDownloadUsers = false,
            AlwaysDownloadDefaultStickers = false,
            GatewayIntents = GatewayIntents.Guilds,
            LogLevel = LogSeverity.Verbose
        });

        client.Log += Log;

        this._client = client;
        this._api = new RefreshApi(this._logger, config.ApiBaseUrl);
    }

    private Task Log(LogMessage message)
    {
        LogLevel level = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Info,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => throw new ArgumentOutOfRangeException(nameof(message))
        };

        if (message.Exception != null)
        {
            this._logger.Log(level, message.Source, message.Exception.ToString());
        }
        
        this._logger.Log(level, message.Source, message.Message);
        return Task.CompletedTask;
    }

    public async Task Start()
    {
        await this._client.LoginAsync(TokenType.Bot, this._config.DiscordToken);
        await this._client.StartAsync();
    }

    public void Dispose()
    {
        this._client.Dispose();
        GC.SuppressFinalize(this);
    }
}