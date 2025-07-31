using DevAppDeploy.Data;
using Microsoft.AspNetCore.Identity;

namespace DevAppDeploy.Services.Roles;

public class RoleAssignmentService : IRoleAssignmentService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleAssignmentService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task AssignRoleAsync(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        // Remove all existing roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
        }

        // Assign the new role
        await _userManager.AddToRoleAsync(user, newRole);
    }
}
