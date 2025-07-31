namespace DevAppDeploy.Models;

public class DisplayApplicationModel
{
    public int Id { get; set; }
    public required string AppName { get; set; }

    public required string OperatingSystem { get; set; }
    
    public required string Environments { get; set; }
}
