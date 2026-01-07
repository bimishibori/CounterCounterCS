// CounterCounter/UI/Infrastructure/TrayIcon.cs
using CounterCounter.Models;
using CounterCounter.UI.Dialogs;
using WinForms = System.Windows.Forms;
using WpfClipboard = System.Windows.Clipboard;
using WpfMessageBox = CounterCounter.UI.Dialogs.ModernMessageBox;
using WpfMessageBoxButton = System.Windows.MessageBoxButton;
using WpfMessageBoxImage = System.Windows.MessageBoxImage;
using WpfMessageBoxResult = System.Windows.MessageBoxResult;

namespace CounterCounter.Core
{
    public class TrayIcon : IDisposable
    {
        private readonly WinForms.NotifyIcon _notifyIcon;
        private readonly CounterManager _counterManager;
        private readonly ConfigManager _configManager;
        private readonly CounterSettings _settings;
        private bool _isServerRunning;
        private WinForms.ToolStripMenuItem? _serverToggleMenuItem;

        public event EventHandler? ShowSettingsRequested;
        public event EventHandler? ServerStartRequested;
        public event EventHandler? ServerStopRequested;

        public TrayIcon(CounterManager counterManager, ConfigManager configManager, CounterSettings settings)
        {
            _counterManager = counterManager;
            _configManager = configManager;
            _settings = settings;
            _isServerRunning = false;

            _notifyIcon = new WinForms.NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Application,
                Visible = true,
                Text = "カウンター・カウンター\nサーバー: 停止中"
            };

            _notifyIcon.DoubleClick += (s, e) => ShowSettingsRequested?.Invoke(this, EventArgs.Empty);

            BuildContextMenu();
        }

        private void BuildContextMenu()
        {
            var contextMenu = new WinForms.ContextMenuStrip();
            contextMenu.Items.Add("設定を開く", null, (s, e) => ShowSettingsRequested?.Invoke(this, EventArgs.Empty));
            contextMenu.Items.Add(new WinForms.ToolStripSeparator());

            _serverToggleMenuItem = new WinForms.ToolStripMenuItem("サーバー起動", null, OnServerToggle);
            contextMenu.Items.Add(_serverToggleMenuItem);

            contextMenu.Items.Add(new WinForms.ToolStripSeparator());
            contextMenu.Items.Add("OBS URLをコピー", null, CopyObsUrl);
            contextMenu.Items.Add(new WinForms.ToolStripSeparator());
            contextMenu.Items.Add("終了", null, Exit);

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void OnServerToggle(object? sender, EventArgs e)
        {
            if (_isServerRunning)
            {
                var result = WpfMessageBox.Show(
                    "サーバーを停止しますか?\nOBSからの接続が切断されます。",
                    "確認",
                    WpfMessageBoxButton.YesNo,
                    WpfMessageBoxImage.Question
                );

                if (result == WpfMessageBoxResult.Yes)
                {
                    ServerStopRequested?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                ServerStartRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public void UpdateServerStatus(bool isRunning, int httpPort)
        {
            _isServerRunning = isRunning;

            if (_serverToggleMenuItem != null)
            {
                _serverToggleMenuItem.Text = _isServerRunning ? "サーバー停止" : "サーバー起動";
            }

            if (_isServerRunning)
            {
                _notifyIcon.Text = $"カウンター・カウンター\nサーバー: 起動中 (Port: {httpPort})";
            }
            else
            {
                _notifyIcon.Text = "カウンター・カウンター\nサーバー: 停止中";
            }
        }

        public void ShowNotification(string title, string message, int timeoutMs = 3000)
        {
            _notifyIcon.ShowBalloonTip(timeoutMs, title, message, WinForms.ToolTipIcon.Info);
        }

        private void CopyObsUrl(object? sender, EventArgs e)
        {
            if (!_isServerRunning)
            {
                WpfMessageBox.Show(
                    "サーバーが起動していません。\n先にサーバーを起動してください。",
                    "エラー",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Warning
                );
                return;
            }

            int port = _settings.ServerPort;
            WpfClipboard.SetText($"http://localhost:{port}/obs.html");

            WpfMessageBox.Show(
                "URLをクリップボードにコピーしました",
                "コピー完了",
                WpfMessageBoxButton.OK,
                WpfMessageBoxImage.Information
            );
        }

        private void Exit(object? sender, EventArgs e)
        {
            global::System.Windows.Application.Current.Shutdown();
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
        }
    }
}