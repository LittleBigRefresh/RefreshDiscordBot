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
            new StatisticActivity(ActivityType.Watching, stats => $"{stats.ActiveUsers} players"),
            new StatisticActivity(ActivityType.Playing, stats => $"{stats.TotalLevels} levels"),
            new StatisticActivity(ActivityType.Playing, stats => $"with {stats.CurrentIngamePlayersCount} online players"),
            new StatisticActivity(ActivityType.CustomStatus, stats => $"{stats.TotalEvents} things have happened on Refresh"),
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
        ];
        
        return Task.CompletedTask;
    }

    public override async Task Update(CancellationToken ct)
    {
        Activity activity = this._activities[Random.Shared.Next(0, this._activities.Count)];
        await activity.SetActivity(this.Bot, ct);
        
        ct.ThrowIfCancellationRequested();
        
        this.Log(LogLevel.Info, $"Our activity is now '*{activity.Type}* {activity.Text}'");
        if (activity.Type == ActivityType.CustomStatus) 
            await this.Bot.Client.SetCustomStatusAsync(activity.Text);
        else
            await this.Bot.Client.SetActivityAsync(new Game(activity.Text, activity.Type));
    }

    private class StatisticActivity : Activity
    {
        private readonly Func<RefreshStatistics, string> _func;
        private RefreshStatistics _statistics;

        public override ActivityType Type { get; }
        public override string Text => _func(this._statistics);

        public override async Task SetActivity(Bot bot, CancellationToken ct)
        {
            this._statistics = await bot.Api.GetStatisticsAsync(ct);
        }

        public StatisticActivity(ActivityType type, Func<RefreshStatistics, string> func)
        {
            this._func = func;
            this.Type = type;
        }
    }

    private class Activity
    {
        public virtual string Text { get; } = null!;
        public virtual ActivityType Type { get; } = ActivityType.CustomStatus;
        
        public virtual Task SetActivity(Bot bot, CancellationToken ct) => Task.CompletedTask;
        
        protected Activity() {}

        internal Activity(ActivityType type, string text)
        {
            this.Text = text;
            this.Type = type;
        }
    }
}