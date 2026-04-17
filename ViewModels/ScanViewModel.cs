using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DefenderUI.Models;
using DefenderUI.Services;
using Microsoft.UI.Dispatching;

namespace DefenderUI.ViewModels;

public partial class ScanViewModel : ObservableObject
{
    private readonly MockDataService _mockDataService;
    private DispatcherQueueTimer? _scanTimer;
    private DispatcherQueueTimer? _elapsedTimer;
    private DateTime _scanStartTime;
    private readonly Random _random = new();

    private static readonly string[] MockFilePaths =
    [
        @"C:\Windows\System32\drivers\etc\hosts",
        @"C:\Windows\System32\ntdll.dll",
        @"C:\Windows\System32\kernel32.dll",
        @"C:\Program Files\Common Files\System\msadc\msadce.dll",
        @"C:\Program Files\Windows Defender\MpClient.dll",
        @"C:\Users\Default\AppData\Local\Temp\setup.exe",
        @"C:\Users\Default\Downloads\document.pdf",
        @"C:\Program Files\Internet Explorer\iexplore.exe",
        @"C:\Windows\System32\config\SAM",
        @"C:\Windows\SysWOW64\msvcp140.dll",
        @"C:\Program Files (x86)\Microsoft\Edge\msedge.dll",
        @"C:\Windows\System32\svchost.exe",
        @"C:\Windows\System32\taskmgr.exe",
        @"C:\Program Files\dotnet\dotnet.exe",
        @"C:\Users\Default\Documents\report.xlsx",
        @"C:\Windows\System32\wbem\WmiPrvSE.exe",
        @"C:\Program Files\WindowsApps\Microsoft.WindowsStore\WinStore.App.exe",
        @"C:\Windows\System32\dxgi.dll",
        @"C:\Windows\Fonts\segoeui.ttf",
        @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp\helper.lnk"
    ];

    // Scan type selection
    [ObservableProperty]
    private ScanType _selectedScanType = ScanType.Quick;

    // Scan state
    [ObservableProperty]
    private ScanStatus _scanStatus = ScanStatus.NotStarted;

    [ObservableProperty]
    private double _scanProgress;

    [ObservableProperty]
    private string _currentFile = string.Empty;

    [ObservableProperty]
    private int _filesScanned;

    [ObservableProperty]
    private int _threatsFound;

    [ObservableProperty]
    private TimeSpan _elapsedTime;

    [ObservableProperty]
    private string _estimatedTimeRemaining = string.Empty;

    // Scan results
    [ObservableProperty]
    private ObservableCollection<ThreatInfo> _detectedThreats = [];

    // Scan history
    [ObservableProperty]
    private ObservableCollection<ScanResult> _scanHistory = [];

    // UI state
    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private bool _isScanComplete;

    [ObservableProperty]
    private string _scanStatusText = "Ready to scan";

    [ObservableProperty]
    private bool _isPaused;

    public bool HasThreats => ThreatsFound > 0;

    // Formatted properties for display
    public string ElapsedTimeFormatted => ElapsedTime.ToString(@"mm\:ss");
    public string FilesScannedFormatted => FilesScanned.ToString("N0");

