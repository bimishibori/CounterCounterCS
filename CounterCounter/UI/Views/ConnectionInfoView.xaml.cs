// CounterCounter/UI/Views/ConnectionInfoView.xaml.cs
using System.Diagnostics;
using System.Windows;
using CounterCounter.UI.Dialogs;
using WpfClipboard = System.Windows.Clipboard;
using WpfMessageBox = CounterCounter.UI.Dialogs.ModernMessageBox;
using WpfMessageBoxButton = System.Windows.MessageBoxButton;
using WpfMessageBoxImage = System.Windows.MessageBoxImage;
using WpfUserControl = System.Windows.Controls.UserControl;

namespace CounterCounter.UI.Views
{
    public partial class ConnectionInfoView : WpfUserControl
    {
        private readonly int _httpPort;
        private readonly bool _isServerRunning;

        public ConnectionInfoView(int httpPort, bool isServerRunning)
        {
            InitializeComponent();
            _httpPort = httpPort;
            _isServerRunning = isServerRunning;
            UpdateConnectionInfo();
        }

        private void UpdateConnectionInfo()
        {
            if (_isServerRunning)
            {
                ObsUrlText.Text = $"http://localhost:{_httpPort}/obs.html";
                RotateUrlText.Text = $"http://localhost:{_httpPort}/rotation.html";
                HttpPortText.Text = _httpPort.ToString();
                ServerStatusText.Text = "起動中";
                ServerStatusText.Foreground = System.Windows.Media.Brushes.LimeGreen;
            }
            else
            {
                ObsUrlText.Text = "サーバーが起動していません";
                RotateUrlText.Text = "サーバーが起動していません";
                HttpPortText.Text = "未起動";
                ServerStatusText.Text = "停止中";
                ServerStatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void OpenObsUrl_Click(object sender, RoutedEventArgs e)
        {
            if (!_isServerRunning)
            {
                WpfMessageBox.Show(
                    "サーバーが起動していません。\n「サーバー設定」からサーバーを起動してください。",
                    "エラー",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Warning
                );
                return;
            }

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
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Error
                );
            }
        }

        private void CopyObsUrl_Click(object sender, RoutedEventArgs e)
        {
            if (!_isServerRunning)
            {
                WpfMessageBox.Show(
                    "サーバーが起動していません。\n「サーバー設定」からサーバーを起動してください。",
                    "エラー",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Warning
                );
                return;
            }

            try
            {
                WpfClipboard.SetText($"http://localhost:{_httpPort}/obs.html");
                WpfMessageBox.Show(
                    "URLをクリップボードにコピーしました",
                    "コピー完了",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(
                    $"クリップボードへのコピーに失敗しました: {ex.Message}",
                    "エラー",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Error
                );
            }
        }


        private void OpenRotateUrl_Click(object sender, RoutedEventArgs e)
        {
            if (!_isServerRunning)
            {
                WpfMessageBox.Show(
                    "サーバーが起動していません。\n「サーバー設定」からサーバーを起動してください。",
                    "エラー",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Warning
                );
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"http://localhost:{_httpPort}/rotation.html",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(
                    $"ブラウザを開けませんでした: {ex.Message}",
                    "エラー",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Error
                );
            }
        }

        private void CopyRotateUrl_Click(object sender, RoutedEventArgs e)
        {
            if (!_isServerRunning)
            {
                WpfMessageBox.Show(
                    "サーバーが起動していません。\n「サーバー設定」からサーバーを起動してください。",
                    "エラー",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Warning
                );
                return;
            }

            try
            {
                WpfClipboard.SetText($"http://localhost:{_httpPort}/rotation.html");
                WpfMessageBox.Show(
                    "URLをクリップボードにコピーしました",
                    "コピー完了",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(
                    $"クリップボードへのコピーに失敗しました: {ex.Message}",
                    "エラー",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Error
                );
            }
        }
    }
}