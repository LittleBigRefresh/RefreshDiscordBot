namespace RefreshDiscordBot.Api.Refresh.Types;

public class RefreshStatistics
{
    public int TotalLevels { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalPhotos { get; set; }
    public int TotalEvents { get; set; }
    public int CurrentRoomCount { get; set; }
    public int CurrentIngamePlayersCount { get; set; }
}