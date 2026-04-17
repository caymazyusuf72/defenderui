using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DefenderUI.Models;
using DefenderUI.ViewModels;

namespace DefenderUI.Views;

public sealed partial class QuarantinePage : Page
{
    public QuarantineViewModel ViewModel { get; }

    public QuarantinePage()
    {
        ViewModel = App.Current.Services.GetRequiredService<QuarantineViewModel>();
        InitializeComponent();
    }

    private void OnRestoreItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is ThreatInfo item)
        {
            ViewModel.RestoreItemCommand.Execute(item);
        }
    }

    private void OnDeleteItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is ThreatInfo item)
        {
            ViewModel.DeleteItemCommand.Execute(item);
        }
    }
}