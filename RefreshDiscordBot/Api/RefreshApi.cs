using System.Net.Http.Json;
using System.Text.Json;
using NotEnoughLogs;
using RefreshDiscordBot.Api.Types;

namespace RefreshDiscordBot.Api;

public class RefreshApi : IDisposable
{
    private readonly Logger _logger;
    private readonly HttpClient _client;

    public RefreshApi(Logger logger, string baseUrl)
    {
        this._client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
        };

        this._logger = logger;
    }
    
    private async Task<TType> GetAsync<TType>(string endpoint, CancellationToken ct)
    {
        this.Log(LogLevel.Info, "GET " + endpoint);
        ct.ThrowIfCancellationRequested();
        
        RefreshApiResponse<TType>? response = await this._client.GetFromJsonAsync<RefreshApiResponse<TType>>(endpoint, ct);
        if (response == null) throw new Exception("Couldn't deserialize/gather a response from the server.");

        if (!response.Success)
            throw new NotImplementedException("error handling");

        return response.Data!;
    }

    public Task<RefreshStatistics> GetStatisticsAsync(CancellationToken ct)
    {
        return this.GetAsync<RefreshStatistics>("statistics", ct);
    }
    
    public Task<IEnumerable<RefreshRoom>> GetRoomListingAsync(CancellationToken ct)
    {
        return this.GetAsync<IEnumerable<RefreshRoom>>("rooms", ct);
    }

    private void Log(LogLevel level, ReadOnlySpan<char> content)
    {
        this._logger.Log(level, "API", content);
    }

    public void Dispose()
    {
        this._client.Dispose();
        GC.SuppressFinalize(this);
    }
}