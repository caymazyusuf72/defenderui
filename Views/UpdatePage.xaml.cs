using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using DefenderUI.ViewModels;
using Microsoft.UI.Dispatching;

namespace DefenderUI.Views;

public sealed partial class UpdatePage : Page
{
    public UpdateViewModel ViewModel { get; }

    public UpdatePage()
    {
        ViewModel = App.Current.Services.GetRequiredService<UpdateViewModel>();
        InitializeComponent();
        ViewModel.SetDispatcherQueue(DispatcherQueue.GetForCurrentThread());
    }
}