﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using Avalonia.Controls.Shapes;
using Avalonia.Input;

namespace DesktopPetAvalonia;

public class PetWindow : Window
{
    private Ellipse petEllipse;
    private Random rand;
    private double petSpeed = 5;
    private DispatcherTimer moveTimer;
    
    private int hunger = 0;
    private int loneliness = 0;
    private const int MAX_STATE = 100;
    
    private bool isDead = false;
    private double targetX;
    private const double Speed = 5; // скорость перемещения окна
    
    // Добавление ProgressBar для отображения состояний
    private ProgressBar hungerProgressBar;
    private ProgressBar lonelinessProgressBar;
    private Button feedButton;
    private Button petButton;
    private bool isHovering = false;
    
    public PetWindow()
    {
        // Настройка окна
        this.Width = 200;
        this.Height = 200;
        this.Topmost = true;
        this.WindowStartupLocation = WindowStartupLocation.Manual;
        this.CanResize = false;

        // Убираем заголовок окна и рамки
        this.SystemDecorations = SystemDecorations.None;  // Убираем панель заголовка
        this.BorderBrush = Brushes.Transparent;
        this.BorderThickness = new Thickness(0);

        // Позиция окна - верхний левый угол экрана
        var screen = Screens.Primary;
        var bounds = screen.WorkingArea;
        this.Position = new PixelPoint((int)(bounds.Width - this.Width), (int)(bounds.Height - this.Height));

        // Настройка фона для прозрачности
        this.Background = Brushes.Transparent; // Окно будет полностью прозрачным, но питомец виден
        this.TransparencyLevelHint = [WindowTransparencyLevel.Transparent];

        
        isDead = false;

        // Создание панели и добавление питомца
        var panel = new Canvas();
        // Создание эллипса для питомца
        petEllipse = new Ellipse
        {
            Width = 50,
            Height = 50,
            Fill = Brushes.Red
        };
        panel.Children.Add(petEllipse);
        hungerProgressBar = new ProgressBar
        {
            Width = 150,
            Height = 20,
            Maximum = MAX_STATE,
            Value = hunger,
            IsVisible = false
        };
        panel.Children.Add(hungerProgressBar);
        
        lonelinessProgressBar = new ProgressBar
        {
            Width = 150,
            Height = 20,
            Maximum = MAX_STATE,
            Value = loneliness,
            IsVisible = false
        };
        panel.Children.Add(lonelinessProgressBar);
        
        // Добавление кнопок на панель:
        feedButton = new Button
        {
            Content = "Кормить",
            Width = 70,
            Height = 30,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
            IsVisible = false
        };
        feedButton.Click += OnFeedClick;
        panel.Children.Add(feedButton);
        
        petButton = new Button
        {
            Content = "Погладить",
            Width = 70,
            Height = 30,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
            IsVisible = false
        };
        petButton.Click += OnPetClick;
        panel.Children.Add(petButton);
        
        this.Content = panel;

        rand = new Random();
        moveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(30)
        };
        moveTimer.Tick += MovePet;
        moveTimer.Start();
        // Устанавливаем начальное положение окна питомца
        UpdateVerticalPosition();
        
    }
    private void OnFeedClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        hunger = 0;
        hungerProgressBar.Value = hunger;
    }

    private void OnPetClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        loneliness = 0;
        lonelinessProgressBar.Value = loneliness;
    }

    // Обработчики событий для отображения/скрытия всплывающих элементов
    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        isHovering = true;

        // Показываем кнопки и прогрессбары при наведении
        hungerProgressBar.IsVisible = true;
        lonelinessProgressBar.IsVisible = true;
        feedButton.IsVisible = true;
        petButton.IsVisible = true;
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        isHovering = false;

        // Прячем кнопки и прогрессбары, если мышь уходит
        hungerProgressBar.IsVisible = false;
        lonelinessProgressBar.IsVisible = false;
        feedButton.IsVisible = false;
        petButton.IsVisible = false;
    }

    // Получаем положение панели задач (также можно использовать SHAppBarMessage, как в предыдущем примере)
    private void UpdateVerticalPosition()
    {
        // Позиционируем окно питомца над панелью задач
        var screenBounds = Screens.Primary.Bounds;
        var taskbarHeight = 40; // Приблизительная высота панели задач (можно динамически получать)
        this.Position = new PixelPoint((int)(screenBounds.Width - this.Width) / 2, screenBounds.Bottom - taskbarHeight - (int)this.Height);
    }

    private void MovePet(object sender, EventArgs e)
    {
        if (isHovering)
        {
            return;
        }
        if (isDead)
        {
            moveTimer.Stop();
            return;
        }

        var currentX = this.Position.X;

        // Перемещаем окно питомца в случайную точку по горизонтали
        if (Math.Abs(currentX - targetX) < Speed)
        {
            targetX = rand.Next(0, (int)(Screens.Primary.Bounds.Width - this.Width));
        }

        // Двигаем окно в целевую точку по оси X
        var newX = currentX + (currentX < targetX ? Speed : -Speed);
        this.Position = new PixelPoint((int)newX, this.Position.Y);

        // Обновление состояний питомца
        hunger++;
        loneliness++;

        // Вызываем обновление состояний, например, через ProgressBar или другие компоненты.
        // Например:
        // hungerProgressBar.Value = hunger;
        // lonelinessProgressBar.Value = loneliness;

        if (hunger >= MAX_STATE && loneliness >= MAX_STATE)
        {
            PetDies();
        }
    }

    private void PetDies()
    {
        isDead = true;
        petEllipse.Fill = Brushes.Black;  // Меняем цвет на чёрный
        moveTimer.Stop();  // Останавливаем движение
        
        var messageBox = new Window
        {
            Width = 200,
            Height = 100,
            Content = new TextBlock
            {
                Text = "Питомец умер...",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            }
        };
        messageBox.ShowDialog(this);  // Показать окно при клике
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
