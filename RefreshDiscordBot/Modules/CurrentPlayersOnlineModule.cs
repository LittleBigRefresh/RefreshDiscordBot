using NotEnoughLogs;

namespace RefreshDiscordBot.Modules;

public class CurrentPlayersOnlineModule(Bot bot, Logger logger) : Module(bot, logger)
{
    public override uint RunFrequency => 5000;

    public override Task Update()
    {
        Log(LogLevel.Debug, $"Update at {this.LastRan}");
        return Task.CompletedTask;
    }
}