// CounterCounter/App.xaml.cs
using System.Windows;
using CounterCounter.Core;
using CounterCounter.Server;
using CounterCounter.UI;
using WpfApp = System.Windows.Application;

namespace CounterCounter
{
    public partial class App : WpfApp
    {
        private TrayIcon? _trayIcon;
        private CounterState? _counterState;
        private WebServer? _webServer;
        private WebSocketServer? _wsServer;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = null;

            _counterState = new CounterState();

            _webServer = new WebServer(_counterState);
            await _webServer.StartAsync();

            _wsServer = new WebSocketServer(_counterState, _webServer.Port);
            _wsServer.Start();

            _trayIcon = new TrayIcon(_counterState, _webServer.Port, _wsServer.Port);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _wsServer?.Dispose();
            _webServer?.Dispose();
            _trayIcon?.Dispose();
            base.OnExit(e);
        }
    }
}