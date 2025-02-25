using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using System;

namespace DesktopPet
{
    public class TrayManager
    {
        private static TrayIcon? _trayIcon;
        private static SettingsWindow? _settingsWindow;
        
        public static void Initialize()
        {
            if (true)
            {
                _trayIcon = new TrayIcon();
                
                // Создание меню трея
                var menu = new NativeMenu();
                
                var settingsItem = new NativeMenuItem("Настройки");
                settingsItem.Click += OnSettingsClicked;
                menu.Add(settingsItem);
                
                var exitItem = new NativeMenuItem("Выход");
                exitItem.Click += OnExitClicked;
                menu.Add(exitItem);
                
                _trayIcon.Menu = menu;
                _trayIcon.IsVisible = true;
                
                // Установка иконки (заглушка, так как в Avalonia это немного сложнее)
                // В реальном приложении вам нужно будет создать и использовать настоящую иконку
            }
        }
        
        private static void OnSettingsClicked(object? sender, EventArgs e)
        {
            if (_settingsWindow == null || !_settingsWindow.IsVisible)
            {
                _settingsWindow = new SettingsWindow();
                _settingsWindow.Show();
            }
            else
            {
                _settingsWindow.UpdateInfo();
                _settingsWindow.Activate();
            }
        }
        
        private static void OnExitClicked(object? sender, EventArgs e)
        {
            if (_trayIcon != null)
            {
                _trayIcon.IsVisible = false;
                _trayIcon.Dispose();
            }
            
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }
    }
}