namespace RefreshDiscordBot.Configuration;

public class BotConfiguration
{
    public string ApiBaseUrl { get; set; } = "https://lbp.lbpbonsai.com/api/v3/";
    public string WebUrl { get; set; } = "https://lbp.lbpbonsai.com/";
    public string UptimeKumaUrl { get; set; } = "https://status.lbpbonsai.com";
    public string DiscordToken { get; set; } = "asdf";
    public ulong PlayersOnlineChannel { get; set; } = 1232216364329074699ul;
    public ulong ServerStatusChannel { get; set; } = 1386441065174663178ul;
}