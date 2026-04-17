using CommunityToolkit.Mvvm.ComponentModel;
using DefenderUI.Services;

namespace DefenderUI.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly MockDataService _mockDataService;

    public DashboardViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
    }
}