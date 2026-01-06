// CounterCounter/UI/Views/ServerSettingsView.xaml.cs
using System.Windows;
using CounterCounter.Models;
using WpfUserControl = System.Windows.Controls.UserControl;

namespace CounterCounter.UI.Views
{
    public partial class ServerSettingsView : WpfUserControl
    {
        private readonly CounterSettings _settings;
        private bool _isServerRunning;

        public event EventHandler? SettingsChanged;

        public ServerSettingsView(CounterSettings settings, bool isServerRunning)
        {
            InitializeComponent();
            _settings = settings;
            _isServerRunning = isServerRunning;
            UpdateUI();
        }

        private void UpdateUI()
        {
            PortTextBox.Text = _settings.ServerPort.ToString();
            PortTextBox.IsEnabled = !_isServerRunning;
            PortWarningText.Visibility = _isServerRunning ? Visibility.Visible : Visibility.Collapsed;

            SlideInTextBox.Text = _settings.SlideInIntervalMs.ToString();
            SlideInTextBox.IsEnabled = !_isServerRunning;
            SlideInWarningText.Visibility = _isServerRunning ? Visibility.Visible : Visibility.Collapsed;

            RotationTextBox.Text = _settings.RotationIntervalMs.ToString();
            RotationTextBox.IsEnabled = !_isServerRunning;
            RotationWarningText.Visibility = _isServerRunning ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PortTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(PortTextBox.Text, out int port) && port >= 1024 && port <= 65535)
            {
                _settings.ServerPort = port;
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private void SlideInTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(SlideInTextBox.Text, out int interval) && interval >= 1000)
            {
                _settings.SlideInIntervalMs = interval;
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void RotationTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(RotationTextBox.Text, out int interval) && interval >= 1000)
            {
                _settings.RotationIntervalMs = interval;
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void UpdateServerStatus(bool isRunning)
        {
            _isServerRunning = isRunning;
            UpdateUI();
        }

        public int GetHttpPort()
        {
            return int.TryParse(PortTextBox.Text, out int port) ? port : 9000;
        }

        public int GetSlideInInterval()
        {
            return int.TryParse(SlideInTextBox.Text, out int interval) ? interval : 5000;
        }

        public int GetRotationInterval()
        {
            return int.TryParse(RotationTextBox.Text, out int interval) ? interval : 5000;
        }
    }
}