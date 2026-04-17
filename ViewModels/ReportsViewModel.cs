using CommunityToolkit.Mvvm.ComponentModel;
using DefenderUI.Services;

namespace DefenderUI.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    private readonly MockDataService _mockDataService;

    public ReportsViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
    }
}