using System.Net.Http.Json;
using NotEnoughLogs;
using RefreshDiscordBot.Api.UptimeKuma;

namespace RefreshDiscordBot.Modules;

public class ServerStatusModule : VoiceChannelTextModule
{
    private readonly HttpClient _client = new(new SocketsHttpHandler
    {
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
        PooledConnectionLifetime = TimeSpan.FromMinutes(1),
    });
    
    public ServerStatusModule(Bot bot, Logger logger) : base(bot.Config.ServerStatusChannel, bot, logger)
    {
        this._client.BaseAddress = new Uri(bot.Config.UptimeKumaUrl);
    }

    protected override string Header => "Server Status: ";
    public override async Task<string> GetString(CancellationToken ct)
    {
        StatusPageHeartbeat? heartbeat = await this._client.GetFromJsonAsync<StatusPageHeartbeat>("/api/status-page/heartbeat/bonsai", cancellationToken: ct);
        
        if (heartbeat == null)
            return "Unknown";

        List<List<HeartbeatList>> lists = heartbeat.HeartbeatList.Values.ToList();
        if (lists.All(l => (l.LastOrDefault()?.Status ?? StatusType.Unknown) is StatusType.Up or StatusType.Pending))
            return "OK";
        
        if (lists.Any(l => (l.LastOrDefault()?.Status ?? StatusType.Unknown) == StatusType.Maintenance))
            return "🔧 Under Maintenance";
        
        if (lists.All(l => (l.LastOrDefault()?.Status ?? StatusType.Unknown) == StatusType.Down))
            return "❌ Down";
        
        return "⚠️ Degraded";
    }
}