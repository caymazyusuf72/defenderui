using DefenderUI.Helpers;
using DefenderUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DefenderUI.Views;

public sealed partial class UpdatePage : Page
{
    private bool _hasAnimated;

    public UpdateViewModel ViewModel { get; }

    public UpdatePage()
    {
        ViewModel = App.Current.Services.GetRequiredService<UpdateViewModel>();
        InitializeComponent();
        ViewModel.SetDispatcherQueue(DispatcherQueue.GetForCurrentThread());
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

        var hero = F<FrameworkElement>("HeroCard");
        if (hero is not null)
        {
            AnimationHelper.AnimateScaleIn(hero, delayMs: 120, durationMs: 550);

            if (ViewModel.IsUpdateAvailable && !ViewModel.IsUpdating)
            {
                AnimationHelper.StartPulse(hero, minScale: 1.0f, maxScale: 1.015f, durationMs: 2400);
            }
        }

        var virusCard = F<FrameworkElement>("VirusDefinitionsCard");
        if (virusCard is not null)
        {
            AnimationHelper.AnimateEntrance(virusCard, delayMs: 280, durationMs: 500);
        }
        var appCard = F<FrameworkElement>("ApplicationCard");
        if (appCard is not null)
        {
            AnimationHelper.AnimateEntrance(appCard, delayMs: 360, durationMs: 500);
        }
        var settingsCard = F<FrameworkElement>("UpdateSettingsCard");
        if (settingsCard is not null)
        {
            AnimationHelper.AnimateEntrance(settingsCard, delayMs: 520, durationMs: 500);
        }
        var historyCard = F<FrameworkElement>("HistoryCard");
        if (historyCard is not null)
        {
            AnimationHelper.AnimateEntrance(historyCard, delayMs: 700, durationMs: 550);
        }
    }
}