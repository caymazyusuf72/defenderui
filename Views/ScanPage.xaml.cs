using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DefenderUI.Controls;
using DefenderUI.Services;
using DefenderUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DefenderUI.Views;

/// <summary>
/// Scan sayfası (Faz 4). IScanService + ScanViewModel ile çalışır;
/// ItemsRepeater içindeki ScanModeCard'ların IsSelected durumunu
/// ViewModel.SelectedMode değişikliklerine göre code-behind'da yönetir.
/// </summary>
public sealed partial class ScanPage : Page
{
    public ScanViewModel ViewModel { get; }

    public ScanPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ScanViewModel>();
        InitializeComponent();
        DataContext = ViewModel;

        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.ApplyNavigationParameter(e.Parameter);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectedMode))
        {
            UpdateModeCardSelection();
        }
    }

    private void UpdateModeCardSelection()
    {
        var root = IdleStatePanel;
        if (root is null) return;

        foreach (var card in EnumerateScanModeCards(root))
        {
            card.IsSelected = card.Mode == ViewModel.SelectedMode;
        }
    }

    private static System.Collections.Generic.IEnumerable<ScanModeCard> EnumerateScanModeCards(DependencyObject parent)
    {
        var count = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is ScanModeCard card)
            {
                yield return card;
            }
            foreach (var sub in EnumerateScanModeCards(child))
            {
                yield return sub;
            }
        }
    }

    private async void AddPathButton_Click(object sender, RoutedEventArgs e)
    {
        // Not: FolderPicker host Window gerektirir; basitleştirmek adına
        // MVP'de örnek bir path ekleyelim. İleri fazda FolderPicker entegre edilecek.
        var samples = new[]
        {
            @"C:\Users",
            @"C:\Program Files",
            @"C:\Windows\System32",
            @"D:\Documents",
            @"D:\Downloads",
            @"E:\"
        };

        var existing = ViewModel.CustomPaths.ToHashSet();
        var candidate = samples.FirstOrDefault(s => !existing.Contains(s));
        if (candidate is not null)
        {
            ViewModel.AddCustomPathCommand.Execute(candidate);
        }
        await System.Threading.Tasks.Task.CompletedTask;
    }

    /// <summary>
    /// x:Bind fonksiyon çağrısı — CustomPaths boşsa Visible, değilse Collapsed.
    /// </summary>
    public static Visibility IsCollectionEmpty(int count)
        => count == 0 ? Visibility.Visible : Visibility.Collapsed;
}