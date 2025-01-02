using Microsoft.EntityFrameworkCore;
using SimpleTweetApi.Database;

namespace SimpleTweetApi.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigration(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Database.Migrate();
    }
}
