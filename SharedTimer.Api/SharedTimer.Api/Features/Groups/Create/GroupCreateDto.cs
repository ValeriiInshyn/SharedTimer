namespace shared_timer_api.Features.Groups.Create;

public sealed record GroupCreateDto
{
    public required string Name { get; set; }
}
