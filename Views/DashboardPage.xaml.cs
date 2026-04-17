using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using DefenderUI.ViewModels;

namespace DefenderUI.Views;

public sealed partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel { get; }

    public DashboardPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<DashboardViewModel>();
        InitializeComponent();
    }
}