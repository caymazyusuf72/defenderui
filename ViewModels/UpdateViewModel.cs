using CommunityToolkit.Mvvm.ComponentModel;
using DefenderUI.Services;

namespace DefenderUI.ViewModels;

public partial class UpdateViewModel : ObservableObject
{
    private readonly MockDataService _mockDataService;

    public UpdateViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
    }
}