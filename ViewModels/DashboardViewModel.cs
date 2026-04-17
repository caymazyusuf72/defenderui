using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DefenderUI.Models;
using DefenderUI.Services;

namespace DefenderUI.ViewModels;

#pragma warning disable MVVMTK0045 // Using [ObservableProperty] with fields for WinRT compatibility

public partial class DashboardViewModel : ObservableObject
{
    private readonly MockDataService _mockDataService;

    [ObservableProperty]
    private ProtectionState _protectionState;

    [ObservableProperty]
    private int _securityScore;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _statusDescription = string.Empty;

    // KPI values
    [ObservableProperty]
    private int _threatsDetected;

    [ObservableProperty]
    private int _quarantinedItems;

    [ObservableProperty]
    private int _blockedAttacks;

    [ObservableProperty]
    private int _protectedFiles;

    [ObservableProperty]
    private int _suspiciousActivity;

    // Last scan info
    [ObservableProperty]
    private string _lastScanDate = string.Empty;

    [ObservableProperty]
    private string _lastScanDuration = string.Empty;

    [ObservableProperty]
    private int _lastScanFilesChecked;

    [ObservableProperty]
    private int _lastScanThreatsFound;

    // Protection Modules
    [ObservableProperty]
    private ObservableCollection<ProtectionModule> _protectionModules = [];

    // Activity Log
    [ObservableProperty]
    private ObservableCollection<ActivityLogItem> _recentActivities = [];

    // Update Info
    [ObservableProperty]
    private string _virusDefinitionVersion = string.Empty;

    [ObservableProperty]
    private string _appVersion = string.Empty;

    [ObservableProperty]
    private string _lastUpdateDate = string.Empty;

    [ObservableProperty]
    private bool _isUpdateAvailable;

    // System Health
    [ObservableProperty]
    private int _healthScore;

    [ObservableProperty]
    private double _cpuImpact;

    [ObservableProperty]
    private double _memoryUsage;

    [ObservableProperty]
    private bool _autoUpdatesEnabled;

    [ObservableProperty]
    private bool _backgroundProtection;

    // Alerts
    [ObservableProperty]
    private ObservableCollection<string> _alerts = [];

    public DashboardViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
        LoadData();
    }

    private void LoadData()
    {
        var status = _mockDataService.GetProtectionStatus();
        ProtectionState = status.State;
        SecurityScore = status.SecurityScore;
        StatusMessage = status.StatusMessage;
        StatusDescription = status.Description;

        var threats = _mockDataService.GetRecentThreats();
        ThreatsDetected = threats.Count;
        QuarantinedItems = threats.Count(t => t.IsQuarantined);
        BlockedAttacks = threats.Count(t => t.ActionTaken == "Blocked");
        SuspiciousActivity = threats.Count(t => t.RiskLevel == RiskLevel.Medium);

        ProtectedFiles = 128_457;

        // Protection Modules
        ProtectionModules = new ObservableCollection<ProtectionModule>(_mockDataService.GetProtectionModules());

        // Recent Activities
        RecentActivities = new ObservableCollection<ActivityLogItem>(_mockDataService.GetRecentActivity());

        // Update Info
        var updateInfo = _mockDataService.GetUpdateInfo();
        VirusDefinitionVersion = updateInfo.VirusDefinitionVersion;
        AppVersion = updateInfo.AppVersion;
        LastUpdateDate = updateInfo.LastUpdateDate.ToString("MMM dd, yyyy HH:mm");
        IsUpdateAvailable = updateInfo.IsUpdateAvailable;

        // System Health
        var healthInfo = _mockDataService.GetSystemHealthInfo();
        HealthScore = healthInfo.HealthScore;
        CpuImpact = healthInfo.CpuImpact;
        MemoryUsage = healthInfo.MemoryUsage;
        AutoUpdatesEnabled = healthInfo.AutoUpdates;
        BackgroundProtection = healthInfo.BackgroundProtection;

        // Alerts
        Alerts =
        [
            "Scheduled scan overdue - last scan was 3 days ago",
            "2 items in quarantine require attention"
        ];

        var lastScan = _mockDataService.GetLastScanResult();
        LastScanDate = lastScan.StartTime.ToString("MMM dd, yyyy HH:mm");
        LastScanDuration = $"{lastScan.Duration.Minutes}m {lastScan.Duration.Seconds}s";
        LastScanFilesChecked = lastScan.FilesScanned;
        LastScanThreatsFound = lastScan.ThreatsFound;
    }

    [RelayCommand]
    private void QuickScan()
    {
        // UI placeholder
    }

    [RelayCommand]
    private void FullScan()
    {
        // UI placeholder
    }

    [RelayCommand]
    private void CustomScan()
    {
        // UI placeholder
    }

    [RelayCommand]
    private void UpdateNow()
    {
        // UI placeholder
    }

    [RelayCommand]
    private void FixIssues()
    {
        // UI placeholder
    }

    [RelayCommand]
    private void CheckForUpdates()
    {
        // UI placeholder
    }

    [RelayCommand]
    private void ViewAllActivity()
    {
        // UI placeholder
    }

    [RelayCommand]
    private void RunScanNow()
    {
        // UI placeholder
    }

    [RelayCommand]
    private void ViewQuarantine()
    {
        // UI placeholder
    }
}

#pragma warning restore MVVMTK0045