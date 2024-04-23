using NotEnoughLogs;

namespace RefreshDiscordBot.Modules;

public class CurrentPlayersOnlineModule(Bot bot, Logger logger) : Module(bot, logger)
{
    protected override uint RunFrequency => 5000;

    public override Task Update()
    {
        return Task.CompletedTask;
    }
}