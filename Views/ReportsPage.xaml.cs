using DefenderUI.Helpers;
using DefenderUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DefenderUI.Views;

public sealed partial class ReportsPage : Page
{
    private bool _hasAnimated;
    private int _weeklyIndex;

    public ReportsViewModel ViewModel { get; }

    public ReportsPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ReportsViewModel>();
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

        var header = F<FrameworkElement>("HeaderGrid");
        if (header is not null)
        {
            AnimationHelper.AnimateEntrance(header, delayMs: 0, durationMs: 450, offsetY: -16f);
        }

        // 4 KPI cards
        string[] kpiNames = { "KpiScans", "KpiThreats", "KpiBlocked", "KpiFiles" };
        double kpiDelay = 200;
        foreach (var name in kpiNames)
        {
            var kpi = F<UIElement>(name);
            if (kpi is not null)
            {
                AnimationHelper.AnimateScaleIn(kpi, delayMs: kpiDelay, durationMs: 450);
            }
            kpiDelay += 80;
        }

        var trends = F<FrameworkElement>("TrendsCard");
        if (trends is not null)
        {
            AnimationHelper.AnimateEntrance(trends, delayMs: 620, durationMs: 500);
        }

        var dist = F<FrameworkElement>("DistributionCard");
        if (dist is not null)
        {
            AnimationHelper.AnimateEntrance(dist, delayMs: 760, durationMs: 500);
        }

        // Threat distribution bars: animate from 0 to their bound values.
        AnimateBar("TrojanBar", ViewModel.TrojanCount, 900);
        AnimateBar("AdwareBar", ViewModel.AdwareCount, 1000);
        AnimateBar("SpywareBar", ViewModel.SpywareCount, 1100);
        AnimateBar("PupBar", ViewModel.PupCount, 1200);
        AnimateBar("RansomwareBar", ViewModel.RansomwareCount, 1300);

        var summary = F<FrameworkElement>("SummaryCard");
        if (summary is not null)
        {
            AnimationHelper.AnimateEntrance(summary, delayMs: 1200, durationMs: 500);
        }
        var recent = F<FrameworkElement>("RecentScansCard");
        if (recent is not null)
        {
            AnimationHelper.AnimateEntrance(recent, delayMs: 1340, durationMs: 500);
        }
    }

    private void AnimateBar(string name, double target, double delayMs)
    {
        var bar = F<ProgressBar>(name);
        if (bar is not null)
        {
            AnimationHelper.AnimateProgressBar(bar, target, delayMs: delayMs, durationMs: 800);
        }
    }

    private void WeeklyRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        if (args.Element is not FrameworkElement fe)
        {
            return;
        }

        var delay = 800 + (_weeklyIndex * 100);
        AnimationHelper.AnimateEntrance(fe, delayMs: delay, durationMs: 500, offsetY: 12f);

        fe.Loaded += (_, _) =>
        {
            if (FindDescendantProgressBar(fe) is ProgressBar bar)
            {
                double target = bar.Value;
                AnimationHelper.AnimateProgressBar(bar, target, delayMs: delay + 120, durationMs: 700);
            }
        };

        _weeklyIndex++;
    }

    private static ProgressBar? FindDescendantProgressBar(DependencyObject root)
    {
        int count = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(root);
        for (int i = 0; i < count; i++)
        {
            var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(root, i);
            if (child is ProgressBar bar)
            {
                return bar;
            }

            var nested = FindDescendantProgressBar(child);
            if (nested is not null)
            {
                return nested;
            }
        }
        return null;
    }
}