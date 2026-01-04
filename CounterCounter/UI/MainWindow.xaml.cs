// CounterCounter/UI/MainWindow.xaml.cs
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using CounterCounter.Core;
using CounterCounter.Models;
using WpfClipboard = System.Windows.Clipboard;
using WpfMessageBox = System.Windows.MessageBox;

namespace CounterCounter.UI
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly CounterManager _counterManager;
        private int _httpPort;
        private int _wsPort;
        private bool _isServerRunning;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsServerStopped => !_isServerRunning;

        public MainWindow(CounterManager counterManager, int httpPort, int wsPort)
        {
            InitializeComponent();
            DataContext = this;

            _counterManager = counterManager;
            _httpPort = httpPort;
            _wsPort = wsPort;
            _isServerRunning = true;

            _counterManager.CounterChanged += OnCounterChanged;

            PortTextBox.Text = _httpPort.ToString();
            UpdateConnectionInfo();
            UpdateCounterList();
            UpdateServerStatus();
        }

        public void ShowWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void UpdateCounterList()
        {
            var counters = _counterManager.GetAllCounters();
            CountersListBox.ItemsSource = counters.Select(c => new CounterViewModel(c, _counterManager)).ToList();
        }

        private void UpdateConnectionInfo()
        {
            ManagementUrlText.Text = $"http://localhost:{_httpPort}/";
            ObsUrlText.Text = $"http://localhost:{_httpPort}/obs.html";
        }

        private void UpdateServerStatus()
        {
            if (_isServerRunning)
            {
                ServerStatusText.Text = "起動中";
                ServerStatusText.Foreground = System.Windows.Media.Brushes.LimeGreen;
                StartServerButton.IsEnabled = false;
                StopServerButton.IsEnabled = true;
            }
            else
            {
                ServerStatusText.Text = "停止中";
                ServerStatusText.Foreground = System.Windows.Media.Brushes.Red;
                StartServerButton.IsEnabled = true;
                StopServerButton.IsEnabled = false;
            }
            OnPropertyChanged(nameof(IsServerStopped));
        }

        private void OnCounterChanged(object? sender, CounterChangedEventArgs e)
        {
            Dispatcher.Invoke(UpdateCounterList);
        }

        private void AddCounter_Click(object sender, RoutedEventArgs e)
        {
            if (_isServerRunning)
            {
                WpfMessageBox.Show(
                    "サーバー起動中はカウンターを追加できません。\nサーバーを停止してください。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            var dialog = new CounterEditDialog(null);
            if (dialog.ShowDialog() == true && dialog.Counter != null)
            {
                _counterManager.AddCounter(dialog.Counter);
                UpdateCounterList();
            }
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(PortTextBox.Text, out int newPort) || newPort < 1024 || newPort > 65535)
            {
                WpfMessageBox.Show(
                    "ポート番号は1024～65535の範囲で指定してください。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            WpfMessageBox.Show(
                "サーバーの起動機能は次回実装予定です。\n現在はアプリ再起動でポート変更が反映されます。",
                "未実装",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void StopServer_Click(object sender, RoutedEventArgs e)
        {
            var result = WpfMessageBox.Show(
                "サーバーを停止しますか？\nOBSからの接続が切断されます。",
                "確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                WpfMessageBox.Show(
                    "サーバーの停止機能は次回実装予定です。",
                    "未実装",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_isServerRunning)
            {
                WpfMessageBox.Show(
                    "サーバー起動中は設定を保存できません。\nサーバーを停止してください。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (!int.TryParse(PortTextBox.Text, out int newPort) || newPort < 1024 || newPort > 65535)
            {
                WpfMessageBox.Show(
                    "ポート番号は1024～65535の範囲で指定してください。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            var app = (System.Windows.Application)System.Windows.Application.Current;
            var counterApp = app as CounterCounter.App;
            if (counterApp?.Settings != null)
            {
                counterApp.Settings.ServerPort = newPort;
                counterApp.Settings.Counters = _counterManager.GetAllCounters();

                var configManager = new ConfigManager();
                configManager.Save(counterApp.Settings);

                WpfMessageBox.Show(
                    "設定を保存しました。\nポート変更を反映するにはアプリを再起動してください。",
                    "保存完了",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        private void OpenManagementPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"http://localhost:{_httpPort}/",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show($"ブラウザを開けませんでした: {ex.Message}", "エラー");
            }
        }

        private void CopyObsUrl_Click(object sender, RoutedEventArgs e)
        {
            WpfClipboard.SetText($"http://localhost:{_httpPort}/obs.html");
            WpfMessageBox.Show("URLをクリップボードにコピーしました", "コピー完了");
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}