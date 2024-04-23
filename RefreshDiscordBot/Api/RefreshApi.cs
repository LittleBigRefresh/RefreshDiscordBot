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