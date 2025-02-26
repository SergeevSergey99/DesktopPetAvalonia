using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using System;
using MsBox.Avalonia.ViewModels.Commands;

namespace DesktopPet
{
    public class TrayManager
    {
        private static TrayIcon? _trayIcon;
        private static SettingsWindow? _settingsWindow;
        
        /*public static void Initialize()
        {
            _trayIcon = new TrayIcon()
            {
                ToolTipText = "Desktop Pet",
                Icon = new WindowIcon("Images/Capy/frame_0.png"),
                IsVisible = true,
            };
            
            // Создание меню трея
            var menu = new NativeMenu();
            
            var settingsItem = new NativeMenuItem("Настройки");
            settingsItem.Click += OnSettingsClicked;
            menu.Add(settingsItem);
            
            var exitItem = new NativeMenuItem("Выход");
            exitItem.Click += OnExitClicked;
            menu.Add(exitItem);
            
            _trayIcon.Menu = menu;
        }*/


        public void OnSettingsClicked(object? sender, EventArgs e)
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
        
        public void OnExitClicked(object? sender, EventArgs e)
        {
            /*if (_trayIcon != null)
            {
                _trayIcon.IsVisible = false;
                _trayIcon.Dispose();
            }*/
            
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }
    }
}