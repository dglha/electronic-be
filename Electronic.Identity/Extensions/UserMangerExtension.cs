using System.Security.Claims;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Extensions;

public static class UserMangerExtension
{
    public static async Task<ApplicationUser> FindByEmailFromClaimsPrinciple(this UserManager<ApplicationUser> userManager, ClaimsPrincipal user)
    {
        var email = user.FindFirstValue(ClaimTypes.Email);
        return await userManager.Users.SingleOrDefaultAsync(x => x.Email == email);
    }
}