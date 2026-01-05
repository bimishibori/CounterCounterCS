// CounterCounter/UI/Views/ServerSettingsView.xaml.cs
using System.Windows;
using WpfMessageBox = System.Windows.MessageBox;
using WpfUserControl = System.Windows.Controls.UserControl;
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;
using WpfLinearGradientBrush = System.Windows.Media.LinearGradientBrush;
using WpfGradientStop = System.Windows.Media.GradientStop;
using Point = System.Windows.Point;

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
            PortTextBox.IsEnabled = !_isServerRunning;
            PortWarningText.Visibility = _isServerRunning ? Visibility.Visible : Visibility.Collapsed;

            UpdateToggleButton();
            UpdateServerInfo();
        }

        private void UpdateToggleButton()
        {
            if (_isServerRunning)
            {
                ServerToggleButton.Content = "サーバー停止";
                var stopBrush = new WpfLinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1)
                };
                stopBrush.GradientStops.Add(new WpfGradientStop(
                    (WpfColor)WpfColorConverter.ConvertFromString("#ff4757"), 0));
                stopBrush.GradientStops.Add(new WpfGradientStop(
                    (WpfColor)WpfColorConverter.ConvertFromString("#ff1744"), 1));
                ServerToggleButton.Background = stopBrush;
                ServerToggleButton.Tag = WpfColorConverter.ConvertFromString("#ff4757");
            }
            else
            {
                ServerToggleButton.Content = "サーバー起動";
                var startBrush = new WpfLinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1)
                };
                startBrush.GradientStops.Add(new WpfGradientStop(
                    (WpfColor)WpfColorConverter.ConvertFromString("#5fec5f"), 0));
                startBrush.GradientStops.Add(new WpfGradientStop(
                    (WpfColor)WpfColorConverter.ConvertFromString("#2ecc71"), 1));
                ServerToggleButton.Background = startBrush;
                ServerToggleButton.Tag = WpfColorConverter.ConvertFromString("#5fec5f");
            }
        }

        private void UpdateServerInfo()
        {
            if (_isServerRunning)
            {
                ServerStatusText.Text = "起動中";
                ServerStatusText.Foreground = new WpfSolidColorBrush(
                    (WpfColor)WpfColorConverter.ConvertFromString("#5fec5f"));
                HttpPortText.Text = _httpPort.ToString();
                WsPortText.Text = (_httpPort + 1).ToString();
            }
            else
            {
                ServerStatusText.Text = "停止中";
                ServerStatusText.Foreground = new WpfSolidColorBrush(
                    (WpfColor)WpfColorConverter.ConvertFromString("#ff4757"));
                HttpPortText.Text = "未起動";
                WsPortText.Text = "未起動";
            }
        }

        private void ServerToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_isServerRunning)
            {
                var result = WpfMessageBox.Show(
                    "サーバーを停止しますか？\nOBSからの接続が切断されます。",
                    "確認",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    ServerStopRequested?.Invoke(this, EventArgs.Empty);
                    _isServerRunning = false;
                    UpdateUI();
                }
            }
            else
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

                _httpPort = port;
                ServerStartRequested?.Invoke(this, port);
                _isServerRunning = true;
                UpdateUI();
            }
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

        public void UpdateServerStatus(bool isRunning, int httpPort)
        {
            _isServerRunning = isRunning;
            _httpPort = httpPort;
            UpdateUI();
        }
    }
}