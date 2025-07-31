using DevAppDeploy.Data.Tables;
using DevAppDeploy.Models;

namespace DevAppDeploy.Services.Application;

public interface IApplicationService
{
    Task CreateApplicationAsync(CreateApplicationModel model);
    Task<List<DisplayApplicationModel>> GetApplicationsAsync();
    Task<List<DisplayReleaseModel>> GetReleasesByApplicationId(int applicationId);
    Task CreateReleaseAsync(CreateReleaseModel model);
    Task DeleteReleaseByIdAsync(int releaseId);
}
