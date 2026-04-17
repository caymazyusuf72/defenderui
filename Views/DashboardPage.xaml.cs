using System;
using DefenderUI.Helpers;
using DefenderUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI;

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

        // Number count-up on KPI cards (odometer effect).
        AnimationHelper.AnimateNumberCount(ThreatsDetectedText, 0, ViewModel.ThreatsDetected, 800);
        AnimationHelper.AnimateNumberCount(QuarantinedText, 0, ViewModel.QuarantinedItems, 900);
        AnimationHelper.AnimateNumberCount(BlockedAttacksText, 0, ViewModel.BlockedAttacks, 1200);
        AnimationHelper.AnimateNumberCount(ProtectedFilesText, 0, ViewModel.ProtectedFiles, 1500);

        // Security score counts up from 0 as well.
        AnimationHelper.AnimateNumberCount(SecurityScoreText, 0, ViewModel.SecurityScore, 1200);

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

        // Decorative glow ring slowly rotates around the security score.
        AnimationHelper.StartRotation(GlowRing, durationSec: 20.0);

        // Color-aware glow pulse on the hero accent stripe based on protection state.
        var glowColor = ViewModel.ProtectionState switch
        {
            Models.ProtectionState.Protected => Color.FromArgb(255, 0x3F, 0xB9, 0x50),
            Models.ProtectionState.AtRisk => Color.FromArgb(255, 0xF8, 0x51, 0x49),
            Models.ProtectionState.AttentionNeeded => Color.FromArgb(255, 0xD2, 0x99, 0x22),
            Models.ProtectionState.Scanning => Color.FromArgb(255, 0x58, 0xA6, 0xFF),
            _ => Color.FromArgb(255, 0x3F, 0xB9, 0x50),
        };
        AnimationHelper.StartGlowPulse(HeroAccentStripe, glowColor, durationMs: 2400,
            minBlur: 10f, maxBlur: 24f, minOpacity: 0.3f, maxOpacity: 0.75f);

        if (ViewModel.ProtectionState == Models.ProtectionState.AtRisk)
        {
            AnimationHelper.Shake(HeroAccentStripe, intensity: 4f, durationMs: 450);
        }
    }
}