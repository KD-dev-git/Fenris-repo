using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Fenris_TaskApp
{
    /// <summary>
    /// Interaction logic for Task2_Motion2CoachDesign.xaml
    /// </summary>
    public partial class Task2_Motion2CoachDesign : Window
    {
        public Task2_Motion2CoachDesign()
        {
            InitializeComponent();

            SwingTypeComboBox.ItemsSource = new string[] { "ALL", "FULL SWING", "SHORT GAME", "PUTTER", "BUNKER" };
            OrientationComboBox.ItemsSource = new string[] { "ALL", "FACE ON", "DOWN THE LINE", "BOTH" };
            ClubTypeComboBox.ItemsSource = new string[] { "ALL", "DRIVER", "IRON", "WEDGE" };
            DeviceTypeComboBox.ItemsSource = new string[] { "ALL", "ANDROID", "IOS", "DESKTOP" };

            SwingTypeComboBox.IsTextSearchEnabled = true;
            OrientationComboBox.IsTextSearchEnabled = true;
            ClubTypeComboBox.IsTextSearchEnabled = true;
            DeviceTypeComboBox.IsTextSearchEnabled = true;
        }
    }
}
