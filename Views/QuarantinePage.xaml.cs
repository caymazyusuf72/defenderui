using DefenderUI.Helpers;
using DefenderUI.Models;
using DefenderUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DefenderUI.Views;

public sealed partial class QuarantinePage : Page
{
    private bool _hasAnimated;

    public QuarantineViewModel ViewModel { get; }

    public QuarantinePage()
    {
        ViewModel = App.Current.Services.GetRequiredService<QuarantineViewModel>();
        InitializeComponent();
    }

    private T? F<T>(string name) where T : class => this.FindName(name) as T;

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (_hasAnimated)
        {
            return;
        }

        _hasAnimated = true;

        var header = F<FrameworkElement>("HeaderPanel");
        if (header is not null)
        {
            AnimationHelper.AnimateEntrance(header, delayMs: 0, durationMs: 450, offsetY: -16f);
        }

        double delay = 120;
        for (int i = 1; i <= 4; i++)
        {
            var kpi = F<UIElement>($"KpiCard{i}");
            if (kpi is not null)
            {
                AnimationHelper.AnimateScaleIn(kpi, delayMs: delay, durationMs: 450);
            }
            delay += 80;
        }

        var toolbar = F<FrameworkElement>("ToolbarCard");
        if (toolbar is not null)
        {
            AnimationHelper.AnimateEntrance(toolbar, delayMs: 500, durationMs: 500, offsetY: -12f);
        }

        var list = F<FrameworkElement>("ListContainer");
        if (list is not null)
        {
            AnimationHelper.AnimateEntrance(list, delayMs: 640, durationMs: 500);
        }
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