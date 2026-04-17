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
}

#pragma warning restore MVVMTK0045