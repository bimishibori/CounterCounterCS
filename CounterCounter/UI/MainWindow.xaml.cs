// CounterCounter/UI/MainWindow.xaml.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using CounterCounter.Core;
using CounterCounter.Models;
using CounterCounter.Server;
using CounterCounter.UI.Views;
using WpfButton = System.Windows.Controls.Button;
using WpfMessageBox = System.Windows.MessageBox;
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace CounterCounter.UI
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly CounterManager _counterManager;
        private readonly ConfigManager _configManager;
        private readonly CounterSettings _settings;
        private WebServer? _webServer;
        private WebSocketServer? _wsServer;
        private HotkeyManager? _hotkeyManager;
        private IntPtr _hwnd;
        private int _httpPort;
        private int _wsPort;
        private bool _isServerRunning;

        private CounterManagementView? _counterManagementView;
        private ServerSettingsView? _serverSettingsView;
        private ConnectionInfoView? _connectionInfoView;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow(CounterManager counterManager, ConfigManager configManager, CounterSettings settings)
        {
            InitializeComponent();
            DataContext = this;

            _counterManager = counterManager;
            _configManager = configManager;
            _settings = settings;
            _httpPort = settings.ServerPort;
            _wsPort = _httpPort + 1;
            _isServerRunning = false;

            UpdateServerStatus();
            ShowCountersView();
        }

        public void SetHwnd(IntPtr hwnd)
        {
            _hwnd = hwnd;
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

        public void StartServerFromTray()
        {
            if (_isServerRunning)
            {
                return;
            }

            try
            {
                StartServer(_httpPort);
                WpfMessageBox.Show(
                    $"サーバーを起動しました。\nHTTP: {_httpPort}\nWebSocket: {_wsPort}",
                    "起動完了",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(
                    $"サーバーの起動に失敗しました: {ex.Message}",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        public void StopServerFromTray()
        {
            if (!_isServerRunning)
            {
                return;
            }

            StopServer();
            WpfMessageBox.Show(
                "サーバーを停止しました。",
                "停止完了",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void NavCounters_Click(object sender, RoutedEventArgs e)
        {
            ShowCountersView();
            UpdateNavButtons(NavCountersButton);
        }

        private void NavServer_Click(object sender, RoutedEventArgs e)
        {
            ShowServerSettingsView();
            UpdateNavButtons(NavServerButton);
        }

        private void NavConnection_Click(object sender, RoutedEventArgs e)
        {
            ShowConnectionInfoView();
            UpdateNavButtons(NavConnectionButton);
        }

        private void UpdateNavButtons(WpfButton activeButton)
        {
            var inactiveBrush = new WpfSolidColorBrush((WpfColor)WpfColorConverter.ConvertFromString("#444444"));
            var activeBrush = new WpfSolidColorBrush((WpfColor)WpfColorConverter.ConvertFromString("#00d4ff"));

            NavCountersButton.Background = inactiveBrush;
            NavServerButton.Background = inactiveBrush;
            NavConnectionButton.Background = inactiveBrush;

            activeButton.Background = activeBrush;
        }

        private void ShowCountersView()
        {
            _counterManagementView = new CounterManagementView(
                _counterManager,
                _settings.Hotkeys,
                _configManager,
                _settings);
            ContentArea.Children.Clear();
            ContentArea.Children.Add(_counterManagementView);
        }

        private void ShowServerSettingsView()
        {
            _serverSettingsView = new ServerSettingsView(_httpPort, _isServerRunning);
            _serverSettingsView.ServerStartRequested += OnServerStartRequested;
            _serverSettingsView.ServerStopRequested += OnServerStopRequested;
            _serverSettingsView.SaveSettingsRequested += OnSaveSettingsRequested;
            ContentArea.Children.Clear();
            ContentArea.Children.Add(_serverSettingsView);
        }

        private void ShowConnectionInfoView()
        {
            _connectionInfoView = new ConnectionInfoView(_httpPort, _wsPort, _isServerRunning);
            ContentArea.Children.Clear();
            ContentArea.Children.Add(_connectionInfoView);
        }

        private void OnServerStartRequested(object? sender, int port)
        {
            if (_isServerRunning)
            {
                WpfMessageBox.Show("サーバーは既に起動しています。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                _httpPort = port;
                _wsPort = port + 1;
                StartServer(port);

                _serverSettingsView?.UpdateServerStatus(true, _httpPort);

                WpfMessageBox.Show(
                    $"サーバーを起動しました。\nHTTP: {_httpPort}\nWebSocket: {_wsPort}",
                    "起動完了",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show($"サーバーの起動に失敗しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnServerStopRequested(object? sender, EventArgs e)
        {
            if (!_isServerRunning)
            {
                WpfMessageBox.Show("サーバーは起動していません。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            StopServer();
            _serverSettingsView?.UpdateServerStatus(false, _httpPort);
            WpfMessageBox.Show("サーバーを停止しました。", "停止完了", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnSaveSettingsRequested(object? sender, int port)
        {
            _settings.ServerPort = port;
            _settings.Counters = _counterManager.GetAllCounters();
            _configManager.Save(_settings);

            WpfMessageBox.Show(
                "設定を保存しました。\nポート変更を反映するには、サーバーを再起動してください。",
                "保存完了",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void StartServer(int port)
        {
            _webServer = new WebServer(_counterManager);
            Task.Run(async () => await _webServer.StartAsync(port));

            _wsServer = new WebSocketServer(_counterManager, port);
            _wsServer.Start();

            _hotkeyManager = new HotkeyManager();
            _hotkeyManager.Initialize(_hwnd);
            RegisterHotkeys();

            _isServerRunning = true;
            _httpPort = port;
            _wsPort = port + 1;

            UpdateServerStatus();
            UpdateTrayIcon();
        }

        private void StopServer()
        {
            _hotkeyManager?.Dispose();
            _hotkeyManager = null;

            _wsServer?.Dispose();
            _wsServer = null;

            _webServer?.Dispose();
            _webServer = null;

            _isServerRunning = false;

            UpdateServerStatus();
            UpdateTrayIcon();
        }

        private void UpdateTrayIcon()
        {
            var app = System.Windows.Application.Current as App;
            app?.UpdateTrayIconServerStatus(_isServerRunning, _httpPort);
        }

        private void RegisterHotkeys()
        {
            if (_hotkeyManager == null) return;

            foreach (var hotkey in _settings.Hotkeys)
            {
                _hotkeyManager.RegisterHotkey(hotkey.CounterId, hotkey.Action, hotkey.Modifiers, hotkey.VirtualKey);
            }

            _hotkeyManager.HotkeyPressed += OnHotkeyPressed;
        }

        private void OnHotkeyPressed(object? sender, HotkeyPressedEventArgs e)
        {
            switch (e.Action)
            {
                case HotkeyAction.Increment:
                    _counterManager.Increment(e.CounterId);
                    break;
                case HotkeyAction.Decrement:
                    _counterManager.Decrement(e.CounterId);
                    break;
                case HotkeyAction.Reset:
                    _counterManager.Reset(e.CounterId);
                    break;
            }
        }

        private void UpdateServerStatus()
        {
            if (_isServerRunning)
            {
                ServerStatusText.Text = "起動中";
                ServerStatusDot.Fill = new WpfSolidColorBrush((WpfColor)WpfColorConverter.ConvertFromString("#5fec5f"));
            }
            else
            {
                ServerStatusText.Text = "停止中";
                ServerStatusDot.Fill = new WpfSolidColorBrush((WpfColor)WpfColorConverter.ConvertFromString("#ff4757"));
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Cleanup()
        {
            _settings.Counters = _counterManager.GetAllCounters();
            _settings.Hotkeys = _settings.Hotkeys;
            _configManager.Save(_settings);

            _hotkeyManager?.Dispose();
            _wsServer?.Dispose();
            _webServer?.Dispose();
        }
    }
}