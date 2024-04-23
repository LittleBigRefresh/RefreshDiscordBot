using Discord;
using Discord.WebSocket;
using NotEnoughLogs;
using RefreshDiscordBot.Api.Types;

namespace RefreshDiscordBot.Modules;

public class CurrentPlayersOnlineModule(Bot bot, Logger logger) : Module(bot, logger)
{
    // run once every 5 minutes (plus one second to offset potential rate limit)
    // unfortunately channel edits are pretty heavily rate limited
    // https://old.reddit.com/r/Discord_Bots/comments/qzrl5h/channel_name_edit_rate_limit/
    protected override uint RunFrequency => 301_000;
    private int _lastCount = -1;

    private const string Header = "Players Online: ";

    private SocketVoiceChannel _channel = null!;

    public override async Task Ready()
    {
        this._channel = (SocketVoiceChannel)await this.Bot.Client.GetChannelAsync(this.Bot.Config.PlayersOnlineChannel);
        SetCountFromChannelName();
    }

    private void SetCountFromChannelName()
    {
        try
        {
            ReadOnlySpan<char> countStr = this._channel.Name.AsSpan()[Header.Length..];
            this._lastCount = int.Parse(countStr);
            Log(LogLevel.Info, $"Recovered previous count from channel name: {this._lastCount}");
        }
        catch
        {
            // ignored
        }
    }

    public override async Task Update(CancellationToken ct)
    {
        RefreshStatistics statistics = await this.Bot.Api.GetStatisticsAsync(ct);
        int count = statistics.CurrentIngamePlayersCount;

        Log(LogLevel.Debug, $"{count} players online");

        ct.ThrowIfCancellationRequested();
        if (count == this._lastCount) return;
        this._lastCount = count;

        Log(LogLevel.Info, $"Updating player count channel to {count}");
        await this._channel.ModifyAsync(properties =>
        {
            properties.Name = Header + count;
        }, new RequestOptions
        {
            CancelToken = ct
        });
    }
}