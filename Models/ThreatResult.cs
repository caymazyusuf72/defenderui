using System;

namespace DefenderUI.Models;

public enum ThreatType
{
    Malware,
    PUA,          // Potentially Unwanted Application
    Adware,
    Ransomware,
    Trojan,
    HeuristicSuspicious // Sezgisel olarak şüpheli bulundu
}

public class ThreatResult
{
    public string FilePath { get; set; } = string.Empty;
    public string ThreatName { get; set; } = string.Empty;
    public ThreatType Type { get; set; }
    public string DetectionEngine { get; set; } = string.Empty; // Örn: "Signature", "Heuristic"
    public DateTime DetectedAt { get; set; } = DateTime.Now;
    
    // Uygulanan işlem (Örn: "Quarantined", "Cleaned", "Ignored", "Pending")
    public string ActionTaken { get; set; } = "Pending";
}
