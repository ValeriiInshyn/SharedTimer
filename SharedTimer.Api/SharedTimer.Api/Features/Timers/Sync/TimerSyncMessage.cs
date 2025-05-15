namespace shared_timer_api.Features.Timers.TimerSync;

public enum TimerAction
{
    Start,
    Stop,
    CurrentState
}

public class TimerSyncMessage
{
    public required TimerAction Action { get; set; }
    public required string GroupName { get; set; }
    public required Guid GroupOwnerId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? InitiatedByUserId { get; set; }
} 