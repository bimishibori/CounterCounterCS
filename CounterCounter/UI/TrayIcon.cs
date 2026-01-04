// CounterCounter/UI/TrayIcon.cs
using System;
using System.Drawing;
using CounterCounter.Core;
using WinForms = System.Windows.Forms;

namespace CounterCounter.UI
{
    public class TrayIcon : IDisposable
    {
        private WinForms.NotifyIcon _notifyIcon;
        private WinForms.ContextMenuStrip _contextMenu;
        private CounterState _counterState;
        private MainWindow? _mainWindow;
        private int _port;
        private int _wsPort;

        public TrayIcon(CounterState counterState, int port, int wsPort)
        {
            _counterState = counterState;
            _port = port;
            _wsPort = wsPort;

            _counterState.ValueChanged += OnCounterValueChanged;

            InitializeTrayIcon();
        }

        private void OnCounterValueChanged(object? sender, CounterChangedEventArgs e)
        {
            _notifyIcon.Text = $"カウンター・カウンター: {e.NewValue} (HTTP:{_port} WS:{_wsPort})";
        }

        private void InitializeTrayIcon()
        {
            _contextMenu = new WinForms.ContextMenuStrip();
            _contextMenu.Items.Add("設定を開く", null, OnOpenSettings);
            _contextMenu.Items.Add("管理ページを開く", null, OnOpenManager);
            _contextMenu.Items.Add("OBS URLをコピー", null, OnCopyUrl);
            _contextMenu.Items.Add(new WinForms.ToolStripSeparator());
            _contextMenu.Items.Add("終了", null, OnExit);

            _notifyIcon = new WinForms.NotifyIcon
            {
                Text = $"カウンター・カウンター (HTTP:{_port} WS:{_wsPort})",
                ContextMenuStrip = _contextMenu,
                Visible = true
            };

            _notifyIcon.Icon = SystemIcons.Application;
            _notifyIcon.DoubleClick += (s, e) => OnOpenSettings(s, e);
        }

        private void OnOpenSettings(object? sender, EventArgs e)
        {
            if (_mainWindow == null)
            {
                _mainWindow = new MainWindow(_counterState, _port, _wsPort);
            }

            if (_mainWindow.IsVisible)
            {
                _mainWindow.Activate();
            }
            else
            {
                _mainWindow.Show();
            }
        }

        private void OnOpenManager(object? sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = $"http://localhost:{_port}",
                UseShellExecute = true
            });
        }

        private void OnCopyUrl(object? sender, EventArgs e)
        {
            string url = $"http://localhost:{_port}/obs.html";
            WinForms.Clipboard.SetText(url);
            WinForms.MessageBox.Show($"OBS用URLをクリップボードにコピーしました！\n{url}",
                "カウンター・カウンター",
                WinForms.MessageBoxButtons.OK,
                WinForms.MessageBoxIcon.Information);
        }

        private void OnExit(object? sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            System.Windows.Application.Current.Shutdown();
        }

        public void Dispose()
        {
            _counterState.ValueChanged -= OnCounterValueChanged;
            _mainWindow?.Close();
            _notifyIcon?.Dispose();
            _contextMenu?.Dispose();
        }
    }
}