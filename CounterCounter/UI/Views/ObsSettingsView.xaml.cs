// CounterCounter/UI/Views/ObsSettingsView.xaml.cs
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CounterCounter.Models;
using CounterCounter.UI.Dialogs;
using WpfClipboard = System.Windows.Clipboard;
using WpfMessageBox = CounterCounter.UI.Dialogs.ModernMessageBox;
using WpfMessageBoxButton = System.Windows.MessageBoxButton;
using WpfMessageBoxImage = System.Windows.MessageBoxImage;
using WpfUserControl = System.Windows.Controls.UserControl;

namespace CounterCounter.UI.Views
{
    public partial class ObsSettingsView : WpfUserControl
    {
        private int _httpPort;
        private bool _isServerRunning;
        private readonly CounterSettings _settings;
        private readonly string _themesPath;

        public event EventHandler? SettingsChanged;

        public ObsSettingsView(int httpPort, bool isServerRunning, CounterSettings settings)
        {
            InitializeComponent();
            _httpPort = httpPort;
            _isServerRunning = isServerRunning;
            _settings = settings;

            string exeDir = AppContext.BaseDirectory;
            _themesPath = Path.Combine(exeDir, "wwwroot", "themes");

            LoadThemes();
            UpdateConnectionInfo();
        }

        private void LoadThemes()
        {
            ThemeComboBox.Items.Clear();

            if (!Directory.Exists(_themesPath))
            {
                Directory.CreateDirectory(_themesPath);
            }

            var themeFolders = Directory.GetDirectories(_themesPath)
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrEmpty(name))
                .OrderBy(name => name)
                .ToList();

            if (themeFolders.Count == 0)
            {
                ThemeComboBox.Items.Add(new ComboBoxItem { Content = "default", Tag = "default" });
            }
            else
            {
                foreach (var theme in themeFolders)
                {
                    ThemeComboBox.Items.Add(new ComboBoxItem { Content = theme, Tag = theme });
                }
            }

            for (int i = 0; i < ThemeComboBox.Items.Count; i++)
            {
                if (ThemeComboBox.Items[i] is ComboBoxItem item &&
                    item.Tag?.ToString() == _settings.SelectedTheme)
                {
                    ThemeComboBox.SelectedIndex = i;
                    return;
                }
            }

            if (ThemeComboBox.Items.Count > 0)
            {
                ThemeComboBox.SelectedIndex = 0;
            }
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeComboBox.SelectedItem is ComboBoxItem item)
            {
                string selectedTheme = item.Tag?.ToString() ?? "default";
                if (_settings.SelectedTheme != selectedTheme)
                {
                    _settings.SelectedTheme = selectedTheme;
                    SettingsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void UpdateServerStatus(int httpPort, bool isServerRunning)
        {
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