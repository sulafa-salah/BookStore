using Identity.Domain.Entities;
using Identity.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Configurations;
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("RefreshTokens");
        b.HasKey(x => x.Id);

        b.Property(x => x.TokenHash).HasMaxLength(200).IsRequired();
        b.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        b.HasIndex(x => new { x.UserId, x.TokenHash }).IsUnique();

        b.HasOne(rt => rt.User)
            .WithMany(rt => rt.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}