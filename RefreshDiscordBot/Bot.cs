using System.Diagnostics;
using Discord;
using Discord.WebSocket;
using NotEnoughLogs;
using RefreshDiscordBot.Api;
using RefreshDiscordBot.Configuration;
using RefreshDiscordBot.Modules;

namespace RefreshDiscordBot;

public class Bot : IDisposable
{
    public readonly BotConfiguration Config;
    public readonly DiscordSocketClient Client;
    public readonly RefreshApi Api;

    private readonly Logger _logger;
    private readonly List<Module> _modules = [];

    public Bot(Logger logger, BotConfiguration config)
    {
        this.Config = config;
        this._logger = logger;
        
        DiscordSocketClient client = new(new DiscordSocketConfig
        {
            AlwaysDownloadUsers = false,
            AlwaysDownloadDefaultStickers = false,
            GatewayIntents = GatewayIntents.Guilds,
            LogLevel = LogSeverity.Verbose
        });

        client.Log += OnLog;
        client.Ready += OnReady;

        this.Client = client;
        this.Api = new RefreshApi(this._logger, config.ApiBaseUrl);
        
        this._logger.LogInfo("Bot", "Initialized bot");
        
        this._modules.Add(new CurrentPlayersOnlineModule(this, this._logger));
    }

    private Task OnLog(LogMessage message)
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
        await this.Client.LoginAsync(TokenType.Bot, this.Config.DiscordToken);
        await this.Client.StartAsync();
    }

    private async Task OnReady()
    {
        this._logger.LogInfo("Bot", "Bot is ready");
        foreach (Module module in _modules)
            await module.Ready();

        this.StartUpdateThread();
    }

    private void StartUpdateThread()
    {
        Thread thread = new(() => UpdateThread().Wait());
        thread.Start();
    }

    private async Task UpdateThread()
    {
        this._logger.LogTrace("Update", "Starting update thread");
        while (this.Client.LoginState == LoginState.LoggedIn)
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            
            try
            {
                foreach (Module module in _modules)
                {
                    await ProcessModule(module, now);
                }
            }
            catch(Exception e)
            {
                this._logger.LogError("Update", e.ToString());
            }

            await Task.Delay(10);
        }
        this._logger.LogWarning("Update", "Update thread stopping");
    }
    
    private async Task ProcessModule(Module module, long now)
    {
        if (!module.ShouldRun(now)) return;
        
        CancellationTokenSource cts = new(5_000);
        CancellationToken ct = cts.Token;
        
        await module.Update(ct);
        ct.ThrowIfCancellationRequested();
    }

    public void Dispose()
    {
        this.Client.Dispose();
        GC.SuppressFinalize(this);
    }
}