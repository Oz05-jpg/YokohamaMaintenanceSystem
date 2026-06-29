using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using YokohamaMaintenanceSystem.Data;

namespace YokohamaMaintenanceSystem.Tests
{
    // Test auth handler — bypass JWT สำหรับ integration test
    public class TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // ลบ DbContext options ทั้งหมด (SQL Server + configuration extensions)
                    var toRemove = services
                        .Where(d => d.ServiceType.Name.Contains("DbContextOptions"))
                        .ToList();
                    foreach (var d in toRemove)
                        services.Remove(d);

                    // เพิ่ม InMemory DB แทน
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("IntegrationTestDb"));
                });

                // bypass authorization — override policy ให้ pass ทุก request
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthorization(options =>
                    {
                        options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                            .RequireAssertion(_ => true)
                            .Build();
                        options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                            .RequireAssertion(_ => true)
                            .Build();
                    });
                });
            }).CreateClient();
        }

        [Fact]
        public async Task GetMaintenanceRequests_ReturnsOk()
        {
            // Act — route จริงคือ /api/maintenance
            var response = await _client.GetAsync("/api/maintenance");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
