using Hearthrock.Contracts;
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
        RockConfiguration configuration = new RockConfiguration();

        public MainWindow()
        {
            InitializeComponent();
            InitializeConfigurationAsync();
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

        private async void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            DebugButton.IsEnabled = false;

            var patcher = new Hacking.PatchHelper();
            var path = string.Empty;
            try
            {
                path = await patcher.SearchHearthstoneDirectoryAsync();
            }
            catch
            {
                MessageBox.Show("Cannot find Hearthstone Directory.");
                DebugButton.IsEnabled = true;
                return;
            }

            switch (TraceComboBox.SelectedIndex)
            {
                default:
                case 0:
                    configuration.TraceEndpoint = string.Empty;
                    break;
                case 1:
                    configuration.TraceEndpoint = RockConstants.DefaultEndpoint + RockConstants.DefaultTracePath;
                    break;
                case 2:
                    configuration.TraceEndpoint = TraceTextBox.Text;
                    break;
            }

            switch (BotComboBox.SelectedIndex)
            {
                default:
                case 0:
                    configuration.BotEndpoint = string.Empty;
                    break;
                case 1:
                    configuration.BotEndpoint = RockConstants.DefaultEndpoint + RockConstants.DefaultBotPath;
                    break;
                case 2:
                    configuration.BotEndpoint = configuration.BotEndpoint;
                    break;
            }

            await patcher.WriteConfigurationAsync(path, configuration);
            InitializeConfigurationAsync();

            DebugButton.IsEnabled = true;
        }

        private async void InitializeConfigurationAsync()
        {
            var patcher = new Hacking.PatchHelper();
            var path = string.Empty;
            try
            {
                path = await patcher.SearchHearthstoneDirectoryAsync();
                this.configuration = await patcher.ReadConfigurationAsync(path);
            }
            catch
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(configuration.TraceEndpoint))
            {
                TraceComboBox.SelectedIndex = 0;
            }

            try
            {
                var defaultTraceEndpoint = new Uri(RockConstants.DefaultEndpoint + RockConstants.DefaultTracePath);
                var traceEndpoint = new Uri(configuration.TraceEndpoint);
                if (traceEndpoint.AbsoluteUri == defaultTraceEndpoint.AbsoluteUri)
                {
                    TraceComboBox.SelectedIndex = 1;
                }
                else
                {
                    TraceComboBox.SelectedIndex = 2;
                }
            }
            catch { }

            if (string.IsNullOrWhiteSpace(configuration.BotEndpoint))
            {
                BotComboBox.SelectedIndex = 0;
            }

            try
            {
                var defaultBotEndpoint = new Uri(RockConstants.DefaultEndpoint + RockConstants.DefaultBotPath);
                var botEndpoint = new Uri(configuration.BotEndpoint);
                if (botEndpoint.AbsoluteUri == defaultBotEndpoint.AbsoluteUri)
                {
                    BotComboBox.SelectedIndex = 1;
                }
                else
                {
                    BotComboBox.SelectedIndex = 2;
                }
            }
            catch { }

            DeckComboBox.SelectedIndex = 0;
            DeckComboBox.IsEnabled = false;

            OpponentComboBox.SelectedIndex = 0;
            OpponentComboBox.IsEnabled = false;

            if (configuration.GameMode == RockGameMode.None)
            {
                configuration.GameMode = RockGameMode.NormalPractice;
            }

            GameModeComboBox.SelectedIndex = (int)configuration.GameMode - 1;
        }

        private void TraceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (TraceComboBox.SelectedIndex)
            {
                default:
                case 0:
                    TraceTextBox.Text = "N/A";
                    TraceTextBox.IsEnabled = false;
                    break;
                case 1:
                    TraceTextBox.Text = RockConstants.DefaultEndpoint + RockConstants.DefaultTracePath;
                    TraceTextBox.IsEnabled = false;
                    break;
                case 2:
                    TraceTextBox.IsEnabled = true;
                    TraceTextBox.Text = configuration.TraceEndpoint;
                    break;
            }
        }

        private void BotComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (BotComboBox.SelectedIndex)
            {
                default:
                case 0:
                    BotTextBox.Text = "N/A";
                    BotTextBox.IsEnabled = false;
                    break;
                case 1:
                    BotTextBox.Text = RockConstants.DefaultEndpoint + RockConstants.DefaultBotPath;
                    BotTextBox.IsEnabled = false;
                    break;
                case 2:
                    BotTextBox.Text = configuration.BotEndpoint;
                    BotTextBox.IsEnabled = true;
                    break;
            }
        }

        private void GameModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            configuration.GameMode = (RockGameMode)(GameModeComboBox.SelectedIndex + 1);
        }

        private void DeckComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            configuration.DeckIndex = DeckComboBox.SelectedIndex;
        }

        private void OpponentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            configuration.OpponentIndex = OpponentComboBox.SelectedIndex;
        }
    }
}
