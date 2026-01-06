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
using WpfLinearGradientBrush = System.Windows.Media.LinearGradientBrush;
using WpfGradientStop = System.Windows.Media.GradientStop;
using Point = System.Windows.Point;

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

            UpdateServerToggleButton();
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
            if (_isServerRunning)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                SaveSettings();
                e.Cancel = false;
                System.Windows.Application.Current.Shutdown();
            }
        }

        public void StartServerFromTray()
        {
            if (_isServerRunning)
            {
                return;
            }

            try
            {
                SaveSettings();
                StartServer(_httpPort);
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
        }

        private void ServerToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_isServerRunning)
            {
                var result = WpfMessageBox.Show(
                    "サーバーを停止しますか?\nOBSからの接続が切断されます。",
                    "確認",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    StopServer();
                }
            }
            else
            {
                try
                {
                    SaveSettings();
                    StartServer(_httpPort);
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
        }

        private void NavCounters_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            ShowCountersView();
            UpdateNavButtons(NavCountersButton);
        }

        private void NavServer_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            ShowServerSettingsView();
            UpdateNavButtons(NavServerButton);
        }

        private void NavConnection_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
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
            _serverSettingsView = new ServerSettingsView(_settings, _isServerRunning);
            _serverSettingsView.SettingsChanged += OnServerSettingsChanged;
            ContentArea.Children.Clear();
            ContentArea.Children.Add(_serverSettingsView);
        }

        private void ShowConnectionInfoView()
        {
            _connectionInfoView = new ConnectionInfoView(_httpPort, _wsPort, _isServerRunning);
            ContentArea.Children.Clear();
            ContentArea.Children.Add(_connectionInfoView);
        }

        private void OnServerSettingsChanged(object? sender, EventArgs e)
        {
            if (_serverSettingsView == null) return;

            _settings.ServerPort = _serverSettingsView.GetHttpPort();
            _settings.SlideInIntervalMs = _serverSettingsView.GetSlideInInterval();
            _settings.RotationIntervalMs = _serverSettingsView.GetRotationInterval();

            if (!_isServerRunning)
            {
                _httpPort = _settings.ServerPort;
                _wsPort = _httpPort + 1;
            }

            SaveSettings();
        }

        private void StartServer(int port)
        {
            _hotkeyManager?.Dispose();
            _hotkeyManager = new HotkeyManager();
            _hotkeyManager.Initialize(_hwnd);

            RegisterHotkeys();

            _webServer = new WebServer(_counterManager, _settings.RotationIntervalMs, _settings.SlideInIntervalMs);
            Task.Run(async () => await _webServer.StartAsync(port));

            _wsServer = new WebSocketServer(_counterManager, port);
            _wsServer.Start();

            _isServerRunning = true;
            _httpPort = port;
            _wsPort = port + 1;

            UpdateServerToggleButton();
            UpdateServerStatus();
            UpdateTrayIcon();

            _serverSettingsView?.UpdateServerStatus(true);
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

            UpdateServerToggleButton();
            UpdateServerStatus();
            UpdateTrayIcon();

            _serverSettingsView?.UpdateServerStatus(false);
        }

        private void UpdateServerToggleButton()
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
                bool success = _hotkeyManager.RegisterHotkey(
                    hotkey.CounterId,
                    hotkey.Action,
                    hotkey.Modifiers,
                    hotkey.VirtualKey);

                if (!success)
                {
                    Console.WriteLine($"ホットキー登録失敗: {hotkey.GetDisplayText()}");
                }
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

        private void SaveSettings()
        {
            _settings.Counters = _counterManager.GetAllCounters();
            _configManager.Save(_settings);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Cleanup()
        {
            SaveSettings();

            _hotkeyManager?.Dispose();
            _wsServer?.Dispose();
            _webServer?.Dispose();
        }
    }
}