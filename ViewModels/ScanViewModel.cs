using CommunityToolkit.Mvvm.ComponentModel;
using DefenderUI.Services;

namespace DefenderUI.ViewModels;

public partial class ScanViewModel : ObservableObject
{
    private readonly MockDataService _mockDataService;

    public ScanViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
    }
}