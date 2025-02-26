using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace DesktopPet
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new PetWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
        TrayManager trayManager = new TrayManager();

        private void Settings_Click(object? sender, EventArgs eventArgs) => trayManager.OnSettingsClicked(sender, eventArgs);
        private void Exit_Click(object? sender, EventArgs eventArgs) => trayManager.OnExitClicked(sender, eventArgs);
    }
}