using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevAppDeploy.Data.Tables;

public class ReleaseTbl
{
    [Key]
    public int Id { get; set; }
    public required string Version { get; set; }
    public DateTime ReleaseDate { get; set; }
    public required string Unique { get; set; }
    public string FileUrl { get; set; } = string.Empty;

    // Foreign key
    public int ApplicationId { get; set; }

    // Navigation property
    [ForeignKey("ApplicationId")]
    public required ApplicationTbl Application { get; set; }
}
