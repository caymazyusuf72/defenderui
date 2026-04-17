namespace DefenderUI.Models;

public enum RiskLevel
{
    Low,
    Medium,
    High,
    Critical
}

public class ThreatInfo
{
    public string ThreatName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime DetectionDate { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public string ActionTaken { get; set; } = string.Empty;
    public bool IsQuarantined { get; set; }
}