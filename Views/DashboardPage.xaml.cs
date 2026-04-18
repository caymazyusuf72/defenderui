using System;
using DefenderUI.Controls;
using DefenderUI.Models;
using DefenderUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace DefenderUI.Views;

/// <summary>
/// Dashboard (Faz 3). MVVM odaklı — code-behind yalnızca DI ile ViewModel
/// enjeksiyonu ve XAML x:Bind için küçük saf yardımcı metodları barındırır.
/// </summary>
public sealed partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel { get; }

    public DashboardPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<DashboardViewModel>();
        InitializeComponent();
    }

    // ═════════════════════════════════════════════════════════════════
    // Static helpers — x:Bind DataTemplate içinde kullanılır.
    // ═════════════════════════════════════════════════════════════════

    public static string GetActivityGlyph(ActivityType type) => type switch
    {
        ActivityType.ThreatBlocked => "\uE730",       // Shield + block
        ActivityType.ScanCompleted => "\uE73E",       // CheckMark
        ActivityType.DatabaseUpdated => "\uE895",     // Sync
        ActivityType.FileQuarantined => "\uE7B8",     // Lock
        ActivityType.ProtectionEnabled => "\uE72E",   // Shield
        ActivityType.ProtectionDisabled => "\uE7BA",  // Warning
        ActivityType.Warning => "\uE7BA",             // Warning
        _ => "\uE946"                                 // Info
    };

    public static ActivitySeverity GetActivitySeverity(ActivityType type) => type switch
    {
        ActivityType.ThreatBlocked => ActivitySeverity.Error,
        ActivityType.ScanCompleted => ActivitySeverity.Success,
        ActivityType.DatabaseUpdated => ActivitySeverity.Info,
        ActivityType.FileQuarantined => ActivitySeverity.Warning,
        ActivityType.ProtectionEnabled => ActivitySeverity.Success,
        ActivityType.ProtectionDisabled => ActivitySeverity.Warning,
        ActivityType.Warning => ActivitySeverity.Warning,
        _ => ActivitySeverity.Info
    };

    public static string FormatRelativeTime(DateTime timestamp)
    {
        var delta = DateTime.Now - timestamp;
        if (delta.TotalMinutes < 1)
        {
            return "az önce";
        }
        if (delta.TotalMinutes < 60)
        {
            return $"{(int)delta.TotalMinutes} dakika önce";
        }
        if (delta.TotalHours < 24)
        {
            return $"{(int)delta.TotalHours} saat önce";
        }
        if (delta.TotalDays < 7)
        {
            return $"{(int)delta.TotalDays} gün önce";
        }
        return timestamp.ToString("dd MMM yyyy");
    }
}