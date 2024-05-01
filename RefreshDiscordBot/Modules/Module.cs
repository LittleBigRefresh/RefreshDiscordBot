using Discord.WebSocket;
using NotEnoughLogs;

namespace RefreshDiscordBot.Modules;

public abstract class Module
{
    protected virtual uint RunFrequency => 0;
    protected readonly Bot Bot;
    
    private readonly Logger _logger;
    private readonly string _name;
    private long _lastRan = 0;

    public virtual bool ShouldRun(long now)
    {
        if (now - this._lastRan > RunFrequency)
        {
            this._lastRan = now;
            return true;
        };
        return false;
    }

    protected Module(Bot bot, Logger logger)
    {
        this.Bot = bot;
        this._logger = logger;
        
        this._name = this.GetType().Name;
        this.Log(LogLevel.Debug, $"Initialized {this._name}");
    }

    protected void Log(LogLevel level, ReadOnlySpan<char> content)
    {
        this._logger.Log(level, this._name, content);
    }

    public virtual Task Ready()
    {
        return Task.CompletedTask;
    }

    public virtual Task Update(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnUserMessage(SocketUserMessage message, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}