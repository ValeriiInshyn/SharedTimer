using FastEndpoints.Security;
using shared_timer_api.Entities;
using System.Security.Claims;

namespace shared_timer_api.Features.Auth;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken(User user)
    {
        return JwtBearer.CreateToken(
            options =>
            {
                options.SigningKey = _configuration["JWT:Secret"]!;
                options.ExpireAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"]));
                options.User.Claims.Add(("sub", user.UserId.ToString()));
                options.User.Claims.Add(("name", user.DisplayName ?? "User"));
                options.User.Roles.Add(ERoles.User.ToString());
            });
    }

    public string GetUserIdFromToken(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException();
    }
}