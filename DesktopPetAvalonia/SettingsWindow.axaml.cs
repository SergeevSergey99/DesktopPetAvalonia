using Avalonia;
using Avalonia.Controls;
using System.Text;

namespace DesktopPet
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            UpdateInfo();
        }
        
        public void UpdateInfo()
        {
            StringBuilder info = new StringBuilder();
            info.AppendLine($"Количество мертвых питомцев: {PetHistory.DeadPetLifespans.Count}");
            
            for (int i = 0; i < PetHistory.DeadPetLifespans.Count; i++)
            {
                info.AppendLine($"Питомец {i + 1}: {PetHistory.DeadPetLifespans[i].TotalSeconds:F0} секунд");
            }
            
            InfoTextBlock.Text = info.ToString();
        }
    }
}