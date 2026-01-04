// CounterCounter/UI/MainWindow.xaml.cs
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using CounterCounter.Core;
using CounterCounter.Models;
using WpfClipboard = System.Windows.Clipboard;
using WpfMessageBox = System.Windows.MessageBox;
using WpfButton = System.Windows.Controls.Button;
using WpfGroupBox = System.Windows.Controls.GroupBox;
using WpfOrientation = System.Windows.Controls.Orientation;
using WpfStackPanel = System.Windows.Controls.StackPanel;
using WpfTextBlock = System.Windows.Controls.TextBlock;

namespace CounterCounter.UI
{
    public partial class MainWindow : Window
    {
        private readonly CounterManager _counterManager;
        private readonly ConfigManager _configManager;
        private readonly CounterSettings _settings;
        private readonly int _httpPort;
        private readonly int _wsPort;

        public MainWindow(CounterManager counterManager, ConfigManager configManager, CounterSettings settings)
        {
            InitializeComponent();

            _counterManager = counterManager;
            _configManager = configManager;
            _settings = settings;
            _httpPort = settings.ServerPort;
            _wsPort = settings.ServerPort + 1;

            Closing += (s, e) =>
            {
                e.Cancel = true;
                Hide();
            };

            _counterManager.CounterChanged += OnCounterChanged;

            RefreshCounterList();
            RefreshHotkeyPanel();
            InitializeConnectionInfo();
        }

        private void RefreshCounterList()
        {
            var counters = _counterManager.GetAllCounters();
            CounterListBox.ItemsSource = counters;
        }

        private void RefreshHotkeyPanel()
        {
            HotkeyPanel.Children.Clear();

            var counters = _counterManager.GetAllCounters();
            foreach (var counter in counters)
            {
                var group = new WpfGroupBox
                {
                    Header = counter.Name,
                    Margin = new Thickness(0, 10, 0, 0),
                    Foreground = System.Windows.Media.Brushes.White,
                    BorderBrush = System.Windows.Media.Brushes.Gray
                };

                var stack = new WpfStackPanel();

                var incrementHotkey = _settings.Hotkeys.FirstOrDefault(h => h.CounterId == counter.Id && h.Action == HotkeyAction.Increment);
                var decrementHotkey = _settings.Hotkeys.FirstOrDefault(h => h.CounterId == counter.Id && h.Action == HotkeyAction.Decrement);
                var resetHotkey = _settings.Hotkeys.FirstOrDefault(h => h.CounterId == counter.Id && h.Action == HotkeyAction.Reset);

                stack.Children.Add(CreateHotkeyRow("増加", incrementHotkey?.GetDisplayText() ?? "未設定"));
                stack.Children.Add(CreateHotkeyRow("減少", decrementHotkey?.GetDisplayText() ?? "未設定"));
                stack.Children.Add(CreateHotkeyRow("リセット", resetHotkey?.GetDisplayText() ?? "未設定"));

                group.Content = stack;
                HotkeyPanel.Children.Add(group);
            }
        }

        private WpfStackPanel CreateHotkeyRow(string label, string hotkey)
        {
            var panel = new WpfStackPanel { Orientation = WpfOrientation.Horizontal, Margin = new Thickness(5) };
            panel.Children.Add(new WpfTextBlock { Text = $"{label}:", Width = 80 });
            panel.Children.Add(new WpfTextBlock { Text = hotkey, Width = 150, FontWeight = FontWeights.Bold });
            return panel;
        }

        private void InitializeConnectionInfo()
        {
            ObsUrlTextBox.Text = $"http://localhost:{_httpPort}/obs.html";
            ManagerUrlTextBox.Text = $"http://localhost:{_httpPort}/";
            ServerInfoTextBox.Text = $"HTTPポート: {_httpPort}\nWebSocketポート: {_wsPort}\n状態: 稼働中";
        }

        private void AddCounter_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CounterEditDialog(null);
            if (dialog.ShowDialog() == true)
            {
                var counter = new Counter
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = dialog.CounterName,
                    Value = 0,
                    Color = dialog.CounterColor
                };

                _counterManager.AddCounter(counter);
                _settings.Counters = _counterManager.GetAllCounters();
                _configManager.Save(_settings);
                RefreshCounterList();
                RefreshHotkeyPanel();
            }
        }

        private void EditCounter_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as WpfButton;
            var counterId = button?.Tag as string;
            if (string.IsNullOrEmpty(counterId)) return;

            var counter = _counterManager.GetCounter(counterId);
            if (counter == null) return;

            var dialog = new CounterEditDialog(counter);
            if (dialog.ShowDialog() == true)
            {
                _counterManager.UpdateCounter(counterId, dialog.CounterName, dialog.CounterColor);
                _settings.Counters = _counterManager.GetAllCounters();
                _configManager.Save(_settings);
                RefreshCounterList();
                RefreshHotkeyPanel();
            }
        }

        private void DeleteCounter_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as WpfButton;
            var counterId = button?.Tag as string;
            if (string.IsNullOrEmpty(counterId)) return;

            var result = WpfMessageBox.Show("このカウンターを削除しますか？", "確認",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _counterManager.RemoveCounter(counterId);
                _settings.Counters = _counterManager.GetAllCounters();
                _settings.Hotkeys.RemoveAll(h => h.CounterId == counterId);
                _configManager.Save(_settings);
                RefreshCounterList();
                RefreshHotkeyPanel();
            }
        }

        private void CounterListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void IncrementSelected_Click(object sender, RoutedEventArgs e)
        {
            var counter = CounterListBox.SelectedItem as Counter;
            if (counter != null)
            {
                _counterManager.Increment(counter.Id);
            }
        }

        private void DecrementSelected_Click(object sender, RoutedEventArgs e)
        {
            var counter = CounterListBox.SelectedItem as Counter;
            if (counter != null)
            {
                _counterManager.Decrement(counter.Id);
            }
        }

        private void ResetSelected_Click(object sender, RoutedEventArgs e)
        {
            var counter = CounterListBox.SelectedItem as Counter;
            if (counter != null)
            {
                _counterManager.Reset(counter.Id);
            }
        }

        private void CopyObsUrl_Click(object sender, RoutedEventArgs e)
        {
            WpfClipboard.SetText(ObsUrlTextBox.Text);
            WpfMessageBox.Show("URLをコピーしました！", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenManager_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = ManagerUrlTextBox.Text, UseShellExecute = true });
        }

        private void OnCounterChanged(object? sender, CounterChangeEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                RefreshCounterList();
            });
        }
    }
}