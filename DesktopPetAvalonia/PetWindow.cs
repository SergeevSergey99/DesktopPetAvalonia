using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using System;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using Avalonia.Threading;

namespace DesktopPetAvalonia;

public class PetWindow : Window
{
    private Ellipse petEllipse;
    private Random rand;
    private double petSpeed = 5;
    private DispatcherTimer moveTimer;

    public PetWindow()
    {
        // Настройка окна
        this.Width = 200;
        this.Height = 200;
        this.Topmost = true;
        this.WindowStartupLocation = WindowStartupLocation.Manual;
        this.CanResize = false;
        
        // Настройка стиля окна
        this.BorderBrush = Brushes.Transparent;
        this.BorderThickness = new Thickness(0);
        
        // Убираем заголовок окна
        this.SystemDecorations = SystemDecorations.None;
        
        // Позиция окна - верхний левый угол экрана
        var screen = Screens.Primary;
        var bounds = screen.WorkingArea;
        this.Position = new PixelPoint((int)(bounds.Width - this.Width), (int)(bounds.Height - this.Height));

        // Настройка фона для прозрачности
        this.Background = Brushes.Transparent;
        this.TransparencyLevelHint = [WindowTransparencyLevel.Transparent];
        
        this.PointerPressed += OnPetClick;
        
        // Создание эллипса для питомца
        petEllipse = new Ellipse
        {
            Width = 50,
            Height = 50,
            Fill = Brushes.Red
        };

        // Создание панели и добавление питомца
        var panel = new Canvas();
        panel.Children.Add(petEllipse);
        this.Content = panel;

        rand = new Random();
        moveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };
        moveTimer.Tick += MovePet;
        moveTimer.Start();
    }

    private void MovePet(object sender, EventArgs e)
    {
        // Получаем текущие координаты питомца
        var x = petEllipse.RenderTransform is TranslateTransform translateTransform ? translateTransform.X : 0;
        var y = petEllipse.RenderTransform is TranslateTransform translateTransform2 ? translateTransform2.Y : 0;

        // Новые случайные координаты
        var newX = x + rand.Next(-5, 6);
        var newY = y + rand.Next(-5, 6);

        // Обновление позиции питомца
        petEllipse.RenderTransform = new TranslateTransform(newX, newY);
    }

    private void OnPetClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Вставляем простое действие при клике на питомца// Вставляем простое действие при клике на питомца
        var messageBox = new Window
        {
            Width = 200,
            Height = 100,
            Content = new TextBlock
            {
                Text = "Вы нажали на питомца!",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            }
        };
        messageBox.ShowDialog(this);
    }

    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Создание приложения Avalonia
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .UseSkia();
}
