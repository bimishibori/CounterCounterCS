// CounterCounter/UI/TrayIcon.cs
using System.Diagnostics;
using CounterCounter.Core;
using CounterCounter.Models;
using WinForms = System.Windows.Forms;
using WpfClipboard = System.Windows.Clipboard;

namespace CounterCounter.UI
{
    public class TrayIcon : IDisposable
    {
        private readonly WinForms.NotifyIcon _notifyIcon;
        private readonly CounterManager _counterManager;
        private readonly int _httpPort;
        private readonly int _wsPort;
        private readonly ConfigManager _configManager;
        private readonly CounterSettings _settings;

        public event EventHandler? ShowSettingsRequested;

        public TrayIcon(CounterManager counterManager, int httpPort, int wsPort, ConfigManager configManager, CounterSettings settings)
        {
            _counterManager = counterManager;
            _httpPort = httpPort;
            _wsPort = wsPort;
            _configManager = configManager;
            _settings = settings;

            _notifyIcon = new WinForms.NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Application,
                Visible = true,
                Text = $"カウンター・カウンター\nHTTP:{_httpPort} WS:{_wsPort}"
            };

            _notifyIcon.DoubleClick += (s, e) => ShowSettingsRequested?.Invoke(this, EventArgs.Empty);

            var contextMenu = new WinForms.ContextMenuStrip();
            contextMenu.Items.Add("設定を開く", null, (s, e) => ShowSettingsRequested?.Invoke(this, EventArgs.Empty));
            contextMenu.Items.Add("管理ページを開く", null, OpenManagementPage);
            contextMenu.Items.Add("OBS URLをコピー", null, CopyObsUrl);
            contextMenu.Items.Add(new WinForms.ToolStripSeparator());
            contextMenu.Items.Add("設定を保存", null, SaveSettings);
            contextMenu.Items.Add(new WinForms.ToolStripSeparator());
            contextMenu.Items.Add("終了", null, Exit);

            _notifyIcon.ContextMenuStrip = contextMenu;

            _counterManager.CounterChanged += OnCounterChanged;
        }

        private void OnCounterChanged(object? sender, CounterChangedEventArgs e)
        {
            _notifyIcon.ShowBalloonTip(
                1000,
                $"{e.Counter.Name}",
                $"値が変更されました: {e.Counter.Value}",
                WinForms.ToolTipIcon.Info
            );
        }

        private void OpenManagementPage(object? sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"http://localhost:{_httpPort}/",
                UseShellExecute = true
            });
        }

        private void CopyObsUrl(object? sender, EventArgs e)
        {
            WpfClipboard.SetText($"http://localhost:{_httpPort}/obs.html");
            _notifyIcon.ShowBalloonTip(1000, "コピー完了", "OBS用URLをクリップボードにコピーしました", WinForms.ToolTipIcon.Info);
        }

        private void SaveSettings(object? sender, EventArgs e)
        {
            _settings.Counters = _counterManager.GetAllCounters();
            _configManager.Save(_settings);
            _notifyIcon.ShowBalloonTip(1000, "保存完了", "設定を保存しました", WinForms.ToolTipIcon.Info);
        }

        private void Exit(object? sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void Dispose()
        {
            _counterManager.CounterChanged -= OnCounterChanged;
            _notifyIcon?.Dispose();
        }
    }
}