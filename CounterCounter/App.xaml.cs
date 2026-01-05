// CounterCounter/App.xaml.cs
using System.Windows;
using System.Windows.Interop;
using CounterCounter.Core;
using CounterCounter.Models;
using CounterCounter.UI;
using CounterCounter.UI.Infrastructure;
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

            _trayIcon = new TrayIcon(_counterManager, _configManager, _settings);
            _trayIcon.ShowSettingsRequested += OnShowSettingsRequested;
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

        protected override void OnExit(ExitEventArgs e)
        {
            if (_settings != null && _counterManager != null && _configManager != null)
            {
                _settings.Counters = _counterManager.GetAllCounters();
                _configManager.Save(_settings);
            }

            _mainWindow?.Cleanup();
            _hiddenWindow?.Close();
            _trayIcon?.Dispose();
            base.OnExit(e);
        }
    }
}