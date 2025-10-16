using Identity.Domain.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities;
public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();  
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public ApplicationUser User { get; set; } = default!;

    public bool IsActive()
    {
        if (RevokedAt is not null) return false;
        if (DateTime.UtcNow > ExpiresAt) return false;

        return true;
    }

}
    
