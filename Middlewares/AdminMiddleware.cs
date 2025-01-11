using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SimpleTweetApi.Models.App;
using SimpleTweetApi.Models.Auth;
using SimpleTweetApi.Resources.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleTweetApi.Middlewares;

// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class AdminMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdminMiddleware> _logger;

    public AdminMiddleware(RequestDelegate next, ILogger<AdminMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var userManager = httpContext.RequestServices.GetRequiredService<UserManager<User>>();
        var user = await userManager.GetUserAsync(httpContext.User);

        _logger.LogInformation($"User: {user}");

        if (user == null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(new BaseResponse<Tweet>(
                Status: 401,
                Message: "User not authenticated",
                Data: null
                ));
            return;
        } else if (user.Email == "superadmin@mail.com") // TEMPORARY SOLUTION, USING ASP ROLE INSTEAD
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            await httpContext.Response.WriteAsJsonAsync(new BaseResponse<Tweet>(
                Status: 403,
                Message: "User not authorized",
                Data: null
                ));
            return;
        }

        await _next(httpContext);
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class AdminMiddlewareExtensions
{
    public static IApplicationBuilder UseAdminMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AdminMiddleware>();
    }
}
