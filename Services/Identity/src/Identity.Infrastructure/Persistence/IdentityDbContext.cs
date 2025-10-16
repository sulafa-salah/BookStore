using Identity.Domain.Entities;
using Identity.Domain.UserAggregate;
using Identity.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence;
    public class IdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }
        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder b)
        {
        b.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(b);



            b.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            b.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            b.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            b.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            b.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        
            b.ApplyConfiguration(new ApplicationUserConfiguration());
            b.ApplyConfiguration(new ApplicationRoleConfiguration());
            b.ApplyConfiguration(new RefreshTokenConfiguration());

            
        }
    }


