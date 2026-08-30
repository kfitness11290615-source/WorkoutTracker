using System.Net.Http.Headers;
using System.Text;

namespace WorkoutTracker.Middleware;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public BasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var expectedUser = _configuration["BasicAuth:Username"];
        var expectedPass = _configuration["BasicAuth:Password"];

        // If no credentials are set, skip authentication
        if (string.IsNullOrEmpty(expectedUser) || string.IsNullOrEmpty(expectedPass))
        {
            await _next(context);
            return;
        }

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var username = credentials[0];
            var password = credentials[1];

            if (username == expectedUser && password == expectedPass)
            {
                await _next(context);
                return;
            }
        }
        catch
        {
            // Ignore parse exceptions and drop down to unauthorized
        }

        context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"WorkoutTracker\"";
        context.Response.StatusCode = 401; // Unauthorized
    }
}
