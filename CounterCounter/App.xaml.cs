using System.Windows;
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

            // メインウィンドウを表示しない
            MainWindow = null;

            // カウンター状態管理を初期化
            _counterState = new CounterState();

            // Webサーバーを起動
            _webServer = new WebServer(_counterState);
            await _webServer.StartAsync();

            // WebSocketサーバーを起動
            _wsServer = new WebSocketServer(_counterState, _webServer.Port);
            _wsServer.Start();

            // タスクトレイアイコンを表示
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