using Discord;
using Discord.WebSocket;
using NotEnoughLogs;
using RefreshDiscordBot.Api.Types;
using RefreshDiscordBot.Configuration;

namespace RefreshDiscordBot.Modules;

public class CurrentPlayersOnlineModule : VoiceChannelTextModule
{
    public CurrentPlayersOnlineModule(Bot bot, Logger logger) : base(bot.Config.PlayersOnlineChannel, bot, logger)
    {}

    protected override string Header => "Players Online: ";

    public override async Task<string> GetString(CancellationToken ct)
    {
        RefreshStatistics statistics = await this.Bot.Api.GetStatisticsAsync(ct);
        int count = statistics.CurrentIngamePlayersCount;

        Log(LogLevel.Debug, $"{count} players online");
        return count.ToString();
    }
}