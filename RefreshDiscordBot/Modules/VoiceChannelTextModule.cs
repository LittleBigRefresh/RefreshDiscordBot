using Discord;
using Discord.WebSocket;
using NotEnoughLogs;

namespace RefreshDiscordBot.Modules;

public abstract class VoiceChannelTextModule : Module
{
    // run once every 5 minutes (plus one second to offset potential rate limit)
    // unfortunately channel edits are pretty heavily rate limited
    // https://old.reddit.com/r/Discord_Bots/comments/qzrl5h/channel_name_edit_rate_limit/
    protected override uint RunFrequency => 301_000;
    
    protected VoiceChannelTextModule(ulong channelId, Bot bot, Logger logger) : base(bot, logger)
    {
        this._channelId = channelId;
    }

    protected abstract string Header { get; }
    
    private SocketVoiceChannel _channel = null!;
    private string? _lastValue;

    private readonly ulong _channelId;
    
    public override async Task Ready()
    {
        this._channel = (SocketVoiceChannel)await this.Bot.Client.GetChannelAsync(this._channelId);
        
        try
        {
            ReadOnlySpan<char> countStr = this._channel.Name.AsSpan()[Header.Length..];
            this._lastValue = countStr.ToString();
            Log(LogLevel.Info, $"Recovered previous string from channel name: {this._lastValue}");
        }
        catch
        {
            // ignored
        }
    }

    public abstract Task<string> GetString(CancellationToken ct);
    
    public override async Task Update(CancellationToken ct)
    {
        string value = await GetString(ct);

        ct.ThrowIfCancellationRequested();
        if (value == this._lastValue) return;
        this._lastValue = value;

        Log(LogLevel.Info, $"Updating #{_channel.Name} channel to {value}");
        await this._channel.ModifyAsync(properties =>
        {
            properties.Name = Header + value;
        }, new RequestOptions
        {
            CancelToken = ct
        });
    }
}