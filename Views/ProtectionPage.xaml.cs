using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DefenderUI.ViewModels;

namespace DefenderUI.Views;

public sealed partial class ProtectionPage : Page
{
    public ProtectionViewModel ViewModel { get; }

    public ProtectionPage()
    {
        ViewModel = App.Current.Services.GetRequiredService<ProtectionViewModel>();
        InitializeComponent();
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
        }
    }
}