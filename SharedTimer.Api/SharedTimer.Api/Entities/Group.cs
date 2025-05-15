using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace shared_timer_api.Entities;

public class Group
{
    public required string Name { get; set; }

    public Guid OwnerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual required User Owner { get; set; }
    public virtual ICollection<UserToGroup> UserGroups { get; set; } = [];
}
