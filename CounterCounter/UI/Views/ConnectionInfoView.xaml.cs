// CounterCounter/UI/Views/ConnectionInfoView.xaml.cs
using System.Diagnostics;
using System.Windows;
using WpfClipboard = System.Windows.Clipboard;
using WpfMessageBox = System.Windows.MessageBox;
using WpfUserControl = System.Windows.Controls.UserControl;

namespace CounterCounter.UI.Views
{
    public partial class ConnectionInfoView : WpfUserControl
    {
        private readonly int _httpPort;
        private readonly int _wsPort;

        public ConnectionInfoView(int httpPort, int wsPort)
        {
            InitializeComponent();
            _httpPort = httpPort;
            _wsPort = wsPort;
            UpdateConnectionInfo();
        }

        private void UpdateConnectionInfo()
        {
            ObsUrlText.Text = $"http://localhost:{_httpPort}/obs.html";
            HttpPortText.Text = _httpPort.ToString();
            WsPortText.Text = _wsPort.ToString();
        }

        private void OpenObsUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"http://localhost:{_httpPort}/obs.html",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(
                    $"ブラウザを開けませんでした: {ex.Message}",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void CopyObsUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WpfClipboard.SetText($"http://localhost:{_httpPort}/obs.html");
                WpfMessageBox.Show(
                    "URLをクリップボードにコピーしました",
                    "コピー完了",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(
                    $"クリップボードへのコピーに失敗しました: {ex.Message}",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}