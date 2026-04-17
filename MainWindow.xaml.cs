using System;
using System.Collections.Generic;
using DefenderUI.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBarGrid);
        AppWindow.SetIcon("Assets/AppIcon.ico");

        ContentFrame.Navigate(typeof(DashboardPage));
    }

    private void NavView_SelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer is NavigationViewItem selectedItem
            && selectedItem.Tag is string tag
            && _pageMap.TryGetValue(tag, out var pageType))
        {
            ContentFrame.Navigate(pageType);
        }
    }
}
