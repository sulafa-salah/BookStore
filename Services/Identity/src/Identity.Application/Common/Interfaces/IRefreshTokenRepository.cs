using Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Common.Interfaces;
    public interface IRefreshTokenRepository
    {
        Task AddAsync(Guid userId, string hashToken, CancellationToken ct = default);
        Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct = default);
        Task RevokeAsync(Guid tokenId, CancellationToken ct = default);

}