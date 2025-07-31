using System.ComponentModel.DataAnnotations;

namespace DevAppDeploy.Models;

public class CreateReleaseModel
{
    [Required]
    public string Version { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;
    [Required]
    public string Unique { get; set; } = string.Empty;
    [Required]
    public string FileURL { get; set; } =string.Empty;
    
    public int ApplicationId { get; set; }
}
