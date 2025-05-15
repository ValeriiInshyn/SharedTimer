using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using shared_timer_api.Database;
using shared_timer_api.Features.Auth;
using shared_timer_api.Features.Timers.TimerSync;
using System.Text;
using Microsoft.Extensions.Primitives;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddFastEndpoints();
builder.Services.AddCors();
builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configure JWT Auth using FastEndpoints
builder.Services.AddAuthenticationJwtBearer(o1 =>
    {
        o1.SigningKey = builder.Configuration["JWT:Secret"]!;

    },
    o2 =>
    {
        o2.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                StringValues accessToken = context.Request.Query["access_token"];
                PathString path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/timerSync"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

// Register TokenService
builder.Services.AddScoped<TokenService>();

WebApplication app = builder.Build();

app.UseCors(options =>
{
    options.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
});
app.MapHub<TimerSyncHub>("/timerSync");

await app.RunAsync();
