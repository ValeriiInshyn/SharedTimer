using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using shared_timer_api.Features.Auth;
using shared_timer_api.Features.Timers.TimerSync;

namespace shared_timer_api.Features.Timers.Stop;

public class TimerStopEndpoint : Endpoint<TimerStopDto, Ok>
{
    private readonly IHubContext<TimerSyncHub> _hubContext;

    public TimerStopEndpoint(IHubContext<TimerSyncHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public override void Configure()
    {
        Post("timer/stop");
        Description(d => d
            .WithName("Stop Timer")
            .WithTags("Timers")
            .WithSummary("Stop a timer for a group"));
    }

    public override async Task<Ok> ExecuteAsync(TimerStopDto req, CancellationToken ct)
    {
        Guid ownerId = User.GetUserId();
        string groupKey = $"{req.GroupName}:{ownerId}";

        var message = new TimerSyncMessage
        {
            Action = TimerAction.Stop,
            GroupName = req.GroupName,
            GroupOwnerId = ownerId,
            EndTime = req.EndTime,
            InitiatedByUserId = ownerId.ToString()
        };

        await _hubContext.Clients.Group(groupKey).SendAsync("ReceiveTimerAction", message, ct);
        return TypedResults.Ok();
    }
}
