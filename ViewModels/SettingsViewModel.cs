using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DefenderUI.Services;

namespace DefenderUI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly MockDataService _mockDataService;

    // General
    [ObservableProperty] private bool _startWithWindows = true;
    [ObservableProperty] private bool _minimizeToTray = true;
    [ObservableProperty] private bool _showNotifications = true;
    [ObservableProperty] private string _selectedLanguage = "English";

    // Protection
    [ObservableProperty] private bool _realTimeProtection = true;
    [ObservableProperty] private bool _cloudProtection = true;
    [ObservableProperty] private bool _automaticSampleSubmission;
    [ObservableProperty] private string _scanSensitivity = "Balanced";
    [ObservableProperty] private bool _scanArchives = true;
    [ObservableProperty] private bool _scanRemovableDrives = true;
    [ObservableProperty] private bool _scanNetworkDrives;

    // Notifications
    [ObservableProperty] private bool _threatNotifications = true;
    [ObservableProperty] private bool _scanCompleteNotifications = true;
    [ObservableProperty] private bool _updateNotifications = true;
    [ObservableProperty] private bool _weeklyReportNotifications;
    [ObservableProperty] private bool _soundAlerts;

    // Scheduled Scans
    [ObservableProperty] private bool _scheduledScanEnabled = true;
    [ObservableProperty] private string _scanFrequency = "Weekly";
    [ObservableProperty] private string _scanDay = "Monday";
    [ObservableProperty] private string _scanTime = "02:00 AM";
    [ObservableProperty] private string _scheduledScanType = "Quick Scan";

    // Exclusions
    [ObservableProperty] private ObservableCollection<string> _excludedFiles = [];
    [ObservableProperty] private ObservableCollection<string> _excludedFolders = [];

    // Appearance
    [ObservableProperty] private string _selectedTheme = "Dark";
    [ObservableProperty] private string _accentColor = "Blue";

    // Privacy
    [ObservableProperty] private bool _sendUsageData;
    [ObservableProperty] private bool _sendCrashReports = true;
    [ObservableProperty] private bool _participateInBeta;

    // About
    [ObservableProperty] private string _appVersionInfo = "DefenderUI v1.2.0";
    [ObservableProperty] private string _buildNumber = "Build 2026.04.17.001";
    [ObservableProperty] private string _licenseType = "Premium License";
    [ObservableProperty] private string _licenseExpiry = "Dec 31, 2027";

    // ComboBox item sources
    public List<string> Languages { get; } = ["English", "Türkçe", "Deutsch", "Français", "Español", "日本語"];
    public List<string> ScanSensitivities { get; } = ["Low", "Balanced", "High", "Maximum"];
    public List<string> ScanFrequencies { get; } = ["Daily", "Weekly", "Monthly"];
    public List<string> ScanDays { get; } = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    public List<string> ScanTimes { get; } = ["12:00 AM", "01:00 AM", "02:00 AM", "03:00 AM", "04:00 AM", "05:00 AM", "06:00 AM", "07:00 AM", "08:00 AM", "09:00 AM", "10:00 AM", "11:00 AM", "12:00 PM", "01:00 PM", "02:00 PM", "03:00 PM", "04:00 PM", "05:00 PM", "06:00 PM", "07:00 PM", "08:00 PM", "09:00 PM", "10:00 PM", "11:00 PM"];
    public List<string> ScanTypes { get; } = ["Quick Scan", "Full Scan"];
    public List<string> Themes { get; } = ["Dark", "Light", "System"];
    public List<string> AccentColors { get; } = ["Blue", "Teal", "Green", "Purple"];

    public SettingsViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
        LoadData();
    }

    private void LoadData()
    {
        ExcludedFiles =
        [
            @"C:\Program Files\MyApp\app.exe",
            @"C:\Users\John\Documents\whitelist.dll",
        ];

        ExcludedFolders =
        [
            @"C:\Development\Projects",
            @"C:\Games\Steam",
        ];
    }

    [RelayCommand]
    private void AddExcludedFile()
    {
        ExcludedFiles.Add(@"C:\NewPath\newfile.exe");
    }

    [RelayCommand]
    private void AddExcludedFolder()
    {
        ExcludedFolders.Add(@"C:\NewPath\NewFolder");
    }

    [RelayCommand]
    private void RemoveExcludedFile(string path)
    {
        ExcludedFiles.Remove(path);
    }

    [RelayCommand]
    private void RemoveExcludedFolder(string path)
    {
        ExcludedFolders.Remove(path);
    }

    [RelayCommand]
    private void ResetAllSettings()
    {
        StartWithWindows = true;
        MinimizeToTray = true;
        ShowNotifications = true;
        SelectedLanguage = "English";
        RealTimeProtection = true;
        CloudProtection = true;
        AutomaticSampleSubmission = false;
        ScanSensitivity = "Balanced";
        ScanArchives = true;
        ScanRemovableDrives = true;
        ScanNetworkDrives = false;
        ThreatNotifications = true;
        ScanCompleteNotifications = true;
        UpdateNotifications = true;
        WeeklyReportNotifications = false;
        SoundAlerts = false;
        ScheduledScanEnabled = true;
        ScanFrequency = "Weekly";
        ScanDay = "Monday";
        ScanTime = "02:00 AM";
        ScheduledScanType = "Quick Scan";
        SelectedTheme = "Dark";
        AccentColor = "Blue";
        SendUsageData = false;
        SendCrashReports = true;
        ParticipateInBeta = false;
    }

    [RelayCommand]
    private void ExportSettings()
    {
        // Mock: In real app, would export to JSON file
    }

    [RelayCommand]
    private void ImportSettings()
    {
        // Mock: In real app, would import from JSON file
    }

    [RelayCommand]
    private void CheckForUpdates()
    {
        // Mock: In real app, would check for updates
    }
}