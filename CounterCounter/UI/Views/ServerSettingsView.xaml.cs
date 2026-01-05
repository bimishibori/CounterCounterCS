// CounterCounter/UI/Views/ServerSettingsView.xaml.cs
using System.Windows;
using WpfMessageBox = System.Windows.MessageBox;
using WpfUserControl = System.Windows.Controls.UserControl;

namespace CounterCounter.UI.Views
{
    public partial class ServerSettingsView : WpfUserControl
    {
        private int _httpPort;
        private bool _isServerRunning;

        public event EventHandler<int>? ServerStartRequested;
        public event EventHandler? ServerStopRequested;
        public event EventHandler<int>? SaveSettingsRequested;

        public ServerSettingsView(int httpPort, bool isServerRunning)
        {
            InitializeComponent();
            _httpPort = httpPort;
            _isServerRunning = isServerRunning;
            UpdateUI();
        }

        private void UpdateUI()
        {
            PortTextBox.Text = _httpPort.ToString();

            if (_isServerRunning)
            {
                ServerStatusText.Text = "起動中";
                ServerStatusText.Foreground = System.Windows.Media.Brushes.LimeGreen;
                HttpPortText.Text = _httpPort.ToString();
                WsPortText.Text = (_httpPort + 1).ToString();
            }
            else
            {
                ServerStatusText.Text = "停止中";
                ServerStatusText.Foreground = System.Windows.Media.Brushes.Red;
                HttpPortText.Text = "未起動";
                WsPortText.Text = "未起動";
            }
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(PortTextBox.Text, out int port) || port < 1024 || port > 65535)
            {
                WpfMessageBox.Show(
                    "ポート番号は1024～65535の範囲で指定してください。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            ServerStartRequested?.Invoke(this, port);
        }

        private void StopServer_Click(object sender, RoutedEventArgs e)
        {
            ServerStopRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(PortTextBox.Text, out int port) || port < 1024 || port > 65535)
            {
                WpfMessageBox.Show(
                    "ポート番号は1024～65535の範囲で指定してください。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            SaveSettingsRequested?.Invoke(this, port);
        }
    }
}