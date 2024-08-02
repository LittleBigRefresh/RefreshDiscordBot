using Discord;
using NotEnoughLogs;
using RefreshDiscordBot.Api.Types;

namespace RefreshDiscordBot.Modules;

// port from moniku
public class RandomActivityModule(Bot bot, Logger logger) : Module(bot, logger)
{
    protected override uint RunFrequency => 120_000;
    private List<Activity> _activities = null!;

    public override Task Ready()
    {
        this._activities =
        [
            new RandomUserActivity(),
            new StatisticActivity(ActivityType.Watching, stats => $"{stats.ActiveUsers:N0} players"),
            new StatisticActivity(ActivityType.Playing, stats => $"{stats.TotalLevels:N0} levels"),
            new StatisticActivity(ActivityType.Playing, stats => $"with {stats.CurrentIngamePlayersCount:N0} online players"),
            new StatisticActivity(ActivityType.CustomStatus, stats => $"{stats.TotalEvents:N0} things have happened on Refresh"),
            new Activity(ActivityType.CustomStatus, "https://littlebigrefresh.com"),
            new Activity(ActivityType.CustomStatus, "Getting 100CR for a mod giveaway"),
            new Activity(ActivityType.Playing, "Clockworx 2 for the 1000th time"),
            new Activity(ActivityType.CustomStatus, "Sackboy gaming"),
            new Activity(ActivityType.Watching, "the 64"),
            new Activity(ActivityType.Watching, "big tc"),
            new Activity(ActivityType.Playing, "LittleBigPlanet 2"),
            new Activity(ActivityType.Playing, "LittleBigPlanet 4"), // lmao
            new Activity(ActivityType.Playing, "LittleBigPlanet PSP"),
            new Activity(ActivityType.Playing, "Toolkit"),
            new Activity(ActivityType.CustomStatus, "True..."),
            new Activity(ActivityType.CustomStatus, "False..."),
            new Activity(ActivityType.Playing, "GOOD horror levels"),
            new Activity(ActivityType.Playing, "terrible horror levels"),
            new Activity(ActivityType.CustomStatus, "Surely these statuses won't get boring or unfunny"),
            new Activity(ActivityType.CustomStatus, "fixed in the rewrite"),
            new Activity(ActivityType.CustomStatus, "#ful-memes revival arc when"),
            new Activity(ActivityType.Watching, "YOU, specifically"),
            new UncachedActivity(ActivityType.CustomStatus, () => $"PS4 support releases in {Random.Shared.Next(1, 100):N0} days"),
            new UncachedActivity(ActivityType.CustomStatus, () => $"PSP support will release in -{(DateTimeOffset.UtcNow - DateTimeOffset.Parse("11/11/2023 11:04:00 AM")).Days:N0}"),
        ];
        
        return Task.CompletedTask;
    }

    public override async Task Update(CancellationToken ct)
    {
        while (true)
        {
            Activity activity = this._activities[Random.Shared.Next(0, this._activities.Count)];
            bool success = await activity.SetActivity(this.Bot, ct);
            
            if(!success) continue;
        
            ct.ThrowIfCancellationRequested();
        
            this.Log(LogLevel.Info, $"Our activity is now '*{activity.Type}* {activity.Text}'");
            if (activity.Type == ActivityType.CustomStatus) 
                await this.Bot.Client.SetCustomStatusAsync(activity.Text);
            else
                await this.Bot.Client.SetActivityAsync(new Game(activity.Text, activity.Type));

            break;
        }
    }

    private class RandomUserActivity : Activity
    {
        private RefreshRoomPlayerId _player = null!;
        public override string Text => this._player.Username;
        public override ActivityType Type => ActivityType.Watching;

        public override async Task<bool> SetActivity(Bot bot, CancellationToken ct)
        {
            List<RefreshRoom> rooms = (await bot.Api.GetRoomListingAsync(ct)).ToList();
            if (rooms.Count < 1) return false;
            
            RefreshRoom room = rooms[Random.Shared.Next(0, rooms.Count)];
            List<RefreshRoomPlayerId> playerIds = room.PlayerIds.ToList();
            if (playerIds.Count < 1) return false; // if this ever gets hit i'll shit myself

            RefreshRoomPlayerId playerId = playerIds[Random.Shared.Next(0, playerIds.Count)];
            this._player = playerId;
            return true;
        }
    }

    private class StatisticActivity : Activity
    {
        private readonly Func<RefreshStatistics, string> _func;
        private RefreshStatistics _statistics;

        public override ActivityType Type { get; }
        public override string Text => _func(this._statistics);

        public override async Task<bool> SetActivity(Bot bot, CancellationToken ct)
        {
            this._statistics = await bot.Api.GetStatisticsAsync(ct);
            return true;
        }

        public StatisticActivity(ActivityType type, Func<RefreshStatistics, string> func)
        {
            this._func = func;
            this.Type = type;
        }
    }

    private class UncachedActivity : Activity
    {
        private readonly Func<string> _func;
        private readonly ActivityType _activityType;

        public override string Text => _func();
        public override ActivityType Type => _activityType;
        
        public UncachedActivity(ActivityType activityType, Func<string> func)
        {
            this._activityType = activityType;
            this._func = func;
        }
    }

    private class Activity
    {
        public virtual string Text { get; } = null!;
        public virtual ActivityType Type { get; } = ActivityType.CustomStatus;
        
        public virtual Task<bool> SetActivity(Bot bot, CancellationToken ct) => Task.FromResult(true);
        
        protected Activity() {}

        internal Activity(ActivityType type, string text)
        {
            this.Text = text;
            this.Type = type;
        }
    }
}