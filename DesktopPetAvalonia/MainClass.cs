using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System;

namespace DesktopPet
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var builder = BuildAvaloniaApp();
            
            builder.AfterSetup(afterSetupCallback);
            
            builder.StartWithClassicDesktopLifetime(args);
        }
        
        private static void afterSetupCallback(AppBuilder builder)
        {
            TrayManager.Initialize();
        }
        
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}