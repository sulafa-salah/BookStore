using ErrorOr;
using Identity.Application.Common.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Repositories;
    public class UsersRepository : IUsersRepository
    {
        private readonly IdentityDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    public UsersRepository(IdentityDbContext dbContext,UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
        _userManager = userManager;
        }

        public async Task<ErrorOr<ApplicationUser>> AddUserAsync(ApplicationUser user,string password)
        {
      
        if (string.IsNullOrWhiteSpace(user.Email))
            return UserErrors.InvalidEmail;
     
        var existing = await _userManager.FindByEmailAsync(user.Email);
        if (existing is not null)
            return UserErrors.EmailAlreadyUsed;
        var result = await _userManager.CreateAsync(user,password);
        if (!result.Succeeded)
        {
          
            return result.Errors
                .Select(e => Error.Validation(code: $"Identity.{e.Code}", description: e.Description))
                .ToList();

        }

        await _dbContext.SaveChangesAsync();
        return user;
        }

   

    public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null ? true: false;
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> GetByIdAsync(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
        await _userManager.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();
        }

    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
       return await _userManager.CheckPasswordAsync(user, password);
    }

}