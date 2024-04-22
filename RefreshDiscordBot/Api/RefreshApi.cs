using NotEnoughLogs;

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

    public void Dispose()
    {
        this._client.Dispose();
        GC.SuppressFinalize(this);
    }
}