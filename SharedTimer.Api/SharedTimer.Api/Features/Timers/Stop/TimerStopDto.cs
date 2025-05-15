namespace shared_timer_api.Features.Timers.Stop;

public class TimerStopDto
{
    public required string GroupName { get; set; }
    public required DateTime EndTime { get; set; }
}
