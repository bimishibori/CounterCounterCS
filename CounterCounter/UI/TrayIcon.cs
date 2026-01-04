// CounterCounter/UI/TrayIcon.cs
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CounterCounter.Core;
using CounterCounter.Models;
using WpfApp = System.Windows.Application;

namespace CounterCounter.UI
{
    public class TrayIcon : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly CounterManager _counterManager;
        private readonly int _httpPort;
        private readonly int _wsPort;
        private readonly ConfigManager _configManager;
        private readonly CounterSettings _settings;

        public event EventHandler? ShowSettingsRequested;

        public TrayIcon(CounterManager counterManager, int httpPort, int wsPort,
            ConfigManager configManager, CounterSettings settings)
        {
            _counterManager = counterManager;
            _httpPort = httpPort;
            _wsPort = wsPort;
            _configManager = configManager;
            _settings = settings;

            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = $"Counter Counter\nHTTP:{_httpPort} WS:{_wsPort}"
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("設定を開く", null, OnShowSettings);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("OBS URLをコピー", null, OnCopyObsUrl);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("設定を保存", null, OnSaveSettings);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("終了", null, OnExit);

            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (s, e) => OnShowSettings(s, e);
        }

        private void OnShowSettings(object? sender, EventArgs e)
        {
            ShowSettingsRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnCopyObsUrl(object? sender, EventArgs e)
        {
            try
            {
                string url = $"http://localhost:{_httpPort}/obs.html";
                Clipboard.SetText(url);
                MessageBox.Show(
                    "URLをクリップボードにコピーしました",
                    "コピー完了",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"クリップボードへのコピーに失敗しました: {ex.Message}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void OnSaveSettings(object? sender, EventArgs e)
        {
            try
            {
                _settings.Counters = _counterManager.GetAllCounters();
                _configManager.Save(_settings);

                MessageBox.Show(
                    "設定を保存しました",
                    "保存完了",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"設定の保存に失敗しました: {ex.Message}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void OnExit(object? sender, EventArgs e)
        {
            _settings.Counters = _counterManager.GetAllCounters();
            _configManager.Save(_settings);

            WpfApp.Current.Shutdown();
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
        }
    }
}