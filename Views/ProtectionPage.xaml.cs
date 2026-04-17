using DefenderUI.Helpers;
using DefenderUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DefenderUI.Views;

public sealed partial class ProtectionPage : Page
{
    private bool _hasAnimated;

    public ProtectionViewModel ViewModel { get; }

    public ProtectionPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ProtectionViewModel>();
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

        var heroCard = F<FrameworkElement>("ProtectionHeroCard");
        if (heroCard is not null)
        {
            AnimationHelper.AnimateScaleIn(heroCard, delayMs: 80, durationMs: 550);
        }

        var modulesHeader = F<FrameworkElement>("ModulesHeader");
        if (modulesHeader is not null)
        {
            AnimationHelper.AnimateEntrance(modulesHeader, delayMs: 220, durationMs: 400);
        }

        // 6 module cards: staggered entrance (if named).
        double moduleDelay = 300;
        for (int i = 1; i <= 6; i++)
        {
            var card = F<UIElement>($"Module{i}Card");
            if (card is not null)
            {
                AnimationHelper.AnimateEntrance(card, delayMs: moduleDelay, durationMs: 500, offsetY: 22f);
            }
            moduleDelay += 70;
        }

        var advHeader = F<FrameworkElement>("AdvancedHeader");
        if (advHeader is not null)
        {
            AnimationHelper.AnimateEntrance(advHeader, delayMs: 880, durationMs: 400);
        }
        var advGrid = F<FrameworkElement>("AdvancedGrid");
        if (advGrid is not null)
        {
            AnimationHelper.AnimateEntrance(advGrid, delayMs: 960, durationMs: 500);
        }

        var fwHeader = F<FrameworkElement>("FirewallHeader");
        if (fwHeader is not null)
        {
            AnimationHelper.AnimateEntrance(fwHeader, delayMs: 1060, durationMs: 400);
        }
        var fwCard = F<FrameworkElement>("FirewallCard");
        if (fwCard is not null)
        {
            AnimationHelper.AnimateEntrance(fwCard, delayMs: 1140, durationMs: 500);
        }
    }

    private void OnModuleToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch && toggleSwitch.Tag is string tagStr && int.TryParse(tagStr, out int index))
        {
            if (index >= 0 && index < ViewModel.ProtectionModules.Count)
            {
                var module = ViewModel.ProtectionModules[index];
                ViewModel.ToggleModuleCommand.Execute(module);
            }

            // Check pulse on the parent card of the toggle (if named).
            var card = F<FrameworkElement>($"Module{index + 1}Card");
            if (card is not null)
            {
                AnimationHelper.AnimateCheckPulse(card);
            }
        }
    }
}