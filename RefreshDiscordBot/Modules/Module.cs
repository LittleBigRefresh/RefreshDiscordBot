using NotEnoughLogs;

namespace RefreshDiscordBot.Modules;

public abstract class Module
{
    public virtual uint RunFrequency => 0;
    public long LastRan = 0;

    protected readonly Bot _bot;
    
    private readonly Logger _logger;
    private readonly string _name;

    public virtual bool ShouldRun(long now) => now - this.LastRan > RunFrequency;

    protected Module(Bot bot, Logger logger)
    {
        this._bot = bot;
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

    public virtual Task Update()
    {
        return Task.CompletedTask;
    }
}