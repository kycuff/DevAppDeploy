using DevAppDeploy.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace DevAppDeploy.Models;

public class CreateApplicationModel
{
    [Required]
    public string AppName { get; set; } = string.Empty;
    [Required]
    public OperatingSystemEnum? OperatingSystem { get; set; }
    [Required]
    public EnvironmentEnum? Environments { get; set; }
}
