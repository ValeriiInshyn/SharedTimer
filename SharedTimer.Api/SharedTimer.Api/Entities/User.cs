using System.ComponentModel.DataAnnotations;

namespace shared_timer_api.Entities;

public class User
{
    public Guid UserId { get; set; }

    public string? DisplayName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Group> OwnedGroups { get; set; } = [];
    public virtual ICollection<UserToGroup> UserGroups { get; set; } = [];
}