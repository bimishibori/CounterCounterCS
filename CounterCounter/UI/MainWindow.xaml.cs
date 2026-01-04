// CounterCounter/UI/MainWindow.xaml.cs
using System;
using System.Windows;
using CounterCounter.Core;
using WpfClipboard = System.Windows.Clipboard;
using WpfMessageBox = System.Windows.MessageBox;

namespace CounterCounter.UI
{
    public partial class MainWindow : Window
    {
        private readonly CounterState _counterState;
        private readonly int _httpPort;
        private readonly int _wsPort;

        public MainWindow(CounterState counterState, int httpPort, int wsPort)
        {
            InitializeComponent();

            _counterState = counterState;
            _httpPort = httpPort;
            _wsPort = wsPort;

            _counterState.ValueChanged += OnCounterValueChanged;

            InitializeUI();
        }

        private void InitializeUI()
        {
            CounterValueText.Text = _counterState.GetValue().ToString();

            ObsUrlTextBox.Text = $"http://localhost:{_httpPort}/obs.html";
            HttpPortText.Text = _httpPort.ToString();
            WsPortText.Text = _wsPort.ToString();
            ServerStatusText.Text = "稼働中";
        }

        private void OnCounterValueChanged(object? sender, CounterChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                CounterValueText.Text = e.NewValue.ToString();
            });
        }

        private void IncrementButton_Click(object sender, RoutedEventArgs e)
        {
            _counterState.Increment();
        }

        private void DecrementButton_Click(object sender, RoutedEventArgs e)
        {
            _counterState.Decrement();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _counterState.Reset();
        }

        private void CopyUrlButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WpfClipboard.SetText(ObsUrlTextBox.Text);
                WpfMessageBox.Show("URLをクリップボードにコピーしました！",
                    "カウンター・カウンター",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show($"コピーに失敗しました: {ex.Message}",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OpenManagerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = $"http://localhost:{_httpPort}",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show($"ブラウザを開けませんでした: {ex.Message}",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            _counterState.ValueChanged -= OnCounterValueChanged;
            base.OnClosed(e);
        }
    }
}