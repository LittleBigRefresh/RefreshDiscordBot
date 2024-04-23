namespace RefreshDiscordBot.Api.Types;

public class RefreshRoom
{
    public string RoomId { get; set; }
    public IEnumerable<RefreshRoomPlayerId> PlayerIds { get; set; }
}