namespace DevAppDeploy.Models;

public class DisplayReleaseModel
{
    public int Id { get; set; }
    public required string Version { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? FileURL { get; set; }
}
