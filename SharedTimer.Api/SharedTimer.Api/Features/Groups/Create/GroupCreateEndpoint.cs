using FastEndpoints;
using shared_timer_api.Database;
using shared_timer_api.Entities;
using shared_timer_api.Features.Auth;
using System.Security.Claims;
using Group = shared_timer_api.Entities.Group;

namespace shared_timer_api.Features.Groups.Create;

public sealed class GroupCreateEndpoint : Endpoint<GroupCreateDto, GroupCreateResponse>
{
    private readonly AppDbContext _context;

    public GroupCreateEndpoint(AppDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("groups");
        Description(d => d
            .WithName("Create Group")
            .WithTags("Groups")
            .WithSummary("Create a new group"));
    }

    public override async Task<GroupCreateResponse> ExecuteAsync(GroupCreateDto req, CancellationToken ct)
    {
        Guid ownerId = User.GetUserId();

        User? user = await _context.Users.FindAsync(new object[] { ownerId }, ct);
        if (user == null)
        {
            user = new User
            {
                UserId = ownerId,
                DisplayName = User.Identity?.Name
            };
            _context.Users.Add(user);
        }

        var group = new Group
        {
            Name = req.Name,
            OwnerId = ownerId,
            Owner = user
        };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync(ct);

        return new GroupCreateResponse
        {
            Name = group.Name,
            OwnerId = group.OwnerId
        };
    }
}

public class GroupCreateResponse
{
    public required string Name { get; set; }
    public Guid OwnerId { get; set; }
}
