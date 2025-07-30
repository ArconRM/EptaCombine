using EptaCombine.Entities.Utils;
using Microsoft.AspNetCore.Identity;

namespace EptaCombine.Extensions;

public static class RoleSeederExtensions
{
    public static async Task SeedRolesAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();

        var roles = new[] { UserRoles.User, UserRoles.Admin };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<long>(role));
            }
        }
    }
}