using Mamkin.In.Infrastructure.Ef;
using Mamkin.In.WebApi.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Mamkin.In.WebApi.Extensions;

public static class DbContextWebApplicationExtension
{
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
}