namespace shared_timer_api.Features.Groups.SetActive;

public sealed record GroupSetActiveDto
{
    public required string GroupName { get; set; }
}
