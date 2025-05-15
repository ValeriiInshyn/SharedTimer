using System.Security.Claims;

namespace shared_timer_api.Features.Auth;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        if (principal == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }
        
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid guidUserId))
        {
            throw new UnauthorizedAccessException("Invalid or missing user ID in token");
        }
        
        return guidUserId;
    }
} 