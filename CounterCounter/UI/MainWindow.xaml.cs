// CounterCounter/UI/MainWindow.xaml.cs
using System.ComponentModel;
using System.Windows;
using CounterCounter.Core;
using CounterCounter.Models;
using CounterCounter.UI.Views;

namespace CounterCounter.UI
{
    public partial class MainWindow : Window
    {
        private readonly CounterManager _counterManager;
        private readonly int _httpPort;
        private readonly int _wsPort;
        private readonly CounterManagementView _counterManagementView;
        private readonly HotkeySettingsView _hotkeySettingsView;
        private readonly ConnectionInfoView _connectionInfoView;

        public MainWindow(CounterManager counterManager, int httpPort, int wsPort)
        {
            InitializeComponent();
            _counterManager = counterManager;
            _httpPort = httpPort;
            _wsPort = wsPort;

            var app = System.Windows.Application.Current as App;
            var hotkeySettings = app?.Settings?.Hotkeys ?? new List<HotkeySettings>();

            _counterManagementView = new CounterManagementView(_counterManager);
            _hotkeySettingsView = new HotkeySettingsView(_counterManager, hotkeySettings);
            _connectionInfoView = new ConnectionInfoView(_httpPort, _wsPort);

            CounterManagementContent.Content = _counterManagementView;
            HotkeySettingsContent.Content = _hotkeySettingsView;
            ConnectionInfoContent.Content = _connectionInfoView;

            _counterManager.CounterChanged += OnCounterChanged;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }

        private void OnCounterChanged(object? sender, CounterChangeEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _counterManagementView.RefreshCounterList();
                _hotkeySettingsView.RefreshHotkeyList();
            });
        }

        public void ShowWindow()
        {
            Show();
            Activate();
            WindowState = WindowState.Normal;
        }
    }
}