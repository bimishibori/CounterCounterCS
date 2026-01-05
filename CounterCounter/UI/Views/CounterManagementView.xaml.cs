// CounterCounter/UI/Views/CounterManagementView.xaml.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using CounterCounter.Core;
using CounterCounter.Models;
using CounterCounter.UI.Dialogs;
using WpfButton = System.Windows.Controls.Button;
using WpfMessageBox = System.Windows.MessageBox;
using WpfUserControl = System.Windows.Controls.UserControl;
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace CounterCounter.UI.Views
{
    public partial class CounterManagementView : WpfUserControl
    {
        private readonly CounterManager _counterManager;
        private readonly List<HotkeySettings> _hotkeySettings;
        private ObservableCollection<CounterDisplayModel> _counters;

        public CounterManagementView(CounterManager counterManager, List<HotkeySettings> hotkeySettings)
        {
            InitializeComponent();
            _counterManager = counterManager;
            _hotkeySettings = hotkeySettings;
            _counters = new ObservableCollection<CounterDisplayModel>();

            _counterManager.CounterChanged += OnCounterChanged;
            RefreshCounterList();
        }

        private void OnCounterChanged(object? sender, CounterChangedEventArgs e)
        {
            Dispatcher.Invoke(() => RefreshCounterList());
        }

        private void RefreshCounterList()
        {
            var counters = _counterManager.GetAllCounters();
            _counters.Clear();

            foreach (var counter in counters)
            {
                var hotkey = _hotkeySettings.FirstOrDefault(h =>
                    h.CounterId == counter.Id && h.Action == HotkeyAction.Increment);

                string hotkeyText = "ショートカット: ";
                if (hotkey != null)
                {
                    hotkeyText += $"[{hotkey.GetDisplayText()}] で増加";
                }
                else
                {
                    hotkeyText += "未設定";
                }

                _counters.Add(new CounterDisplayModel
                {
                    Id = counter.Id,
                    Name = counter.Name,
                    Value = counter.Value,
                    Color = counter.Color,
                    HotkeyText = hotkeyText
                });
            }

            CountersListBox.ItemsSource = _counters;
        }

        private void AddCounter_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CounterEditDialog();
            if (dialog.ShowDialog() == true)
            {
                _counterManager.AddCounter(dialog.CounterName, dialog.CounterColor);
                RefreshCounterList();
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
            }
        }

        private void IncrementCounter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not WpfButton button || button.Tag is not string counterId)
                return;

            _counterManager.Increment(counterId);
        }

        private void DecrementCounter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not WpfButton button || button.Tag is not string counterId)
                return;

            _counterManager.Decrement(counterId);
        }

        private void ResetCounter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not WpfButton button || button.Tag is not string counterId)
                return;

            _counterManager.Reset(counterId);
        }
    }

    public class CounterDisplayModel : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        private string _name = string.Empty;
        private int _value;
        private string _color = "#ffffff";
        private string _hotkeyText = string.Empty;

        public string Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public int Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(); }
        }

        public string Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ColorValue));
            }
        }

        public WpfColor ColorValue
        {
            get => (WpfColor)WpfColorConverter.ConvertFromString(_color);
        }

        public string HotkeyText
        {
            get => _hotkeyText;
            set { _hotkeyText = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}