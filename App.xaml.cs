using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using DefenderUI.Services;
using DefenderUI.ViewModels;

namespace DefenderUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private Window? _window;

    /// <summary>
    /// Gets the current <see cref="App"/> instance.
    /// </summary>
    public static new App Current => (App)Application.Current;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> for the application.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// Initializes the singleton application object.
    /// </summary>
    public App()
    {
        Services = ConfigureServices();
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Services
        services.AddSingleton<MockDataService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IToastService, ToastService>();
        services.AddSingleton<IScanService, ScanService>();

        // ViewModels
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ScanViewModel>();
        services.AddTransient<ProtectionViewModel>();
        services.AddTransient<QuarantineViewModel>();
        services.AddTransient<ReportsViewModel>();
        services.AddTransient<UpdateViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<PrivacyViewModel>();
        services.AddTransient<FirewallViewModel>();
        services.AddTransient<ToolsViewModel>();
        services.AddTransient<PasswordManagerViewModel>();
        services.AddTransient<VpnViewModel>();

        // Pages (DI tarafından NavigationService / test'ler için opsiyonel resolve)
        services.AddTransient<Views.PrivacyPage>();
        services.AddTransient<Views.FirewallPage>();
        services.AddTransient<Views.ToolsPage>();
        services.AddTransient<Views.PasswordManagerPage>();
        services.AddTransient<Views.VpnPage>();

        return services.BuildServiceProvider();
    }
}
