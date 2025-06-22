using System.Text.Json.Serialization;

namespace RefreshDiscordBot.Api.UptimeKuma;

#nullable disable

public class StatusPageHeartbeat
{
    [JsonPropertyName("heartbeatList")]
    public Dictionary<string, List<HeartbeatList>> HeartbeatList { get; set; }

    [JsonPropertyName("uptimeList")]
    public Dictionary<string, double> UptimeList { get; set; }
}