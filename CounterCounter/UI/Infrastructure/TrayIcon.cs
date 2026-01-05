// CounterCounter/UI/Infrastructure/TrayIcon.cs
using CounterCounter.Core;
using CounterCounter.Models;
using WinForms = System.Windows.Forms;
using WpfClipboard = System.Windows.Clipboard;

namespace CounterCounter.UI.Infrastructure
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
                var result = WinForms.MessageBox.Show(
                    "サーバーを停止しますか？\nOBSからの接続が切断されます。",
                    "確認",
                    WinForms.MessageBoxButtons.YesNo,
                    WinForms.MessageBoxIcon.Question
                );

                if (result == WinForms.DialogResult.Yes)
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

        private void CopyObsUrl(object? sender, EventArgs e)
        {
            if (!_isServerRunning)
            {
                WinForms.MessageBox.Show(
                    "サーバーが起動していません。\n先にサーバーを起動してください。",
                    "エラー",
                    WinForms.MessageBoxButtons.OK,
                    WinForms.MessageBoxIcon.Warning
                );
                return;
            }

            int port = _settings.ServerPort;
            WpfClipboard.SetText($"http://localhost:{port}/obs.html");

            WinForms.MessageBox.Show(
                "URLをクリップボードにコピーしました",
                "コピー完了",
                WinForms.MessageBoxButtons.OK,
                WinForms.MessageBoxIcon.Information
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