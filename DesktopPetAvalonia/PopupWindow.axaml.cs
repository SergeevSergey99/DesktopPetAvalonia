using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace DesktopPet
{
    public partial class PopupWindow : Window
    {
        public PopupWindow()
        {
            InitializeComponent();
        }
        
        public void UpdateState(int hunger, int loneliness, int maxHunger, int maxLoneliness)
        {
            // Вычисляем проценты заполнения от 0 до 100
            double hungerPercentage = ((double)hunger / maxHunger) * 100;
            double lonelinessPercentage = ((double)loneliness / maxLoneliness) * 100;
    
            // Устанавливаем значения для ProgressBar
            HungerBar.Value = hungerPercentage;
            LonelinessBar.Value = lonelinessPercentage;
        }
    }
}