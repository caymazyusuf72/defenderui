using System;
using System.Collections.Generic;
using DefenderUI.Helpers;
using DefenderUI.Services;
using DefenderUI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DefenderUI;

/// <summary>
/// Uygulamanın ana Shell penceresi.
///
/// Faz 2 itibarıyla:
///   • Custom title bar (logo + app adı + <see cref="Controls.StatusPill"/> +
///     theme toggle + bildirim butonu).
///   • <see cref="NavigationView"/> sol menüsü ile sayfa geçişleri
///     <see cref="INavigationService"/> üzerinden yönetilir.
///   • Tema geçişi <see cref="IThemeService"/> aracılığıyla yapılır.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;

    public MainWindow()
    {
        InitializeComponent();

        // ── DI resolve ────────────────────────────────────────────────
        _navigationService = App.Current.Services.GetRequiredService<INavigationService>();
        _themeService = App.Current.Services.GetRequiredService<IThemeService>();

        // ── Title bar ────────────────────────────────────────────────
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBarDragRegion);
        try
        {
            AppWindow.SetIcon("Assets/AppIcon.ico");
        }
        catch
        {
            // Unpackaged çalışmada fail olabilir; kritik değil.
        }

        // ── Navigation ────────────────────────────────────────────────
        _navigationService.Frame = ContentFrame;
        _navigationService.NavigateTo("dashboard");

        // ── Theme ─────────────────────────────────────────────────────
        if (RootGrid is not null)
        {
            _themeService.ApplyTheme(RootGrid);
        }
        UpdateThemeToggleIcon();
    }

    // ═════════════════════════════════════════════════════════════════
    // NavigationView
    // ═════════════════════════════════════════════════════════════════
    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        // Sol menü item'ları için staggered giriş animasyonu.
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

        try
        {
            AnimationHelper.AnimateStaggered(
                items,
                staggerMs: 60,
                initialDelayMs: 120,
                durationMs: 380,
                offsetY: 10f);
        }
        catch
        {
            // Animasyon başarısız olursa sessiz geç — shell çalışmaya devam etsin.
        }
    }

    private void NavView_SelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer is not NavigationViewItem selectedItem
            || selectedItem.Tag is not string tag)
        {
            return;
        }

        _navigationService.NavigateTo(tag);
    }

    // ═════════════════════════════════════════════════════════════════
    // Theme toggle
    // ═════════════════════════════════════════════════════════════════
    private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
    {
        // Sıralı döngü: Light → Dark → Default → Light …
        var next = _themeService.CurrentTheme switch
        {
            ElementTheme.Light => ElementTheme.Dark,
            ElementTheme.Dark => ElementTheme.Default,
            _ => ElementTheme.Light,
        };

        _themeService.SetTheme(next);

        if (RootGrid is not null)
        {
            _themeService.ApplyTheme(RootGrid);
        }

        UpdateThemeToggleIcon();
    }

    private void UpdateThemeToggleIcon()
    {
        if (ThemeToggleIcon is null)
        {
            return;
        }

        // Güneş (E706) = light, Ay (E708) = dark, PC/monitor (E770) = system.
        ThemeToggleIcon.Glyph = _themeService.CurrentTheme switch
        {
            ElementTheme.Light => "\uE706",
            ElementTheme.Dark => "\uE708",
            _ => "\uE770",
        };
    }
}
