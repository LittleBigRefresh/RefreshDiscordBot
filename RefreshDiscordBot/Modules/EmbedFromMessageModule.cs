using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using NotEnoughLogs;
using RefreshDiscordBot.Api.Refresh.Types;

namespace RefreshDiscordBot.Modules;

public partial class EmbedFromMessageModule : Module
{
    public EmbedFromMessageModule(Bot bot, Logger logger) : base(bot, logger)
    {}

    [GeneratedRegex("u:[a-zA-Z0-9_-]{3,16}")]
    public static partial Regex UsernameRegex();
    
    public override async Task OnUserMessage(SocketUserMessage message, CancellationToken ct)
    {
        foreach (Match match in UsernameRegex().Matches(message.Content))
        {
            Log(LogLevel.Debug, match.Value);
            RefreshUser user = await this.Bot.Api.GetUserByUsername(match.Value[2..], ct);

            Embed embed = new EmbedBuilder()
                .WithTitle($"{user.Username}'s profile")
                .WithDescription(user.Description)
                .WithTimestamp(user.JoinDate)
                .WithUrl($"{Bot.Config.WebUrl}/u/{user.UserId}")
                .WithThumbnailUrl($"{Bot.Config.WebUrl}/api/v3/assets/{user.IconHash}/image")
                .WithColor(0x2A1936)
                .Build();
            
            await message.ReplyAsync(embed: embed);
        }
    }
}