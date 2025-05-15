using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using shared_timer_api.Database;
using shared_timer_api.Features.Auth;
using shared_timer_api.Features.Timers.TimerSync;
using Group = shared_timer_api.Entities.Group;

namespace shared_timer_api.Features.Groups.SetActive;

public class GroupSetActiveEndpoint : Endpoint<GroupSetActiveDto, Ok>
{
    private readonly AppDbContext _dbContext;
    private readonly IHubContext<TimerSyncHub> _hubContext;

    public GroupSetActiveEndpoint(AppDbContext dbContext, IHubContext<TimerSyncHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    public override void Configure()
    {
        Post("group/set-active");
        Description(d => d
            .WithName("Set Active Group")
            .WithTags("Groups")
            .WithSummary("Set a group as active"));
    }

    public override async Task<Ok> ExecuteAsync(GroupSetActiveDto req, CancellationToken ct)
    {
        Guid ownerId = User.GetUserId();

        Group? group = await _dbContext.Groups.FindAsync(new object[] { req.GroupName, ownerId }, ct);
        if (group == null)
        {
            ThrowError("Group not found");
        }

        string groupKey = $"{req.GroupName}:{ownerId}";
        await _hubContext.Clients.Group(groupKey).SendAsync("GroupActiveChanged", req.GroupName, ownerId, ct);

        return TypedResults.Ok();
    }
}
