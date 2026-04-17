using DefenderUI.Models;

namespace DefenderUI.Services;

public class MockDataService
{
    public ProtectionStatus GetProtectionStatus()
    {
        return new ProtectionStatus
        {
            State = ProtectionState.Protected,
            SecurityScore = 85,
            StatusMessage = "Your device is protected",
            Description = "All security features are active and up to date."
        };
    }

    public List<ThreatInfo> GetRecentThreats()
    {
        return
        [
            new ThreatInfo
            {
                ThreatName = "Trojan:Win32/Emotet.G",
                FilePath = @"C:\Users\Default\Downloads\invoice_2024.exe",
                DetectionDate = DateTime.Now.AddHours(-2),
                RiskLevel = RiskLevel.Critical,
                ActionTaken = "Quarantined",
                IsQuarantined = true
            },
            new ThreatInfo
            {
                ThreatName = "PUA:Win32/Presenoker",
                FilePath = @"C:\Program Files\FreeApp\toolbar.dll",
                DetectionDate = DateTime.Now.AddDays(-1),
                RiskLevel = RiskLevel.Medium,
                ActionTaken = "Blocked",
                IsQuarantined = false
            },
            new ThreatInfo
            {
                ThreatName = "Adware:Win32/BrowserModifier",
                FilePath = @"C:\Users\Default\AppData\Local\Temp\setup.exe",
                DetectionDate = DateTime.Now.AddDays(-3),
                RiskLevel = RiskLevel.Low,
                ActionTaken = "Removed",
                IsQuarantined = false
            },
            new ThreatInfo
            {
                ThreatName = "Ransom:Win32/LockBit.A",
                FilePath = @"C:\Users\Default\Documents\encrypted_file.locked",
                DetectionDate = DateTime.Now.AddDays(-5),
                RiskLevel = RiskLevel.Critical,
                ActionTaken = "Quarantined",
                IsQuarantined = true
            },
            new ThreatInfo
            {
                ThreatName = "Trojan:Win32/AgentTesla.SM",
                FilePath = @"C:\Users\Default\Downloads\report.docx.exe",
                DetectionDate = DateTime.Now.AddDays(-7),
                RiskLevel = RiskLevel.High,
                ActionTaken = "Removed",
                IsQuarantined = false
            }
        ];
    }

    public ScanResult GetLastScanResult()
    {
        return new ScanResult
        {
            Type = ScanType.Quick,
            Status = ScanStatus.Completed,
            StartTime = DateTime.Now.AddHours(-4),
            EndTime = DateTime.Now.AddHours(-4).AddMinutes(12),
            Duration = TimeSpan.FromMinutes(12),
            FilesScanned = 45892,
            ThreatsFound = 2,
            Progress = 100,
            CurrentFile = string.Empty
        };
    }

    public List<ProtectionModule> GetProtectionModules()
    {
        return
        [
            new ProtectionModule
            {
                Name = "Real-time Protection",
                Description = "Monitors your system for threats in real-time",
                Icon = "\uE8BE",
                IsEnabled = true,
                HasIssue = false,
                IssueDescription = string.Empty
            },
            new ProtectionModule
            {
                Name = "Web Protection",
                Description = "Blocks malicious websites and downloads",
                Icon = "\uE774",
                IsEnabled = true,
                HasIssue = false,
                IssueDescription = string.Empty
            },
            new ProtectionModule
            {
                Name = "File Protection",
                Description = "Scans files when accessed or modified",
                Icon = "\uE8B7",
                IsEnabled = true,
                HasIssue = false,
                IssueDescription = string.Empty
            },
            new ProtectionModule
            {
                Name = "Ransomware Protection",
                Description = "Protects your files from encryption attacks",
                Icon = "\uE72E",
                IsEnabled = true,
                HasIssue = false,
                IssueDescription = string.Empty
            },
            new ProtectionModule
            {
                Name = "Email Protection",
                Description = "Scans email attachments for threats",
                Icon = "\uE715",
                IsEnabled = false,
                HasIssue = true,
                IssueDescription = "Email protection is currently disabled"
            },
            new ProtectionModule
            {
                Name = "Network Protection",
                Description = "Monitors network traffic for suspicious activity",
                Icon = "\uE968",
                IsEnabled = true,
                HasIssue = false,
                IssueDescription = string.Empty
            }
        ];
    }

    public List<ActivityLogItem> GetRecentActivity()
    {
        return
        [
            new ActivityLogItem
            {
                Type = ActivityType.ThreatBlocked,
                Title = "Threat blocked",
                Description = "Trojan:Win32/Emotet.G was quarantined",
                Timestamp = DateTime.Now.AddHours(-2)
            },
            new ActivityLogItem
            {
                Type = ActivityType.ScanCompleted,
                Title = "Quick scan completed",
                Description = "45,892 files scanned, 2 threats found",
                Timestamp = DateTime.Now.AddHours(-4)
            },
            new ActivityLogItem
            {
                Type = ActivityType.DatabaseUpdated,
                Title = "Virus definitions updated",
                Description = "Updated to version 1.405.651.0",
                Timestamp = DateTime.Now.AddHours(-6)
            },
            new ActivityLogItem
            {
                Type = ActivityType.FileQuarantined,
                Title = "File quarantined",
                Description = "PUA:Win32/Presenoker moved to quarantine",
                Timestamp = DateTime.Now.AddDays(-1)
            },
            new ActivityLogItem
            {
                Type = ActivityType.ProtectionEnabled,
                Title = "Protection enabled",
                Description = "Real-time protection was turned on",
                Timestamp = DateTime.Now.AddDays(-1).AddHours(-3)
            },
            new ActivityLogItem
            {
                Type = ActivityType.Warning,
                Title = "Email protection disabled",
                Description = "Email protection module is not active",
                Timestamp = DateTime.Now.AddDays(-2)
            },
            new ActivityLogItem
            {
                Type = ActivityType.ScanCompleted,
                Title = "Full scan completed",
                Description = "1,245,678 files scanned, 0 threats found",
                Timestamp = DateTime.Now.AddDays(-3)
            },
            new ActivityLogItem
            {
                Type = ActivityType.ThreatBlocked,
                Title = "Ransomware blocked",
                Description = "Ransom:Win32/LockBit.A was blocked and quarantined",
                Timestamp = DateTime.Now.AddDays(-5)
            },
            new ActivityLogItem
            {
                Type = ActivityType.DatabaseUpdated,
                Title = "Virus definitions updated",
                Description = "Updated to version 1.405.620.0",
                Timestamp = DateTime.Now.AddDays(-6)
            },
            new ActivityLogItem
            {
                Type = ActivityType.ProtectionDisabled,
                Title = "Web protection temporarily disabled",
                Description = "User disabled web protection for 1 hour",
                Timestamp = DateTime.Now.AddDays(-7)
            }
        ];
    }

    public UpdateInfo GetUpdateInfo()
    {
        return new UpdateInfo
        {
            VirusDefinitionVersion = "1.405.651.0",
            AppVersion = "1.0.0",
            LastUpdateDate = DateTime.Now.AddHours(-6),
            IsUpdateAvailable = false,
            UpdateProgress = 0,
            UpdateSize = "45.2 MB"
        };
    }

    public SystemHealthInfo GetSystemHealthInfo()
    {
        return new SystemHealthInfo
        {
            HealthScore = 85,
            CpuImpact = 2.3,
            MemoryUsage = 128.5,
            BackgroundProtection = true,
            AutoUpdates = true,
            SecureBrowser = true,
            SafeNetwork = false
        };
    }
}