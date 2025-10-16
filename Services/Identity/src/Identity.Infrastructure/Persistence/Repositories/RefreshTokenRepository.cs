using Identity.Application.Common.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Authentication.TokenGenerator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Repositories;
    internal class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IdentityDbContext _db;
    private readonly JwtSettings _jwtSettings;
    public RefreshTokenRepository(IdentityDbContext db, IOptions<JwtSettings> jwtOptions) => (_db, _jwtSettings) = (db, jwtOptions.Value);

        public async Task AddAsync(Guid userId,string hashToken, CancellationToken ct = default)
        {

        var refreshtoken = new RefreshToken
        {
            UserId = userId,
            TokenHash = hashToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
            LastUsedAt = DateTime.UtcNow
            
        };
        await _db.RefreshTokens.AddAsync(refreshtoken, ct);
        await _db.SaveChangesAsync(ct);
    }

        public Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct = default) =>
            _db.RefreshTokens
               .Include(rt => rt.User)
               .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        public async Task RevokeAsync(Guid tokenId, CancellationToken ct = default)
        {
            var token = await _db.RefreshTokens.FindAsync(tokenId, ct);
            if (token is null) return;
            if (token.RevokedAt is null) token.RevokedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
    }

     
    }