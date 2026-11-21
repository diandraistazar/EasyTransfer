using System.Net.WebSockets;
using System.Runtime.Serialization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using EasyTransfer.Desktop.ViewModels;
using EasyTransfer.Desktop.Views;

namespace EasyTransfer.Desktop;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow mw = new();
            MainViewModel vm = new(mw.StorageProvider);
            
            mw.DataContext = vm;
            desktop.MainWindow = mw;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
