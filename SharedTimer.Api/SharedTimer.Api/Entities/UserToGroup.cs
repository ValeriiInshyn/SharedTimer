using System.ComponentModel.DataAnnotations.Schema;

namespace shared_timer_api.Entities;

public class UserToGroup
{
    public Guid UserId { get; set; }

    public required string GroupName { get; set; }

    public Guid GroupOwnerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual required User User { get; set; }
    public virtual required Group Group { get; set; }
}
