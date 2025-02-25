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
            double hungerWidth = ((double)hunger / maxHunger) * (Width - 20);
            double lonelinessWidth = ((double)loneliness / maxLoneliness) * (Width - 20);
            
            HungerBar.Width = hungerWidth;
            LonelinessBar.Width = lonelinessWidth;
        }
    }
}