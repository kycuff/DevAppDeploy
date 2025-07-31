using DevAppDeploy.Data;
using DevAppDeploy.Data.Tables;
using DevAppDeploy.Models;
using Microsoft.EntityFrameworkCore;

namespace DevAppDeploy.Services.Application;

public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _context;

    public ApplicationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateApplicationAsync(CreateApplicationModel model)
    {
        var application = new ApplicationTbl
        {
            ApplicationName = model.AppName,
            OperatingSystem = model.OperatingSystem.ToString(),
            EnvironmentName = model.Environments.ToString()
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();
    }

    public async Task<List<DisplayApplicationModel>> GetApplicationsAsync()
    {
        return await _context.Applications
            .Select(a => new DisplayApplicationModel
            {
                AppName = a.ApplicationName,
                OperatingSystem = a.OperatingSystem,
                Environments = a.EnvironmentName,
                Id = a.Id
            })
            .ToListAsync();
    }

    public async Task<List<DisplayReleaseModel>> GetReleasesByApplicationId(int applicationId)
    {
        return await _context.Releases
            .Where(r => r.ApplicationId == applicationId)
            .Select(r => new DisplayReleaseModel
            {
                Id = r.Id,
                Version = r.Version,
                ReleaseDate = r.ReleaseDate,
                FileURL = r.FileUrl
            })
            .ToListAsync();
    }

    public async Task CreateReleaseAsync(CreateReleaseModel model)
    {
        var application = await _context.Applications.FindAsync(model.ApplicationId);
        if (application == null)
        {
            throw new ArgumentException("Invalid ApplicationId");
        }

        var release = new ReleaseTbl
        {
            Version = model.Version,
            ReleaseDate = model.ReleaseDate,
            Unique = model.Unique,
            ApplicationId = application.Id,
            Application = application,
            FileUrl = model.FileURL
        };

        _context.Releases.Add(release);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReleaseByIdAsync(int releaseId)
    {
        var release = await _context.Releases.FindAsync(releaseId);
        if (release != null)
        {
            _context.Releases.Remove(release);
            await _context.SaveChangesAsync();
        }
    }
}
