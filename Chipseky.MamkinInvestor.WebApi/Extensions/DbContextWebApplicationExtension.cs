using Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Chipseky.MamkinInvestor.WebApi.Extensions;

public static class DbContextWebApplicationExtension
{
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
}