using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BlogApi.Infrastructure;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;

namespace BlogApi.Tests.Integration;

public class TestHost : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set test environment first - this prevents SQLite registration in Program.cs
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // 1. Add InMemory database (SQLite won't be registered due to Testing environment)
            // Shared database for all integration tests - simpler and more realistic
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("SharedBlogApiTestDb");
            });

            // 2. Replace real JWT authentication with fake authentication
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>(
                    "Test", options => { });

            // 3. Disable logging noise in tests (optional)  
            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        });
    }
}

/// <summary>
/// Fake authentication handler for testing protected endpoints
/// </summary>
public class FakeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public FakeAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check if this is a test request with fake auth headers
        if (!Request.Headers.ContainsKey("X-Test-UserId"))
        {
            // No fake auth headers = treat as unauthenticated (for public endpoint tests)
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        // Extract user info from test headers
        var userId = Request.Headers["X-Test-UserId"].FirstOrDefault();
        var email = Request.Headers["X-Test-Email"].FirstOrDefault() ?? "test@example.com";
        var name = Request.Headers["X-Test-Name"].FirstOrDefault() ?? "Test User";

        // Create fake claims (same structure as real JWT claims)
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId!),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, name)
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}