using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using DefenderUI.ViewModels;

namespace DefenderUI.Views;

public sealed partial class ReportsPage : Page
{
    public ReportsViewModel ViewModel { get; }

    public ReportsPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ReportsViewModel>();
        InitializeComponent();
    }
}