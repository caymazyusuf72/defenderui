using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DefenderUI.Models;
using DefenderUI.ViewModels;

namespace DefenderUI.Views;

public sealed partial class ScanPage : Page
{
    public ScanViewModel ViewModel { get; }

    public ScanPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ScanViewModel>();
        InitializeComponent();
        UpdateScanTypeCardStyles();
    }

    private void OnScanTypeSelected(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string tag)
        {
            ViewModel.SelectedScanType = tag switch
            {
                "Quick" => ScanType.Quick,
                "Full" => ScanType.Full,
                "Custom" => ScanType.Custom,
                "USB" => ScanType.USB,
                _ => ScanType.Quick
            };
            UpdateScanTypeCardStyles();
        }
    }

    private void UpdateScanTypeCardStyles()
    {
        var accentBrush = (Microsoft.UI.Xaml.Media.SolidColorBrush)Application.Current.Resources["AccentBrush"];
        var borderBrush = (Microsoft.UI.Xaml.Media.SolidColorBrush)Application.Current.Resources["BorderBrush"];

        // Reset all cards
        QuickScanCard.BorderBrush = borderBrush;
        QuickScanCard.BorderThickness = new Thickness(1);
        FullScanCard.BorderBrush = borderBrush;
        FullScanCard.BorderThickness = new Thickness(1);
        CustomScanCard.BorderBrush = borderBrush;
        CustomScanCard.BorderThickness = new Thickness(1);
        UsbScanCard.BorderBrush = borderBrush;
        UsbScanCard.BorderThickness = new Thickness(1);

        // Highlight selected card
        var selectedCard = ViewModel.SelectedScanType switch
        {
            ScanType.Quick => QuickScanCard,
            ScanType.Full => FullScanCard,
            ScanType.Custom => CustomScanCard,
            ScanType.USB => UsbScanCard,
            _ => QuickScanCard
        };
        selectedCard.BorderBrush = accentBrush;
        selectedCard.BorderThickness = new Thickness(3);
    }
}