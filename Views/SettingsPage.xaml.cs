using System.Collections.Generic;
using DefenderUI.Helpers;
using DefenderUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DefenderUI.Views;

public sealed partial class SettingsPage : Page
{
    private bool _hasAnimated;

    public SettingsViewModel ViewModel { get; }

    public SettingsPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<SettingsViewModel>();
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

        string[] categoryNames =
        {
            "CategoryGeneral",
            "CategoryProtection",
            "CategoryNotifications",
            "CategoryScheduled",
            "CategoryExclusions",
            "CategoryAppearance",
            "CategoryPrivacy",
            "CategoryAbout",
        };

        var categories = new List<UIElement>();
        foreach (var name in categoryNames)
        {
            var cat = F<UIElement>(name);
            if (cat is not null)
            {
                categories.Add(cat);
            }
        }

        AnimationHelper.AnimateStaggered(
            categories,
            staggerMs: 80,
            initialDelayMs: 150,
            durationMs: 500,
            offsetY: 24f);
    }
}