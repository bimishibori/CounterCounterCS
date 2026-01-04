// CounterCounter/App.xaml.cs
using System.Windows;
using System.Windows.Interop;
using CounterCounter.Core;
using CounterCounter.Models;
using CounterCounter.Server;
using CounterCounter.UI;
using WpfApp = System.Windows.Application;
using WpfMessageBox = System.Windows.MessageBox;

namespace CounterCounter
{
    public partial class App : WpfApp
    {
        private TrayIcon? _trayIcon;
        private CounterManager? _counterManager;
        private WebServer? _webServer;
        private WebSocketServer? _wsServer;
        private HotkeyManager? _hotkeyManager;
        private ConfigManager? _configManager;
        private System.Windows.Window? _hiddenWindow;
        private CounterSettings? _settings;
        private MainWindow? _mainWindow;

        public CounterSettings? Settings => _settings;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _configManager = new ConfigManager();
            _settings = _configManager.Load();

            _counterManager = new CounterManager();
            _counterManager.LoadCounters(_settings.Counters);

            _webServer = new WebServer(_counterManager);
            await _webServer.StartAsync(_settings.ServerPort);

            _wsServer = new WebSocketServer(_counterManager, _webServer.Port);
            _wsServer.Start();

            _trayIcon = new TrayIcon(_counterManager, _webServer.Port, _wsServer.Port, _configManager, _settings);
            _trayIcon.ShowSettingsRequested += OnShowSettingsRequested;

            _mainWindow = new MainWindow(_counterManager, _webServer.Port, _wsServer.Port);
            _mainWindow.Show();

            InitializeHotkeys();
        }

        private void OnShowSettingsRequested(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (_mainWindow != null)
                {
                    _mainWindow.ShowWindow();
                }
            });
        }

        private void InitializeHotkeys()
        {
            _hiddenWindow = new System.Windows.Window
            {
                Width = 0,
                Height = 0,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                ShowActivated = false,
                Visibility = Visibility.Hidden
            };
            _hiddenWindow.Show();

            var helper = new WindowInteropHelper(_hiddenWindow);
            IntPtr hwnd = helper.Handle;

            _hotkeyManager = new HotkeyManager();
            _hotkeyManager.Initialize(hwnd);
            _hotkeyManager.HotkeyPressed += OnHotkeyPressed;

            if (_settings == null) return;

            int successCount = 0;
            int failCount = 0;

            foreach (var hotkey in _settings.Hotkeys)
            {
                bool success = _hotkeyManager.RegisterHotkey(
                    hotkey.CounterId,
                    hotkey.Action,
                    hotkey.Modifiers,
                    hotkey.VirtualKey
                );

                if (success)
                    successCount++;
                else
                    failCount++;
            }

            if (failCount > 0)
            {
                WpfMessageBox.Show(
                    $"ホットキーの登録に一部失敗しました。\n成功: {successCount}個 / 失敗: {failCount}個\n" +
                    "他のアプリケーションがキーを使用している可能性があります。",
                    "ホットキー登録エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        private void OnHotkeyPressed(object? sender, HotkeyPressedEventArgs e)
        {
            if (_counterManager == null) return;

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

        protected override void OnExit(ExitEventArgs e)
        {
            if (_settings != null && _counterManager != null && _configManager != null)
            {
                _settings.Counters = _counterManager.GetAllCounters();
                _configManager.Save(_settings);
            }

            _hotkeyManager?.Dispose();
            _hiddenWindow?.Close();
            _wsServer?.Dispose();
            _webServer?.Dispose();
            _trayIcon?.Dispose();
            base.OnExit(e);
        }
    }
}