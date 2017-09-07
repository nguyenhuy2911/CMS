﻿using System.Security.Claims;
using System.Threading.Tasks;

namespace Orchard.Users.Services
{
    public interface IMembershipService
    {
        Task<IUser> GetUserAsync(string userName);
        Task<bool> CheckPasswordAsync(string userName, string password);
        Task<ClaimsPrincipal> CreateClaimsPrincipal(IUser user);
    }
}
