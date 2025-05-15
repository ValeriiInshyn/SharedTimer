using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using shared_timer_api.Features.Auth;
using shared_timer_api.Features.Timers.TimerSync;

namespace shared_timer_api.Features.Timers.Start;

public sealed class TimerStartEndpoint : Endpoint<TimerStartDto, Ok>
{
    private readonly IHubContext<TimerSyncHub> _hubContext;

    public TimerStartEndpoint(IHubContext<TimerSyncHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public override void Configure()
    {
        Post("timer/start");
        Description(d => d
            .WithName("Start Timer")
            .WithTags("Timers")
            .WithSummary("Start a timer for a group"));
    }

    public override async Task<Ok> ExecuteAsync(TimerStartDto req, CancellationToken ct)
    {
        Guid ownerId = User.GetUserId();
        string groupKey = $"{req.GroupName}:{ownerId}";

        var message = new TimerSyncMessage
        {
            Action = TimerAction.Start,
            GroupName = req.GroupName,
            GroupOwnerId = ownerId,
            StartTime = req.StartTime,
            EndTime = null,
            InitiatedByUserId = ownerId.ToString()
        };

        await _hubContext.Clients.Group(groupKey).SendAsync("ReceiveTimerAction", message, ct);
        return TypedResults.Ok();
    }
}
