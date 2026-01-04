// CounterCounter/UI/MainWindow.xaml.cs
using System.Diagnostics;
using System.Windows;
using CounterCounter.Core;
using CounterCounter.Models;
using WpfButton = System.Windows.Controls.Button;
using WpfClipboard = System.Windows.Clipboard;
using WpfGroupBox = System.Windows.Controls.GroupBox;
using WpfMessageBox = System.Windows.MessageBox;
using WpfOrientation = System.Windows.Controls.Orientation;
using WpfStackPanel = System.Windows.Controls.StackPanel;
using WpfTextBlock = System.Windows.Controls.TextBlock;

namespace CounterCounter.UI
{
    public partial class MainWindow : Window
    {
        private readonly CounterManager _counterManager;
        private readonly int _httpPort;
        private readonly int _wsPort;

        public MainWindow(CounterManager counterManager, int httpPort, int wsPort)
        {
            InitializeComponent();
            _counterManager = counterManager;
            _httpPort = httpPort;
            _wsPort = wsPort;

            _counterManager.CounterChanged += OnCounterChanged;

            RefreshCounterList();
            RefreshHotkeyList();
            UpdateConnectionInfo();
        }

        protected override void OnClosed(EventArgs e)
        {
            _counterManager.CounterChanged -= OnCounterChanged;
            Hide();
            base.OnClosed(e);
        }

        private void OnCounterChanged(object? sender, CounterChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                RefreshCounterList();
            });
        }

        private void RefreshCounterList()
        {
            var counters = _counterManager.GetAllCounters();
            CounterListBox.ItemsSource = null;
            CounterListBox.ItemsSource = counters;
        }

        private void RefreshHotkeyList()
        {
            HotkeyPanel.Children.Clear();

            var counters = _counterManager.GetAllCounters();
            var hotkeySettings = ((App)Application.Current).Settings?.Hotkeys ?? new List<HotkeySettings>();

            foreach (var counter in counters)
            {
                var groupBox = new WpfGroupBox
                {
                    Header = counter.Name,
                    Margin = new Thickness(0, 0, 0, 15),
                    Padding = new Thickness(15),
                    Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
                    BorderBrush = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3a3a3a")),
                    BorderThickness = new Thickness(1)
                };

                var panel = new WpfStackPanel { Orientation = WpfOrientation.Vertical };

                var incrementHotkey = hotkeySettings.FirstOrDefault(h =>
                    h.CounterId == counter.Id && h.Action == HotkeyAction.Increment);
                var decrementHotkey = hotkeySettings.FirstOrDefault(h =>
                    h.CounterId == counter.Id && h.Action == HotkeyAction.Decrement);
                var resetHotkey = hotkeySettings.FirstOrDefault(h =>
                    h.CounterId == counter.Id && h.Action == HotkeyAction.Reset);

                panel.Children.Add(CreateHotkeyRow("増加:",
                    incrementHotkey?.GetDisplayText() ?? "未設定"));
                panel.Children.Add(CreateHotkeyRow("減少:",
                    decrementHotkey?.GetDisplayText() ?? "未設定"));
                panel.Children.Add(CreateHotkeyRow("リセット:",
                    resetHotkey?.GetDisplayText() ?? "未設定"));

                groupBox.Content = panel;
                HotkeyPanel.Children.Add(groupBox);
            }
        }

        private WpfStackPanel CreateHotkeyRow(string label, string keyText)
        {
            var row = new WpfStackPanel
            {
                Orientation = WpfOrientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 5)
            };

            var labelBlock = new WpfTextBlock
            {
                Text = label,
                Width = 80,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Colors.LightGray),
                VerticalAlignment = VerticalAlignment.Center
            };

            var keyBlock = new WpfTextBlock
            {
                Text = keyText,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0d7377")),
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center
            };

            row.Children.Add(labelBlock);
            row.Children.Add(keyBlock);

            return row;
        }

        private void UpdateConnectionInfo()
        {
            ObsUrlText.Text = $"http://localhost:{_httpPort}/obs.html";
            HttpPortText.Text = _httpPort.ToString();
            WsPortText.Text = _wsPort.ToString();
        }

        private void AddCounter_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CounterEditDialog();
            if (dialog.ShowDialog() == true)
            {
                _counterManager.AddCounter(dialog.CounterName, dialog.CounterColor);
                RefreshCounterList();
                RefreshHotkeyList();
            }
        }

        private void EditCounter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not WpfButton button || button.Tag is not string counterId)
                return;

            var counter = _counterManager.GetCounter(counterId);
            if (counter == null)
                return;

            var dialog = new CounterEditDialog(counter.Name, counter.Color);
            if (dialog.ShowDialog() == true)
            {
                _counterManager.UpdateCounter(counterId, dialog.CounterName, dialog.CounterColor);
                RefreshCounterList();
                RefreshHotkeyList();
            }
        }

        private void DeleteCounter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not WpfButton button || button.Tag is not string counterId)
                return;

            var counter = _counterManager.GetCounter(counterId);
            if (counter == null)
                return;

            var result = WpfMessageBox.Show(
                $"カウンター「{counter.Name}」を削除しますか？",
                "削除確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                _counterManager.RemoveCounter(counterId);
                RefreshCounterList();
                RefreshHotkeyList();
            }
        }

        private void CounterListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void IncrementCounter_Click(object sender, RoutedEventArgs e)
        {
            if (CounterListBox.SelectedItem is Counter counter)
            {
                _counterManager.Increment(counter.Id);
            }
        }

        private void DecrementCounter_Click(object sender, RoutedEventArgs e)
        {
            if (CounterListBox.SelectedItem is Counter counter)
            {
                _counterManager.Decrement(counter.Id);
            }
        }

        private void ResetCounter_Click(object sender, RoutedEventArgs e)
        {
            if (CounterListBox.SelectedItem is Counter counter)
            {
                _counterManager.Reset(counter.Id);
            }
        }

        private void OpenObsUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"http://localhost:{_httpPort}/obs.html",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(
                    $"ブラウザを開けませんでした: {ex.Message}",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void CopyObsUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WpfClipboard.SetText($"http://localhost:{_httpPort}/obs.html");
                WpfMessageBox.Show(
                    "URLをクリップボードにコピーしました",
                    "コピー完了",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(
                    $"クリップボードへのコピーに失敗しました: {ex.Message}",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}