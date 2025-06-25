namespace RefreshDiscordBot.Api.UptimeKuma;

public enum StatusType : byte
{
    Down = 0,
    Up = 1,
    Pending = 2,
    Maintenance = 3,
    Unknown = 255,
}