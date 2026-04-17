using System;
using System.ComponentModel;
using DefenderUI.Helpers;
using DefenderUI.Models;
using DefenderUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI;

namespace DefenderUI.Views;

public sealed partial class ScanPage : Page
{
    private bool _hasAnimated;
    private int _lastThreatsFound;
    private bool _wasScanning;

    public ScanViewModel ViewModel { get; }

    public ScanPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ScanViewModel>();
        InitializeComponent();
        UpdateScanTypeCardStyles();

        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
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

        var scanTypeHeader = F<FrameworkElement>("ScanTypeHeader");
        if (scanTypeHeader is not null)
        {
            AnimationHelper.AnimateEntrance(scanTypeHeader, delayMs: 120, durationMs: 400);
        }

        // Four scan-type cards: staggered horizontal slide + fade.
        AnimationHelper.AnimateStaggeredHorizontal(
            new UIElement[] { QuickScanCard, FullScanCard, CustomScanCard, UsbScanCard },
            staggerMs: 90,
            initialDelayMs: 180,
            durationMs: 500,
            offsetX: 40f);

        var startButton = F<FrameworkElement>("StartScanButton");
        if (startButton is not null)
        {
            AnimationHelper.AnimateScaleIn(startButton, delayMs: 540, durationMs: 500);
        }

        var historyHeader = F<FrameworkElement>("ScanHistoryHeader");
        if (historyHeader is not null)
        {
            AnimationHelper.AnimateEntrance(historyHeader, delayMs: 620, durationMs: 450);
        }
        var historyCard = F<FrameworkElement>("ScanHistoryCard");
        if (historyCard is not null)
        {
            AnimationHelper.AnimateEntrance(historyCard, delayMs: 760, durationMs: 500);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ViewModel.CurrentFile):
                var currentFileText = F<FrameworkElement>("CurrentFileText");
                if (currentFileText is not null && ViewModel.IsScanning)
                {
                    AnimationHelper.FadeOpacity(currentFileText, 0.5f, 1.0f, durationMs: 200);
                }
                break;

            case nameof(ViewModel.ThreatsFound):
                var progressCard = F<FrameworkElement>("ScanProgressCard");
                if (ViewModel.ThreatsFound > _lastThreatsFound && progressCard is not null)
                {
                    AnimationHelper.FlashAccentColor(
                        progressCard,
                        Color.FromArgb(80, 244, 67, 54),
                        durationMs: 500);
                }
                _lastThreatsFound = ViewModel.ThreatsFound;
                break;

            case nameof(ViewModel.IsScanning):
                var progressBar = F<FrameworkElement>("ScanProgressBar");
                var shimmerHost = F<Panel>("ProgressShimmerHost");
                var scanLineOverlay = F<Panel>("ScanLineOverlay");
                var progressRing = F<FrameworkElement>("ScanProgressRing");
                if (ViewModel.IsScanning && !_wasScanning)
                {
                    _lastThreatsFound = 0;
                    if (progressBar is not null)
                    {
                        AnimationHelper.AnimateProgressShimmer(progressBar, durationMs: 1400);
                    }
                    if (shimmerHost is not null)
                    {
                        AnimationHelper.StartShimmerSweep(shimmerHost, durationMs: 1800);
                    }
                    if (scanLineOverlay is not null)
                    {
                        AnimationHelper.StartScanLinePass(
                            scanLineOverlay,
                            color: Color.FromArgb(180, 88, 166, 255),
                            passDurationMs: 1400,
                            passIntervalMs: 2500);
                    }
                    if (progressRing is not null)
                    {
                        AnimationHelper.StartGlowPulse(
                            progressRing,
                            Color.FromArgb(255, 88, 166, 255),
                            durationMs: 1800,
                            minBlur: 8f,
                            maxBlur: 20f,
                            minOpacity: 0.3f,
                            maxOpacity: 0.8f);
                    }
                }
                else if (!ViewModel.IsScanning && _wasScanning)
                {
                    if (progressBar is not null)
                    {
                        AnimationHelper.StopAnimation(progressBar, "Opacity");
                        progressBar.Opacity = 1.0;
                    }
                    if (shimmerHost is not null)
                    {
                        AnimationHelper.StopShimmerSweep(shimmerHost);
                    }
                    if (scanLineOverlay is not null)
                    {
                        AnimationHelper.StopScanLinePass(scanLineOverlay);
                    }
                    if (progressRing is not null)
                    {
                        AnimationHelper.StopGlowPulse(progressRing);
                    }
                }
                _wasScanning = ViewModel.IsScanning;
                break;

            case nameof(ViewModel.IsScanComplete):
                var completeCard = F<FrameworkElement>("ScanCompleteCard");
                if (ViewModel.IsScanComplete && completeCard is not null)
                {
                    AnimationHelper.AnimateBounce(completeCard, delayMs: 60, durationMs: 650);
                }
                break;
        }
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

            var selectedCard = ViewModel.SelectedScanType switch
            {
                ScanType.Quick => QuickScanCard,
                ScanType.Full => FullScanCard,
                ScanType.Custom => CustomScanCard,
                ScanType.USB => UsbScanCard,
                _ => QuickScanCard
            };
            AnimationHelper.AnimateCheckPulse(selectedCard);
        }
    }

    private void UpdateScanTypeCardStyles()
    {
        var accentBrush = (Microsoft.UI.Xaml.Media.SolidColorBrush)Application.Current.Resources["AccentBrush"];
        var borderBrush = (Microsoft.UI.Xaml.Media.SolidColorBrush)Application.Current.Resources["BorderBrush"];

        QuickScanCard.BorderBrush = borderBrush;
        QuickScanCard.BorderThickness = new Thickness(1);
        FullScanCard.BorderBrush = borderBrush;
        FullScanCard.BorderThickness = new Thickness(1);
        CustomScanCard.BorderBrush = borderBrush;
        CustomScanCard.BorderThickness = new Thickness(1);
        UsbScanCard.BorderBrush = borderBrush;
        UsbScanCard.BorderThickness = new Thickness(1);

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