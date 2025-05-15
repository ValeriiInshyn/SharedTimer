namespace shared_timer_api.Features.Timers.Start;

public sealed record TimerStartDto
{
    public required string GroupName { get; set; }
    public required DateTime StartTime { get; set; }
}
