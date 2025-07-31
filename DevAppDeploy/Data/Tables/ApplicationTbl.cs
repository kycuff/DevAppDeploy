using System.ComponentModel.DataAnnotations;

namespace DevAppDeploy.Data.Tables;

public class ApplicationTbl
{
    [Key]
    public int Id { get; set; }
    public required string ApplicationName { get; set; }
    public required string OperatingSystem { get; set; }
    public required string EnvironmentName { get; set; }
}
