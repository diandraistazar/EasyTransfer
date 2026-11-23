using System;
using Avalonia;

using EasyTransfer.Core.Logging;

namespace EasyTransfer.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Log.Info("GUI session is running ...");
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithInterFont()
        .LogToTrace();
    }
}
