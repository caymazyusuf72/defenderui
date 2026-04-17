using CommunityToolkit.Mvvm.ComponentModel;
using DefenderUI.Services;

namespace DefenderUI.ViewModels;

public partial class ProtectionViewModel : ObservableObject
{
    private readonly MockDataService _mockDataService;

    public ProtectionViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
    }
}