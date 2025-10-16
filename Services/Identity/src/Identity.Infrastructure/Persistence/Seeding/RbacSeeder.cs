using BookStore.Permissions;                  
using Identity.Domain.Entities;
using Identity.Domain.UserAggregate;    
using Identity.Infrastructure.Persistence;     
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Identity.Infrastructure.Persistence.Seeding;

public sealed class RbacSeeder : IHostedService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<RbacSeeder> _logger;

    public RbacSeeder(IServiceProvider sp, ILogger<RbacSeeder> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await RunAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RBAC seeding failed.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  

    public async Task TriggerAsync(CancellationToken cancellationToken = default)
        => await RunAsync(cancellationToken);

    // ----------------- core logic -----------------

    private async Task RunAsync(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        // Apply migrations first.
        await db.Database.MigrateAsync(ct);

        await EnsureRolesAndPermissionsAsync(roleMgr, ct);
        await EnsureAdminUserAsync(userMgr, roleMgr, config, ct);

        _logger.LogInformation("RBAC seeding finished.");
    }

    private static async Task EnsureRolesAndPermissionsAsync(
        RoleManager<ApplicationRole> roleMgr,
        CancellationToken ct)
    {
        foreach (var (roleName, perms) in RolePermissions.Map)
        {
            // 1) Ensure role exists
            var role = await roleMgr.FindByNameAsync(roleName);
            if (role is null)
            {
                role = new ApplicationRole
                {
                    Name = roleName,
                    Description = $"System role: {roleName}",
                   
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                var created = await roleMgr.CreateAsync(role);
                if (!created.Succeeded)
                    throw new InvalidOperationException(
                        $"Failed to create role '{roleName}': {string.Join(", ", created.Errors.Select(e => e.Description))}");
            }

            // 2) Add only missing permission claims (idempotent)
            var existingClaims = await roleMgr.GetClaimsAsync(role);
            var havePerms = existingClaims
                .Where(c => c.Type == Permissions.ClaimType)
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var perm in perms.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!havePerms.Contains(perm))
                {
                    var res = await roleMgr.AddClaimAsync(role, new Claim(Permissions.ClaimType, perm));
                    if (!res.Succeeded)
                        throw new InvalidOperationException(
                            $"Failed adding perm '{perm}' to role '{roleName}': {string.Join(", ", res.Errors.Select(e => e.Description))}");
                }
            }
        }
    }

    private static async Task EnsureAdminUserAsync(
        UserManager<ApplicationUser> userMgr,
        RoleManager<ApplicationRole> roleMgr,
        IConfiguration config,
        CancellationToken ct)
    {
        var adminEmail = config["AdminUser:Email"] ?? "admin@bookstore.local";
        var adminPass = config["AdminUser:Password"] ?? "Change_me_123!";

        var user = await userMgr.FindByEmailAsync(adminEmail);
        if (user is null)
        {
            
            user = ApplicationUser.Create("System", "Admin", adminEmail).Value;

            var create = await userMgr.CreateAsync(user, adminPass);
            if (!create.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to create initial admin user: {string.Join(", ", create.Errors.Select(e => e.Description))}");
        }

    
        var adminRole = await roleMgr.FindByNameAsync(RolePermissions.Admin)
                        ?? throw new InvalidOperationException("Admin role not found during seeding.");

        if (!await userMgr.IsInRoleAsync(user, RolePermissions.Admin))
        {
            var add = await userMgr.AddToRoleAsync(user, RolePermissions.Admin);
            if (!add.Succeeded)
                throw new InvalidOperationException(
                    $"Failed adding admin user to '{RolePermissions.Admin}' role: {string.Join(", ", add.Errors.Select(e => e.Description))}");
        }
    }
}