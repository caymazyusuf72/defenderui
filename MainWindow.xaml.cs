using System;
using System.Collections.Generic;
using DefenderUI.Helpers;
using DefenderUI.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace DefenderUI;

public sealed partial class MainWindow : Window
{
    private readonly Dictionary<string, Type> _pageMap = new()
    {
        { "dashboard", typeof(DashboardPage) },
        { "scan", typeof(ScanPage) },
        { "protection", typeof(ProtectionPage) },
        { "quarantine", typeof(QuarantinePage) },
        { "reports", typeof(ReportsPage) },
        { "update", typeof(UpdatePage) },
        { "settings", typeof(SettingsPage) },
    };

    // Defines navigation order so we can pick a direction for slide transitions.
    private static readonly Dictionary<string, int> _pageOrder = new()
    {
        { "dashboard", 0 },
        { "scan", 1 },
        { "protection", 2 },
        { "quarantine", 3 },
        { "reports", 4 },
        { "update", 5 },
        { "settings", 6 },
    };

    private string? _currentTag;

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBarGrid);
        AppWindow.SetIcon("Assets/AppIcon.ico");

        ContentFrame.Navigate(typeof(DashboardPage), null, new EntranceNavigationTransitionInfo());
        _currentTag = "dashboard";
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        // Staggered entrance for all visible NavigationViewItems.
        var items = new List<UIElement>();
        foreach (var menuItem in NavView.MenuItems)
        {
            if (menuItem is NavigationViewItem navItem)
            {
                items.Add(navItem);
            }
        }
        foreach (var footerItem in NavView.FooterMenuItems)
        {
            if (footerItem is NavigationViewItem navItem)
            {
                items.Add(navItem);
            }
        }

        AnimationHelper.AnimateStaggered(
            items,
            staggerMs: 60,
            initialDelayMs: 120,
            durationMs: 380,
            offsetY: 10f);
    }

    private void NavView_SelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer is not NavigationViewItem selectedItem
            || selectedItem.Tag is not string tag
            || !_pageMap.TryGetValue(tag, out var pageType))
        {
            return;
        }

        // Avoid re-navigating to the same page.
        if (tag == _currentTag)
        {
            return;
        }

        // Decide slide direction based on navigation order for a natural feel.
        NavigationTransitionInfo transition;
        if (_currentTag is not null
            && _pageOrder.TryGetValue(_currentTag, out var fromIndex)
            && _pageOrder.TryGetValue(tag, out var toIndex))
        {
            var effect = toIndex >= fromIndex
                ? SlideNavigationTransitionEffect.FromRight
                : SlideNavigationTransitionEffect.FromLeft;

            transition = new SlideNavigationTransitionInfo { Effect = effect };
        }
        else
        {
            transition = new EntranceNavigationTransitionInfo();
        }

        ContentFrame.Navigate(pageType, null, transition);
        _currentTag = tag;
    }
}
