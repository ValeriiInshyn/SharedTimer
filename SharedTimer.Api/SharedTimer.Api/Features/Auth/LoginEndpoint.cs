using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using shared_timer_api.Database;
using shared_timer_api.Entities;

namespace shared_timer_api.Features.Auth;

public class LoginEndpoint : Endpoint<LoginDto, LoginResponse>
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public LoginEndpoint(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public override void Configure()
    {
        Post("auth/login");
        AllowAnonymous();
        Description(d => d
            .WithName("Login")
            .WithTags("Authentication")
            .WithSummary("Login with a username to get a JWT token"));
    }

    public override async Task<LoginResponse> ExecuteAsync(LoginDto req, CancellationToken ct)
    {
        User? user = await _context.Users.FindAsync(req.UserId);

        if (user == null)
        {
            user = new User
            {
                UserId = Guid.NewGuid(),
                DisplayName = req.Username
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);
        }

        string token = _tokenService.GenerateJwtToken(user);

        return new LoginResponse
        {
            Token = token,
            UserId = user.UserId.ToString(),
            Username = user.DisplayName
        };
    }
}

public class LoginResponse
{
    public required string Token { get; set; }
    public required string UserId { get; set; }
    public string? Username { get; set; }
}