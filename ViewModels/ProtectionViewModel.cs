using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DefenderUI.Models;
using DefenderUI.Services;

namespace DefenderUI.ViewModels;

#pragma warning disable MVVMTK0045 // Using [ObservableProperty] with fields for WinRT compatibility

public partial class ProtectionViewModel : ObservableObject
{
    private readonly MockDataService _mockDataService;

    // Protection modules
    [ObservableProperty]
    private ObservableCollection<ProtectionModule> _protectionModules = [];

    // Overall protection status
    [ObservableProperty]
    private bool _isAllProtectionEnabled = true;

    [ObservableProperty]
    private int _activeModulesCount;

    [ObservableProperty]
    private int _totalModulesCount;

    [ObservableProperty]
    private string _overallStatusText = "All protection features are active";

    // Advanced settings - Cloud & Analysis
    [ObservableProperty]
    private bool _cloudProtection = true;

    [ObservableProperty]
    private bool _automaticSampleSubmission = true;

    [ObservableProperty]
    private bool _tamperProtection = true;

    // Advanced settings - Advanced Protection
    [ObservableProperty]
    private bool _puaProtection = true;

    [ObservableProperty]
    private bool _exploitProtection = true;

    [ObservableProperty]
    private bool _networkInspection = true;

    // Firewall status
    [ObservableProperty]
    private bool _firewallEnabled = true;

    [ObservableProperty]
    private string _firewallProfile = "Private Network";

    [ObservableProperty]
    private int _blockedConnectionsToday = 23;

    [ObservableProperty]
    private int _inboundRules = 145;

    [ObservableProperty]
    private int _outboundRules = 89;

    public ProtectionViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
        LoadData();
    }

    private void LoadData()
    {
        var modules = _mockDataService.GetProtectionModules();
        ProtectionModules = new ObservableCollection<ProtectionModule>(modules);
        TotalModulesCount = modules.Count;
        ActiveModulesCount = modules.Count(m => m.IsEnabled);
        UpdateOverallStatus();
    }

    private void UpdateOverallStatus()
    {
        IsAllProtectionEnabled = ActiveModulesCount == TotalModulesCount;

        if (ActiveModulesCount == TotalModulesCount)
        {
            OverallStatusText = "All protection features are active";
        }
        else if (ActiveModulesCount == 0)
        {
            OverallStatusText = "All protection features are disabled";
        }
        else
        {
            OverallStatusText = $"{TotalModulesCount - ActiveModulesCount} protection feature(s) need attention";
        }
    }

    [RelayCommand]
    private void EnableAllProtection()
    {
        foreach (var module in ProtectionModules)
        {
            module.IsEnabled = true;
            module.HasIssue = false;
            module.IssueDescription = string.Empty;
        }

        ActiveModulesCount = TotalModulesCount;
        UpdateOverallStatus();
        OnPropertyChanged(nameof(ProtectionModules));
    }

    [RelayCommand]
    private void DisableAllProtection()
    {
        foreach (var module in ProtectionModules)
        {
            module.IsEnabled = false;
        }

        ActiveModulesCount = 0;
        UpdateOverallStatus();
        OnPropertyChanged(nameof(ProtectionModules));
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        CloudProtection = true;
        AutomaticSampleSubmission = true;
        TamperProtection = true;
        PuaProtection = true;
        ExploitProtection = true;
        NetworkInspection = true;
        FirewallEnabled = true;
        LoadData();
    }

    [RelayCommand]
    private void ToggleModule(ProtectionModule? module)
    {
        if (module is null)
        {
            return;
        }

        module.IsEnabled = !module.IsEnabled;

        if (module.IsEnabled)
        {
            module.HasIssue = false;
            module.IssueDescription = string.Empty;
        }

        ActiveModulesCount = ProtectionModules.Count(m => m.IsEnabled);
        UpdateOverallStatus();
        OnPropertyChanged(nameof(ProtectionModules));
    }

    [RelayCommand]
    private void ViewFirewallRules()
    {
        // UI placeholder
    }

    [RelayCommand]
    private void ResetFirewall()
    {
        // UI placeholder
    }
}

#pragma warning restore MVVMTK0045