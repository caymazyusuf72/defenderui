using System;
using DefenderUI.Helpers;
using DefenderUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DefenderUI.Views;

public sealed partial class DashboardPage : Page
{
    private bool _hasAnimated;

    public DashboardViewModel ViewModel { get; }

    public DashboardPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<DashboardViewModel>();
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (_hasAnimated)
        {
            return;
        }

        _hasAnimated = true;

        // Page header slides down + fades in first.
        AnimationHelper.AnimateEntrance(HeaderPanel, delayMs: 0, durationMs: 450, offsetY: -16f);

        // Hero card scales in with a subtle bounce.
        AnimationHelper.AnimateScaleIn(HeroCard, delayMs: 100, durationMs: 550);

        // KPI cards staggered from the right.
        AnimationHelper.AnimateStaggered(
            new UIElement[] { Kpi1, Kpi2, Kpi3, Kpi4 },
            staggerMs: 80,
            initialDelayMs: 250,
            durationMs: 450);

        // Quick actions header + buttons.
        AnimationHelper.AnimateEntrance(QuickActionsHeader, delayMs: 500, durationMs: 400);
        AnimationHelper.AnimateStaggered(
            new UIElement[] { QuickAction1, QuickAction2, QuickAction3, QuickAction4 },
            staggerMs: 70,
            initialDelayMs: 600,
            durationMs: 450);

        // Protection section.
        AnimationHelper.AnimateEntrance(ProtectionSectionHeader, delayMs: 900, durationMs: 400);
        AnimationHelper.AnimateEntrance(ProtectionCard, delayMs: 980, durationMs: 500);
        AnimationHelper.AnimateEntrance(UpdateStatusCard, delayMs: 1060, durationMs: 500);
        AnimationHelper.AnimateEntrance(HealthCard, delayMs: 1140, durationMs: 500);

        // Activity section.
        AnimationHelper.AnimateEntrance(ActivitySectionHeader, delayMs: 1300, durationMs: 400);
        AnimationHelper.AnimateEntrance(ActivityCard, delayMs: 1380, durationMs: 500);
        AnimationHelper.AnimateEntrance(AlertsCard, delayMs: 1460, durationMs: 500);

        // Hero emphasis effects - start after entrance completes.
        _ = StartHeroEmphasisAsync();
    }

    private async System.Threading.Tasks.Task StartHeroEmphasisAsync()
    {
        // Wait for entrance animations to finish before starting continuous loops.
        await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(900));

        // Pulsing outer ring around the security score.
        AnimationHelper.StartPulse(ScorePulseRing, minScale: 1.0f, maxScale: 1.12f, durationMs: 1800);
        AnimationHelper.StartOpacityPulse(ScorePulseRing, minOpacity: 0.15f, maxOpacity: 0.55f, durationMs: 1800);

        // Subtle pulse on the shield icon.
        AnimationHelper.StartPulse(ShieldIcon, minScale: 1.0f, maxScale: 1.08f, durationMs: 2200);
    }
}