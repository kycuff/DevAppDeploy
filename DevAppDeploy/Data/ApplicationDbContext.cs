using DevAppDeploy.Data.Tables;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevAppDeploy.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationTbl> Applications { get; set; }
    public DbSet<ReleaseTbl> Releases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the relationship between ReleaseTbl and ApplicationTbl
        modelBuilder.Entity<ReleaseTbl>()
            .HasOne(r => r.Application)
            .WithMany()
            .HasForeignKey(r => r.ApplicationId);
    }
}
