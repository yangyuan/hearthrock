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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hearthrock.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void PatchButton_Click(object sender, RoutedEventArgs e)
        {
            PatchButton.IsEnabled = false;

            var patcher = new Hacking.PatchHelper();
            var path = string.Empty;
            try
            {
                path = await patcher.SearchHearthstoneDirectoryAsync();
            }
            catch
            {
                MessageBox.Show("Cannot find Hearthstone Directory.");
                PatchButton.IsEnabled = true;
                return;
            }

            try
            {
                await patcher.RecoverHearthstoneAsync(path, Environment.CurrentDirectory);
            }
            catch
            {
                MessageBox.Show("Hearthstone broken, or Hearthrock out of date.");
                PatchButton.IsEnabled = true;
                return;
            }

            await patcher.InjectAsync(path);
            PatchButton.IsEnabled = true;
        }

        private async void UnpatchButton_Click(object sender, RoutedEventArgs e)
        {
            UnpatchButton.IsEnabled = false;

            var patcher = new Hacking.PatchHelper();
            var path = string.Empty;
            try
            {
                path = await patcher.SearchHearthstoneDirectoryAsync();
            }
            catch
            {
                MessageBox.Show("Cannot find Hearthstone Directory.");
                PatchButton.IsEnabled = true;
                return;
            }

            try
            {
                await patcher.RecoverHearthstoneAsync(path, Environment.CurrentDirectory);
            }
            catch
            {
                MessageBox.Show("Hearthstone broken, or Hearthrock out of date.");
                UnpatchButton.IsEnabled = true;
                return;
            }

            UnpatchButton.IsEnabled = true;
        }

        private void DebugButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