    public ScanViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
        LoadData();
    }

    public void LoadData()
    {
        var history = _mockDataService.GetScanHistory();
        ScanHistory = new ObservableCollection<ScanResult>(history);
    }

    [RelayCommand]
    private void StartScan()
    {
        // Reset state
        ScanProgress = 0;
        FilesScanned = 0;
        ThreatsFound = 0;
        CurrentFile = string.Empty;
        ElapsedTime = TimeSpan.Zero;
        EstimatedTimeRemaining = "Calculating...";
        DetectedThreats = [];
        IsScanning = true;
        IsScanComplete = false;
        IsPaused = false;
        ScanStatus = ScanStatus.Running;
        ScanStatusText = $"Scanning... {SelectedScanType} Scan";
        _scanStartTime = DateTime.Now;

        OnPropertyChanged(nameof(ElapsedTimeFormatted));
        OnPropertyChanged(nameof(FilesScannedFormatted));

        // Create timers
        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        _scanTimer = dispatcherQueue.CreateTimer();
        _scanTimer.Interval = TimeSpan.FromMilliseconds(120);
        _scanTimer.Tick += OnScanTimerTick;
        _scanTimer.Start();

        _elapsedTimer = dispatcherQueue.CreateTimer();
        _elapsedTimer.Interval = TimeSpan.FromSeconds(1);
        _elapsedTimer.Tick += OnElapsedTimerTick;
        _elapsedTimer.Start();
    }

    [RelayCommand]
    private void PauseScan()
    {
        if (ScanStatus != ScanStatus.Running)
        {
            return;
        }

        IsPaused = true;
        ScanStatus = ScanStatus.Paused;
        ScanStatusText = "Scan paused";
        _scanTimer?.Stop();
        _elapsedTimer?.Stop();
    }

    [RelayCommand]
    private void ResumeScan()
    {
        if (ScanStatus != ScanStatus.Paused)
        {
            return;
        }

        IsPaused = false;
        ScanStatus = ScanStatus.Running;
        ScanStatusText = $"Scanning... {SelectedScanType} Scan";
        _scanTimer?.Start();
        _elapsedTimer?.Start();
    }

    [RelayCommand]
    private void StopScan()
    {
        StopTimers();
        IsScanning = false;
        IsPaused = false;
        ScanStatus = ScanStatus.Cancelled;
        ScanStatusText = "Scan cancelled";
    }

    [RelayCommand]
    private void ScanAgain()
    {
        IsScanComplete = false;
        ScanStatus = ScanStatus.NotStarted;
        ScanStatusText = "Ready to scan";
        ScanProgress = 0;
        FilesScanned = 0;
        ThreatsFound = 0;
        CurrentFile = string.Empty;
        ElapsedTime = TimeSpan.Zero;
        EstimatedTimeRemaining = string.Empty;
        DetectedThreats = [];

        OnPropertyChanged(nameof(ElapsedTimeFormatted));
        OnPropertyChanged(nameof(FilesScannedFormatted));
    }

    private void OnScanTimerTick(DispatcherQueueTimer sender, object args)
    {
        // Increment progress
        double increment = SelectedScanType switch
        {
            ScanType.Quick => _random.NextDouble() * 1.8 + 0.5,
            ScanType.Full => _random.NextDouble() * 0.4 + 0.1,
            ScanType.Custom => _random.NextDouble() * 0.8 + 0.3,
            ScanType.USB => _random.NextDouble() * 1.2 + 0.4,
            _ => 1.0
        };

        ScanProgress = Math.Min(100, ScanProgress + increment);

        // Update files scanned
        int fileIncrement = _random.Next(50, 200);
        FilesScanned += fileIncrement;
        OnPropertyChanged(nameof(FilesScannedFormatted));

        // Update current file
        CurrentFile = MockFilePaths[_random.Next(MockFilePaths.Length)];

        // Calculate estimated time remaining
        if (ScanProgress > 5)
        {
            double elapsed = ElapsedTime.TotalSeconds;
            double estimatedTotal = elapsed / (ScanProgress / 100.0);
            double remaining = estimatedTotal - elapsed;
            if (remaining > 0)
            {
                var remainingSpan = TimeSpan.FromSeconds(remaining);
                EstimatedTimeRemaining = $"~{remainingSpan:mm\\:ss}";
            }
        }

        // Add mock threats at specific progress points
        if (ScanProgress >= 30 && ScanProgress < 32 && ThreatsFound == 0)
        {
            ThreatsFound = 1;
            OnPropertyChanged(nameof(HasThreats));
            DetectedThreats.Add(new ThreatInfo
            {
                ThreatName = "Trojan.Gen.2",
                FilePath = @"C:\Users\Default\Downloads\setup.exe",
                DetectionDate = DateTime.Now,
                RiskLevel = RiskLevel.High,
                ActionTaken = "Detected",
                IsQuarantined = false
            });
        }

        if (ScanProgress >= 65 && ScanProgress < 67 && ThreatsFound == 1)
        {
            ThreatsFound = 2;
            OnPropertyChanged(nameof(HasThreats));
            DetectedThreats.Add(new ThreatInfo
            {
                ThreatName = "PUP.Optional.BrowserHelper",
                FilePath = @"C:\Program Files\FreeApp\helper.dll",
                DetectionDate = DateTime.Now,
                RiskLevel = RiskLevel.Medium,
                ActionTaken = "Detected",
                IsQuarantined = false
            });
        }

        // Complete scan
        if (ScanProgress >= 100)
        {
            CompleteScan();
        }
    }

    private void OnElapsedTimerTick(DispatcherQueueTimer sender, object args)
    {
        ElapsedTime = DateTime.Now - _scanStartTime;
        OnPropertyChanged(nameof(ElapsedTimeFormatted));
    }

    private void CompleteScan()
    {
        StopTimers();
        ScanProgress = 100;
        IsScanning = false;
        IsScanComplete = true;
        IsPaused = false;
        ScanStatus = ScanStatus.Completed;
        CurrentFile = string.Empty;
        EstimatedTimeRemaining = "00:00";

        ScanStatusText = ThreatsFound > 0
            ? $"{ThreatsFound} threats found and quarantined"
            : "No threats found";

        // Mark detected threats as quarantined
        foreach (var threat in DetectedThreats)
        {
            threat.ActionTaken = "Quarantined";
            threat.IsQuarantined = true;
        }

        // Add to scan history
        var result = new ScanResult
        {
            Type = SelectedScanType,
            Status = ScanStatus.Completed,
            StartTime = _scanStartTime,
            EndTime = DateTime.Now,
            Duration = ElapsedTime,
            FilesScanned = FilesScanned,
            ThreatsFound = ThreatsFound,
            Progress = 100
        };
        ScanHistory.Insert(0, result);

        OnPropertyChanged(nameof(FilesScannedFormatted));
        OnPropertyChanged(nameof(ElapsedTimeFormatted));
    }

    private void StopTimers()
    {
        if (_scanTimer is not null)
        {
            _scanTimer.Stop();
            _scanTimer.Tick -= OnScanTimerTick;
            _scanTimer = null;
        }

        if (_elapsedTimer is not null)
        {
            _elapsedTimer.Stop();
            _elapsedTimer.Tick -= OnElapsedTimerTick;
            _elapsedTimer = null;
        }
    }
}