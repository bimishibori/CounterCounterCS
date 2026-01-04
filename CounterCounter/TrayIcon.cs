using System;
using System.Windows.Forms;
using System.Drawing;

namespace CounterCounter
{
    public class TrayIcon : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;
        private CounterState _counterState;
        private int _port;
        private int _wsPort;

        public TrayIcon(CounterState counterState, int port, int wsPort)
        {
            _counterState = counterState;
            _port = port;
            _wsPort = wsPort;

            // イベント購読
            _counterState.ValueChanged += OnCounterValueChanged;

            InitializeTrayIcon();
        }

        private void OnCounterValueChanged(object? sender, CounterChangedEventArgs e)
        {
            // 値が変わったらツールチップに表示
            _notifyIcon.Text = $"カウンター・カウンター: {e.NewValue} (HTTP:{_port} WS:{_wsPort})";
        }

        private void InitializeTrayIcon()
        {
            // コンテキストメニューの作成
            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("設定を開く", null, OnOpenSettings);
            _contextMenu.Items.Add("管理ページを開く", null, OnOpenManager);
            _contextMenu.Items.Add("OBS URLをコピー", null, OnCopyUrl);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add("再起動", null, OnRestart);
            _contextMenu.Items.Add("終了", null, OnExit);

            // トレイアイコンの作成
            _notifyIcon = new NotifyIcon
            {
                Text = $"カウンター・カウンター (HTTP:{_port} WS:{_wsPort})",
                ContextMenuStrip = _contextMenu,
                Visible = true
            };

            // 仮アイコン（後でちゃんとした画像に変更）
            _notifyIcon.Icon = SystemIcons.Application;

            // ダブルクリックで設定画面を開く
            _notifyIcon.DoubleClick += (s, e) => OnOpenSettings(s, e);
        }

        private void OnOpenSettings(object sender, EventArgs e)
        {
            // テスト用：カウンターを増やす
            _counterState.Increment();
            MessageBox.Show($"カウンター値: {_counterState.GetValue()}\n\n増加しました！",
                "カウンター・カウンター");
        }

        private void OnOpenManager(object sender, EventArgs e)
        {
            // ブラウザでhttp://localhost:{port}を開く
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = $"http://localhost:{_port}",
                UseShellExecute = true
            });
        }

        private void OnCopyUrl(object sender, EventArgs e)
        {
            // クリップボードにURLをコピー
            string url = $"http://localhost:{_port}/obs.html";
            Clipboard.SetText(url);
            MessageBox.Show($"OBS用URLをクリップボードにコピーしました！\n{url}",
                "カウンター・カウンター",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void OnRestart(object sender, EventArgs e)
        {
            MessageBox.Show("再起動（未実装）", "カウンター・カウンター");
        }

        private void OnExit(object sender, EventArgs e)
        {
            // アプリケーション終了
            _notifyIcon.Visible = false;
            System.Windows.Application.Current.Shutdown();
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
            _contextMenu?.Dispose();
        }
    }
}