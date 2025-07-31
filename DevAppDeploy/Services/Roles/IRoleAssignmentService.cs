namespace DevAppDeploy.Services.Roles;

public interface IRoleAssignmentService
{
    Task AssignRoleAsync(string userId, string role);
}
