// CounterCounter/UI/TrayIcon.cs
using System;
using System.Diagnostics;
using CounterCounter.Core;
using CounterCounter.Models;
using WinForms = System.Windows.Forms;
using WpfClipboard = System.Windows.Clipboard;
using WpfMessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace CounterCounter.UI
{
    public class TrayIcon : IDisposable
    {
        private readonly WinForms.NotifyIcon _notifyIcon;
        private readonly CounterManager _counterManager;
        private readonly ConfigManager _configManager;
        private readonly CounterSettings _settings;
        private readonly int _httpPort;
        private readonly int _wsPort;
        private MainWindow? _mainWindow;

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
                Text = $"Counter Counter\nHTTP:{_httpPort} WS:{_wsPort}"
            };

            _notifyIcon.DoubleClick += OnDoubleClick;
            _notifyIcon.ContextMenuStrip = CreateContextMenu();

            _counterManager.CounterChanged += OnCounterChanged;
        }

        private WinForms.ContextMenuStrip CreateContextMenu()
        {
            var menu = new WinForms.ContextMenuStrip();

            menu.Items.Add("設定を開く", null, (s, e) => OpenSettings());
            menu.Items.Add("管理ページを開く", null, (s, e) => OpenManagerPage());
            menu.Items.Add("OBS URLをコピー", null, (s, e) => CopyObsUrl());
            menu.Items.Add(new WinForms.ToolStripSeparator());
            menu.Items.Add("設定を保存", null, (s, e) => SaveSettings());
            menu.Items.Add(new WinForms.ToolStripSeparator());
            menu.Items.Add("終了", null, (s, e) => Exit());

            return menu;
        }

        private void OnDoubleClick(object? sender, EventArgs e)
        {
            OpenSettings();
        }

        private void OpenSettings()
        {
            if (_mainWindow == null)
            {
                _mainWindow = new MainWindow(_counterManager, _configManager, _settings);
                _mainWindow.Closed += (s, e) => _mainWindow = null;
            }

            _mainWindow.Show();
            _mainWindow.Activate();
        }

        private void OpenManagerPage()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"http://localhost:{_httpPort}/",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show($"ブラウザの起動に失敗しました: {ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CopyObsUrl()
        {
            string url = $"http://localhost:{_httpPort}/obs.html";
            WpfClipboard.SetText(url);
            WpfMessageBox.Show($"OBS用URLをコピーしました:\n{url}", "コピー完了",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveSettings()
        {
            _settings.Counters = _counterManager.GetAllCounters();
            bool success = _configManager.Save(_settings);

            if (success)
            {
                WpfMessageBox.Show("設定を保存しました。", "保存完了",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                WpfMessageBox.Show("設定の保存に失敗しました。", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OnCounterChanged(object? sender, CounterChangeEventArgs e)
        {
            var counter = _counterManager.GetCounter(e.CounterId);
            if (counter != null)
            {
                _notifyIcon.ShowBalloonTip(
                    1000,
                    "カウンター更新",
                    $"{counter.Name}: {e.OldValue} → {e.NewValue}",
                    WinForms.ToolTipIcon.Info
                );
            }
        }

        public void Dispose()
        {
            _counterManager.CounterChanged -= OnCounterChanged;
            _notifyIcon.Dispose();
            _mainWindow?.Close();
        }
    }
}