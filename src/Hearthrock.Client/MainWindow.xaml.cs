// <copyright file = "MainWindow.xaml.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Client
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using Hearthrock.Bot.Exceptions;
    using Hearthrock.Client.Hacking;
    using Hearthrock.Contracts;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The RockConfiguration.
        /// </summary>
        private RockConfiguration configuration = new RockConfiguration();

        /// <summary>
        /// The Patcher instance.
        /// </summary>
        private Patcher patcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.patcher = new Patcher(Environment.CurrentDirectory);
            this.InitializeConfigurationAsync();
        }

        /// <summary>
        /// The method to response Click event for PatchButton.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private async void PatchButton_Click(object sender, RoutedEventArgs e)
        {
            PatchButton.IsEnabled = false;

            try
            {
                await this.patcher.RecoverHearthstoneAsync();
            }
            catch (PegasusException pegasusException)
            {
                MessageBox.Show(pegasusException.Message);
                PatchButton.IsEnabled = true;
                return;
            }

            await this.patcher.InjectAsync();
            PatchButton.IsEnabled = true;

            if (!await this.patcher.TestRockConfigurationAsync())
            {
                await this.patcher.WriteRockConfigurationAsync(this.configuration);
            }
        }

        /// <summary>
        /// The method to response Click event for RecoverButton.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private async void RecoverButton_Click(object sender, RoutedEventArgs e)
        {
            RecoverButton.IsEnabled = false;

            try
            {
                await this.patcher.RecoverHearthstoneAsync();
            }
            catch (PegasusException pegasusException)
            {
                MessageBox.Show(pegasusException.Message);
                RecoverButton.IsEnabled = true;
                return;
            }

            RecoverButton.IsEnabled = true;
        }

        /// <summary>
        /// The method to response Click event for SaveButton.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = false;

            switch (TraceComboBox.SelectedIndex)
            {
                default:
                case 0:
                    this.configuration.TraceEndpoint = string.Empty;
                    break;
                case 1:
                    this.configuration.TraceEndpoint = RockConstants.DefaultEndpoint + RockConstants.DefaultTracePath;
                    break;
                case 2:
                    this.configuration.TraceEndpoint = TraceTextBox.Text;
                    break;
            }

            switch (this.BotComboBox.SelectedIndex)
            {
                default:
                case 0:
                    this.configuration.BotEndpoint = string.Empty;
                    break;
                case 1:
                    this.configuration.BotEndpoint = RockConstants.DefaultEndpoint + RockConstants.DefaultBotPath;
                    break;
                case 2:
                    this.configuration.BotEndpoint = BotTextBox.Text;
                    break;
            }

            if (this.configuration.BotEndpoint != string.Empty && !this.configuration.BotEndpoint.EndsWith("/"))
            {
                MessageBox.Show("Bot endpoints usually end with \"/\".");
            }

            try
            {
                await this.patcher.WriteRockConfigurationAsync(this.configuration);
            }
            catch (PegasusException pegasusException)
            {
                MessageBox.Show(pegasusException.Message);
                SaveButton.IsEnabled = true;
                return;
            }

            await this.InitializeConfigurationAsync(this.patcher.RootPath);

            SaveButton.IsEnabled = true;
        }

        /// <summary>
        /// The method to response SelectionChanged event for TraceComboBox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
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
                    TraceTextBox.Text = this.configuration.TraceEndpoint;
                    break;
            }
        }
        
        /// <summary>
        /// The method to response SelectionChanged event for BotComboBox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
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
                    BotTextBox.Text = this.configuration.BotEndpoint;
                    BotTextBox.IsEnabled = true;
                    break;
            }
        }

        /// <summary>
        /// The method to response SelectionChanged event for GameModeComboBox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void GameModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.configuration.GameMode = (RockGameMode)(GameModeComboBox.SelectedIndex + 1);
        }
        
        /// <summary>
        /// The method to response SelectionChanged event for DeckComboBox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void DeckComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.configuration.DeckIndex = DeckComboBox.SelectedIndex;
        }

        /// <summary>
        /// The method to response SelectionChanged event for OpponentComboBox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OpponentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.configuration.OpponentIndex = OpponentComboBox.SelectedIndex;
        }

        /// <summary>
        /// The method to response SelectionChanged event for LoggingComboBox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void LoggingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.configuration.TraceLevel = LoggingComboBox.SelectedIndex;
        }

        /// <summary>
        /// The method to response Click event for PathButton.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private async void PathButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    try
                    {
                        this.patcher.RootPath = dialog.SelectedPath;
                        await this.InitializeConfigurationAsync(this.patcher.RootPath);
                    }
                    catch (PegasusException pegasusException)
                    {
                        MessageBox.Show(pegasusException.Message);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Initialize Configuration and UI.
        /// </summary>
        private async void InitializeConfigurationAsync()
        {
            string path = string.Empty;
            try
            {
                path = await this.patcher.SearchHearthstoneDirectoryAsync();
                this.patcher.RootPath = path;
            }
            catch
            {
            }

            await this.InitializeConfigurationAsync(path);
        }

        /// <summary>
        /// Initialize Configuration and UI.
        /// </summary>
        /// <param name="path">the rootPath</param>
        /// <returns>The task</returns>
        private async Task InitializeConfigurationAsync(string path)
        {
            await Task.Delay(0);

            PathTextBox.Text = path;

            if (string.IsNullOrWhiteSpace(this.configuration.TraceEndpoint))
            {
                TraceComboBox.SelectedIndex = 0;
            }

            try
            {
                var defaultTraceEndpoint = new Uri(RockConstants.DefaultEndpoint + RockConstants.DefaultTracePath);
                var traceEndpoint = new Uri(this.configuration.TraceEndpoint);
                if (traceEndpoint.AbsoluteUri == defaultTraceEndpoint.AbsoluteUri)
                {
                    TraceComboBox.SelectedIndex = 1;
                }
                else
                {
                    TraceComboBox.SelectedIndex = 2;
                }
            }
            catch
            {
            }

            if (string.IsNullOrWhiteSpace(this.configuration.BotEndpoint))
            {
                BotComboBox.SelectedIndex = 0;
            }

            try
            {
                var defaultBotEndpoint = new Uri(RockConstants.DefaultEndpoint + RockConstants.DefaultBotPath);
                var botEndpoint = new Uri(this.configuration.BotEndpoint);
                if (botEndpoint.AbsoluteUri == defaultBotEndpoint.AbsoluteUri)
                {
                    BotComboBox.SelectedIndex = 1;
                }
                else
                {
                    BotComboBox.SelectedIndex = 2;
                }
            }
            catch
            {
            }

            DeckComboBox.SelectedIndex = 0;
            DeckComboBox.IsEnabled = false;

            OpponentComboBox.SelectedIndex = 0;
            OpponentComboBox.IsEnabled = false;

            if (this.configuration.GameMode == RockGameMode.None)
            {
                this.configuration.GameMode = RockGameMode.NormalPractice;
            }

            GameModeComboBox.SelectedIndex = (int)this.configuration.GameMode - 1;

            if (this.configuration.TraceLevel < 0 && this.configuration.TraceLevel > 4)
            {
                this.configuration.TraceLevel = 0;
            }

            LoggingComboBox.SelectedIndex = this.configuration.TraceLevel;
        }
    }
}
