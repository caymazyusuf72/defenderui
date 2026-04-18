using System;

namespace DefenderUI.Services;

/// <summary>
/// <see cref="IToastService"/>'in varsayılan implementasyonu. Event tabanlı
/// yayınlayıcı (pub/sub); görsel gösterim host UI (MainWindow vb.)
/// sorumluluğundadır (Faz 7'de polish edilecek).
/// </summary>
public sealed class ToastService : IToastService
{
    public event EventHandler<ToastMessage>? ToastRequested;

    public void Show(ToastMessage toast)
    {
        ArgumentNullException.ThrowIfNull(toast);
        ToastRequested?.Invoke(this, toast);
    }

    public void Info(string title, string? body = null)
        => Show(new ToastMessage(title, body, ToastSeverity.Info));

    public void Success(string title, string? body = null)
        => Show(new ToastMessage(title, body, ToastSeverity.Success));

    public void Warning(string title, string? body = null)
        => Show(new ToastMessage(title, body, ToastSeverity.Warning));

    public void Error(string title, string? body = null)
        => Show(new ToastMessage(title, body, ToastSeverity.Error));
}