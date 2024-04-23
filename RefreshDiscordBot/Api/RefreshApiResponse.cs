namespace RefreshDiscordBot.Api.Types;

public class RefreshApiResponse<TType>
{
    public bool Success { get; set; }
    public TType? Data { get; set; }
}