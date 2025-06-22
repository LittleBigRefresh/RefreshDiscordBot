using System.Text.Json.Serialization;

namespace RefreshDiscordBot.Api.UptimeKuma;

#nullable disable

public class HeartbeatList
{
    [JsonPropertyName("status")]
    public StatusType Status { get; set; }

    // [JsonPropertyName("time")]
    // public DateTimeOffset Time { get; set; }
    //
    // [JsonPropertyName("msg")]
    // public string Message { get; set; }
    //
    // [JsonPropertyName("ping")]
    // public float? Ping { get; set; }
}