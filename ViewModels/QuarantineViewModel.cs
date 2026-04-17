using CommunityToolkit.Mvvm.ComponentModel;
using DefenderUI.Services;

namespace DefenderUI.ViewModels;

public partial class QuarantineViewModel : ObservableObject
{
    private readonly MockDataService _mockDataService;

    public QuarantineViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
    }
}