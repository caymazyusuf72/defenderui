namespace DefenderUI.Models;

public class ProtectionModule
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public bool HasIssue { get; set; }
    public string IssueDescription { get; set; } = string.Empty;
}