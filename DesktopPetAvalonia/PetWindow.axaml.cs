using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Platform;
using Avalonia.Remote.Protocol.Viewport;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace DesktopPet
{
    public partial class PetWindow : Window
    {
        private readonly PetState _petState;
        private readonly DispatcherTimer _moveTimer;
        private readonly DispatcherTimer _stateTimer;
        private readonly DispatcherTimer _animationTimer;
        private readonly Random _random = new Random();
        
        private List<Bitmap> _petFrames = new List<Bitmap>();
        private int _currentFrame = 0;
        
        private double _currentX;
        private double _targetX;
        private bool _isPaused;
        private int _pauseTicks;
        
        private const int MOVE_INTERVAL = 50; // мс
        private const double SPEED = 5; // пикселей за тик
        
        private PopupWindow? _popupWindow;

        bool isMouseOver = false;
        public PetWindow()
        {
            InitializeComponent();
            // Загрузка изображений питомца
            LoadPetFrames();
            
            // Инициализация состояния
            _petState = new PetState();
            _petState.StateChanged += OnPetStateChanged;
            _petState.PetDied += OnPetDied;
            
            // Инициализация позиции
            var screenBounds = Screens.Primary!.Bounds;
            _currentX = _random.Next((int)screenBounds.X, (int)(screenBounds.Width - Width));
            _targetX = _currentX;
            Position = new PixelPoint((int)_currentX, (int)(screenBounds.Height - Height));
            
            // Таймер для движения
            _moveTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(MOVE_INTERVAL) };
            _moveTimer.Tick += MoveTimer_Tick;
            _moveTimer.Start();
            
            // Таймер для обновления состояний
            _stateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _stateTimer.Tick += StateTimer_Tick;
            _stateTimer.Start();
            
            // Таймер для анимации
            _animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            _animationTimer.Tick += AnimationTimer_Tick;
            _animationTimer.Start();
            
            // Обработчики событий мыши
            PointerEntered += PetWindow_PointerEntered;
            PointerExited += PetWindow_PointerExited;
            
            // Инициализируем всплывающее окно
            _popupWindow = new PopupWindow();
            
            // Установка начального изображения
            if (_petFrames.Count > 0)
            {
                PetImage.Source = _petFrames[0];
            }
        }
        
        private void LoadPetFrames()
        {
            try
            {
                // Проверяем наличие папки и создаем, если её нет
                string imageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Capy");
                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                    // Здесь можно создать базовые изображения, если папка была только что создана
                    CreateDefaultPetImages(imageFolder);
                }
                
                // Загружаем существующие изображения
                for (int i = 0; i < 2; i++)
                {
                    string imagePath = Path.Combine(imageFolder, $"frame_{i}.png");
                    if (File.Exists(imagePath))
                    {
                        _petFrames.Add(new Bitmap(imagePath));
                    }
                }
                
                // Если изображения не найдены, создаем базовые
                if (_petFrames.Count == 0)
                {
                    CreateDefaultPetImages(imageFolder);
                    // И загружаем их снова
                    for (int i = 0; i < 2; i++)
                    {
                        string imagePath = Path.Combine(imageFolder, $"frame_{i}.png");
                        if (File.Exists(imagePath))
                        {
                            _petFrames.Add(new Bitmap(imagePath));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке изображений: {ex.Message}");
                // Создаем заглушку, если не удалось загрузить изображения
                if (_petFrames.Count == 0)
                {
                    var bitmap = new WriteableBitmap(new PixelSize(100, 100), new Vector(96, 96));
                    _petFrames.Add(bitmap);
                }
            }
        }
        
        private void CreateDefaultPetImages(string folder)
        {
            // Здесь должна быть логика создания базовых изображений
            // Но в минимальной реализации можно просто оставить заглушкой
            Console.WriteLine("Базовые изображения питомца не найдены.");
        }
        
        private void MoveTimer_Tick(object? sender, EventArgs e)
        {
            if (_petState.IsDead || isMouseOver)
                return;
                
            var screenBounds = Screens.Primary!.Bounds;
            
            if (_isPaused)
            {
                _pauseTicks--;
                if (_pauseTicks <= 0)
                {
                    _targetX = _random.Next((int)screenBounds.X, (int)(screenBounds.Width - Width));
                    _isPaused = false;
                }
            }
            else
            {
                if (Math.Abs(_currentX - _targetX) < SPEED)
                {
                    _currentX = _targetX;
                    _pauseTicks = _random.Next(20, 60);
                    _isPaused = true;
                }
                else
                {
                    _currentX += (_currentX < _targetX) ? SPEED : -SPEED;
                }
            }
            
            Position = new PixelPoint((int)_currentX, Position.Y);
        }
        
        private void StateTimer_Tick(object? sender, EventArgs e)
        {
            _petState.UpdateState();
        }
        
        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            if (_petFrames.Count > 1)
            {
                _currentFrame = (_currentFrame + 1) % _petFrames.Count;
                PetImage.Source = _petFrames[_currentFrame];
            }
        }
        private void PetWindow_PointerEntered(object? sender, PointerEventArgs e)
        {
            isMouseOver = true;
            ButtonsPanel.IsVisible = !_petState.IsDead;
            BuryButton.IsVisible = _petState.IsDead;
            
            // Позиционируем всплывающее окно
            if (_popupWindow != null)
            {
                var position = Position;
                new PixelPoint(
                    (int)Math.Max(0, 
                        Math.Min(
                            (double)(Screens.Primary?.Bounds.Width - _popupWindow.Width),
                            position.X + (this.Width - _popupWindow.Width) / 2)
                    ),
                (int)Math.Max(0, position.Y - _popupWindow.Height - 5)
                    );
                _popupWindow.Position = new PixelPoint((int)(position.X -_popupWindow.Width/2 + this.Width/2), position.Y - 110);
                _popupWindow.UpdateState(_petState.Hunger, _petState.Loneliness, PetState.MAX_HUNGER, PetState.MAX_LONELINESS);
                _popupWindow.Show();
            }
        }
        
        private void PetWindow_PointerExited(object? sender, PointerEventArgs e)
        {
            isMouseOver = false;
            ButtonsPanel.IsVisible = false;
            BuryButton.IsVisible = false;
            
            if (_popupWindow != null && _popupWindow.IsVisible)
            {
                _popupWindow.Hide();
            }
        }
        
        private void FeedButton_Click(object? sender, RoutedEventArgs e)
        {
            _petState.PerformAction(PetAction.Feed);
        }
        
        private void PetButton_Click(object? sender, RoutedEventArgs e)
        {
            _petState.PerformAction(PetAction.Pet);
        }
        
        private void BuryButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_petState.IsDead)
            {
                PetHistory.AddDeadPet(_petState.GetLifespan());
                _petState.Reset();
                ButtonsPanel.IsVisible = true;
                BuryButton.IsVisible = false;
            }
        }
        
        private void OnPetStateChanged(object? sender, EventArgs e)
        {
            if (_popupWindow != null && _popupWindow.IsVisible)
            {
                _popupWindow.UpdateState(_petState.Hunger, _petState.Loneliness, PetState.MAX_HUNGER, PetState.MAX_LONELINESS);
            }
            
            // Если питомец мертв, меняем его внешний вид
            if (_petState.IsDead)
            {
                PetImage.Opacity = 0.5;
            }
            else
            {
                PetImage.Opacity = 1.0;
            }
        }
        
        private void OnPetDied(object? sender, EventArgs e)
        {
            _moveTimer.Stop();
            _animationTimer.Stop();
            
            ButtonsPanel.IsVisible = false;
            BuryButton.IsVisible = true;
            PetImage.Opacity = 0.5;
            /*
            // Отображаем сообщение о смерти питомца
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                "Питомец умер", 
                "Ваш питомец умер... Нажмите 'Похоронить', чтобы заменить его на нового.",
                ButtonEnum.Ok);

            messageBox.ShowAsync();*/
        }
        
        protected override void OnClosed(EventArgs e)
        {
            _moveTimer.Stop();
            _stateTimer.Stop();
            _animationTimer.Stop();
            
            if (_popupWindow != null && _popupWindow.IsVisible)
            {
                _popupWindow.Close();
            }
            
            // Освобождаем ресурсы изображений
            foreach (var frame in _petFrames)
            {
                frame.Dispose();
            }
            
            base.OnClosed(e);
        }
    }
}