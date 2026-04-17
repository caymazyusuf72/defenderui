using CommunityToolkit.Mvvm.ComponentModel;
using DefenderUI.Services;

namespace DefenderUI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly MockDataService _mockDataService;

    public SettingsViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
    }
}