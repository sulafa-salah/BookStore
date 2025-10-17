using Cart.Domain.Entities;
using System.Threading.Tasks;

namespace Cart.Application.Common.Interfaces;
    public interface ICartRepository
    {
        Task<ShoppingCart?> GetAsync(string userId, CancellationToken ct = default);
        Task SaveAsync(ShoppingCart cart, TimeSpan? ttl = null, CancellationToken ct = default);
        Task<bool> RemoveAsync(string userId, CancellationToken ct = default);
    }