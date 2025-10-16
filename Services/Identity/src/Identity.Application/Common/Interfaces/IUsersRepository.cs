using ErrorOr;
using Identity.Domain.Entities;
using Identity.Domain.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Common.Interfaces;
    public interface IUsersRepository
    {
    Task<ErrorOr<ApplicationUser>> AddUserAsync(ApplicationUser user, string password);
        Task<bool> ExistsByEmailAsync(string email);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser?> GetByIdAsync(Guid userId);
        Task UpdateAsync(ApplicationUser user);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
   
   
   
}
