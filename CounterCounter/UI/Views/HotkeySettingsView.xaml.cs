// CounterCounter/UI/Views/HotkeySettingsView.xaml.cs
using System.Windows;
using CounterCounter.Core;
using CounterCounter.Models;
using WpfGroupBox = System.Windows.Controls.GroupBox;
using WpfOrientation = System.Windows.Controls.Orientation;
using WpfStackPanel = System.Windows.Controls.StackPanel;
using WpfTextBlock = System.Windows.Controls.TextBlock;
using WpfUserControl = System.Windows.Controls.UserControl;

namespace CounterCounter.UI.Views
{
    public partial class HotkeySettingsView : WpfUserControl
    {
        private readonly CounterManager _counterManager;
        private readonly List<HotkeySettings> _hotkeySettings;

        public HotkeySettingsView(CounterManager counterManager, List<HotkeySettings> hotkeySettings)
        {
            InitializeComponent();
            _counterManager = counterManager;
            _hotkeySettings = hotkeySettings;
            RefreshHotkeyList();
        }

        public void RefreshHotkeyList()
        {
            HotkeyPanel.Children.Clear();

            var counters = _counterManager.GetAllCounters();

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

                var incrementHotkey = _hotkeySettings.FirstOrDefault(h =>
                    h.CounterId == counter.Id && h.Action == HotkeyAction.Increment);
                var decrementHotkey = _hotkeySettings.FirstOrDefault(h =>
                    h.CounterId == counter.Id && h.Action == HotkeyAction.Decrement);
                var resetHotkey = _hotkeySettings.FirstOrDefault(h =>
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
    }
}