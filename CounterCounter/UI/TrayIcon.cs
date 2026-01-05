// CounterCounter/UI/TrayIcon.cs
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
        private readonly ConfigManager _configManager;
        private readonly CounterSettings _settings;

        public event EventHandler? ShowSettingsRequested;

        public TrayIcon(CounterManager counterManager, ConfigManager configManager, CounterSettings settings)
        {
            _counterManager = counterManager;
            _configManager = configManager;
            _settings = settings;

            _notifyIcon = new WinForms.NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Application,
                Visible = true,
                Text = "カウンター・カウンター\nサーバー: 停止中"
            };

            _notifyIcon.DoubleClick += (s, e) => ShowSettingsRequested?.Invoke(this, EventArgs.Empty);

            var contextMenu = new WinForms.ContextMenuStrip();
            contextMenu.Items.Add("設定を開く", null, (s, e) => ShowSettingsRequested?.Invoke(this, EventArgs.Empty));
            contextMenu.Items.Add("OBS URLをコピー", null, CopyObsUrl);
            contextMenu.Items.Add(new WinForms.ToolStripSeparator());
            contextMenu.Items.Add("設定を保存", null, SaveSettings);
            contextMenu.Items.Add(new WinForms.ToolStripSeparator());
            contextMenu.Items.Add("終了", null, Exit);

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void CopyObsUrl(object? sender, EventArgs e)
        {
            int port = _settings.ServerPort;
            WpfClipboard.SetText($"http://localhost:{port}/obs.html");
        }

        private void SaveSettings(object? sender, EventArgs e)
        {
            _settings.Counters = _counterManager.GetAllCounters();
            _configManager.Save(_settings);
        }

        private void Exit(object? sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
        }
    }
}