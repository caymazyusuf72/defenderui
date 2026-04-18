using System;
using System.Collections.Generic;
using DefenderUI.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace DefenderUI.Services;

/// <summary>
/// <see cref="INavigationService"/>'in Shell-bağımsız implementasyonu.
///
/// Sayfa kayıtları (<see cref="PageMap"/>):
///   • dashboard   → <see cref="DashboardPage"/>
///   • scan        → <see cref="ScanPage"/>
///   • protection  → <see cref="ProtectionPage"/>
///   • quarantine  → <see cref="QuarantinePage"/>
///   • reports     → <see cref="ReportsPage"/>
///   • update      → <see cref="UpdatePage"/>
///   • settings    → <see cref="SettingsPage"/>
///
/// NOT: privacy / firewall / tools gibi henüz uygulanmamış sayfalar
/// kasıtlı olarak kayıt edilmemiştir; Faz 6'da eklenecektir.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private static readonly IReadOnlyDictionary<string, Type> PageMap =
        new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "dashboard",  typeof(DashboardPage) },
            { "scan",       typeof(ScanPage) },
            { "protection", typeof(ProtectionPage) },
            { "quarantine", typeof(QuarantinePage) },
            { "reports",    typeof(ReportsPage) },
            { "update",     typeof(UpdatePage) },
            { "settings",   typeof(SettingsPage) },
        };

    // Sayfa sıralaması — Slide transition yönünü belirlemek için.
    private static readonly IReadOnlyDictionary<string, int> PageOrder =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "dashboard",  0 },
            { "scan",       1 },
            { "protection", 2 },
            { "quarantine", 3 },
            { "reports",    4 },
            { "update",     5 },
            { "settings",   6 },
        };

    private string? _currentKey;

    public Frame? Frame { get; set; }

    public bool CanGoBack => Frame?.CanGoBack == true;

    public event EventHandler? Navigated;

    public bool NavigateTo(string pageKey, object? parameter = null)
    {
        if (string.IsNullOrWhiteSpace(pageKey) || Frame is null)
        {
            return false;
        }

        if (!PageMap.TryGetValue(pageKey, out var pageType))
        {
            return false;
        }

        // Aynı sayfa açıksa re-navigate etme.
        if (string.Equals(_currentKey, pageKey, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var transition = ResolveTransition(_currentKey, pageKey);

        var navigated = Frame.Navigate(pageType, parameter, transition);
        if (!navigated)
        {
            return false;
        }

        _currentKey = pageKey;
        Navigated?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool GoBack()
    {
        if (Frame is null || !Frame.CanGoBack)
        {
            return false;
        }

        Frame.GoBack();

        // Geri gittikten sonra aktif key'i senkron tutmaya çalış (best-effort):
        // Frame.CurrentSourcePageType → key eşlemesi.
        _currentKey = ResolveKey(Frame.CurrentSourcePageType);
        Navigated?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public void ClearHistory()
    {
        if (Frame is null)
        {
            return;
        }

        Frame.BackStack.Clear();
        Frame.ForwardStack.Clear();
    }

    private static NavigationTransitionInfo ResolveTransition(string? fromKey, string toKey)
    {
        if (fromKey is not null
            && PageOrder.TryGetValue(fromKey, out var fromIndex)
            && PageOrder.TryGetValue(toKey, out var toIndex))
        {
            var effect = toIndex >= fromIndex
                ? SlideNavigationTransitionEffect.FromRight
                : SlideNavigationTransitionEffect.FromLeft;

            return new SlideNavigationTransitionInfo { Effect = effect };
        }

        return new EntranceNavigationTransitionInfo();
    }

    private static string? ResolveKey(Type? pageType)
    {
        if (pageType is null)
        {
            return null;
        }

        foreach (var kvp in PageMap)
        {
            if (kvp.Value == pageType)
            {
                return kvp.Key;
            }
        }

        return null;
    }
}