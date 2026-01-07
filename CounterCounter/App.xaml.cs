// CounterCounter/App.xaml.cs
using System.Windows;
using System.Windows.Interop;
using CounterCounter.Core;
using CounterCounter.Models;
using CounterCounter.UI;
using WpfApp = System.Windows.Application;

namespace CounterCounter
{
    public partial class App : WpfApp
    {
        private TrayIcon? _trayIcon;
        private CounterManager? _counterManager;
        private ConfigManager? _configManager;
        private Window? _hiddenWindow;
        private CounterSettings? _settings;
        private MainWindow? _mainWindow;

        public CounterSettings? Settings => _settings;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _configManager = new ConfigManager();
            _settings = _configManager.Load();

            _counterManager = new CounterManager();
            _counterManager.LoadCounters(_settings.Counters);

            _hiddenWindow = new Window
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

            _mainWindow = new MainWindow(_counterManager, _configManager, _settings);
            _mainWindow.SetHwnd(hwnd);

            _mainWindow.Show();
            _mainWindow.Activate();

            _trayIcon = new TrayIcon(_counterManager, _configManager, _settings);
            _trayIcon.ShowSettingsRequested += OnShowSettingsRequested;
            _trayIcon.ServerStartRequested += OnServerStartRequested;
            _trayIcon.ServerStopRequested += OnServerStopRequested;
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

        private void OnServerStartRequested(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _mainWindow?.StartServerFromTray();
            });
        }

        private void OnServerStopRequested(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _mainWindow?.StopServerFromTray();
            });
        }

        public void UpdateTrayIconServerStatus(bool isRunning, int httpPort)
        {
            _trayIcon?.UpdateServerStatus(isRunning, httpPort);
        }

        public void ShowTrayNotification(string title, string message, int timeoutMs = 3000)
        {
            _trayIcon?.ShowNotification(title, message, timeoutMs);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_settings != null && _counterManager != null && _configManager != null)
            {
                _settings.Counters = _counterManager.GetAllCounters();
                bool saved = _configManager.Save(_settings);
                Console.WriteLine($"[App] OnExit: Settings saved = {saved}, RotationHotkey = {_settings.NextRotationHotkey?.GetDisplayText() ?? "null"}");
            }

            _mainWindow?.Cleanup();
            _hiddenWindow?.Close();
            _trayIcon?.Dispose();
            base.OnExit(e);
        }
    }
}