// CounterCounter/UI/Views/CounterManagementView.xaml.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using CounterCounter.Core;
using CounterCounter.Models;
using CounterCounter.UI.Dialogs;
using WpfMessageBox = System.Windows.MessageBox;
using WpfUserControl = System.Windows.Controls.UserControl;
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;

namespace CounterCounter.UI.Views
{
    public partial class CounterManagementView : WpfUserControl
    {
        private readonly CounterManager _counterManager;
        private readonly List<HotkeySettings> _hotkeySettings;
        private readonly ConfigManager _configManager;
        private readonly CounterSettings _settings;
        private ObservableCollection<CounterDisplayModel> _counters;

        public CounterManagementView(
            CounterManager counterManager,
            List<HotkeySettings> hotkeySettings,
            ConfigManager configManager,
            CounterSettings settings)
        {
            InitializeComponent();
            _counterManager = counterManager;
            _hotkeySettings = hotkeySettings;
            _configManager = configManager;
            _settings = settings;
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
                var incrementHotkeys = _hotkeySettings.Where(h =>
                    h.CounterId == counter.Id && h.Action == HotkeyAction.Increment).ToList();
                var decrementHotkeys = _hotkeySettings.Where(h =>
                    h.CounterId == counter.Id && h.Action == HotkeyAction.Decrement).ToList();

                string hotkeyText = "ショートカット: ";
                if (incrementHotkeys.Any() || decrementHotkeys.Any())
                {
                    var parts = new List<string>();
                    if (incrementHotkeys.Any())
                    {
                        var keys = string.Join(", ", incrementHotkeys.Select(h => h.GetDisplayText()));
                        parts.Add($"増加[{keys}]");
                    }
                    if (decrementHotkeys.Any())
                    {
                        var keys = string.Join(", ", decrementHotkeys.Select(h => h.GetDisplayText()));
                        parts.Add($"減少[{keys}]");
                    }
                    hotkeyText += string.Join(" ", parts);
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
                    HotkeyText = hotkeyText,
                    ShowInRotation = counter.ShowInRotation
                });
            }

            CountersListBox.ItemsSource = _counters;
        }

        private void AddCounter_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CounterEditDialog(_hotkeySettings);
            if (dialog.ShowDialog() == true)
            {
                var counter = new Counter
                {
                    Id = dialog.Counter!.Id,
                    Name = dialog.CounterName,
                    Color = dialog.CounterColor,
                    Value = 0,
                    ShowInRotation = true
                };

                _counterManager.AddCounter(counter);

                _hotkeySettings.AddRange(dialog.IncrementHotkeys);
                _hotkeySettings.AddRange(dialog.DecrementHotkeys);

                AutoSaveSettings();
                RefreshCounterList();
            }
        }

        private void CounterCard_IncrementRequested(object? sender, string counterId)
        {
            _counterManager.Increment(counterId);
        }

        private void CounterCard_DecrementRequested(object? sender, string counterId)
        {
            _counterManager.Decrement(counterId);
        }

        private void CounterCard_ResetRequested(object? sender, string counterId)
        {
            _counterManager.Reset(counterId);
        }

        private void CounterCard_EditRequested(object? sender, string counterId)
        {
            var counter = _counterManager.GetCounter(counterId);
            if (counter == null)
                return;

            var dialog = new CounterEditDialog(counter, _hotkeySettings);
            if (dialog.ShowDialog() == true)
            {
                _counterManager.UpdateCounter(counterId, dialog.CounterName, dialog.CounterColor);

                _hotkeySettings.RemoveAll(h => h.CounterId == counterId);
                _hotkeySettings.AddRange(dialog.IncrementHotkeys);
                _hotkeySettings.AddRange(dialog.DecrementHotkeys);

                AutoSaveSettings();
                RefreshCounterList();
            }
        }

        private void CounterCard_DeleteRequested(object? sender, string counterId)
        {
            var counter = _counterManager.GetCounter(counterId);
            if (counter == null)
                return;

            var result = WpfMessageBox.Show(
                $"カウンター「{counter.Name}」を削除しますか?",
                "削除確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                _counterManager.RemoveCounter(counterId);
                _hotkeySettings.RemoveAll(h => h.CounterId == counterId);
                AutoSaveSettings();
                RefreshCounterList();
            }
        }

        private void CounterCard_ShowInRotationChanged(object? sender, (string counterId, bool showInRotation) args)
        {
            var counter = _counterManager.GetCounter(args.counterId);
            if (counter != null)
            {
                counter.ShowInRotation = args.showInRotation;
                AutoSaveSettings();
            }
        }

        private void AutoSaveSettings()
        {
            _settings.Counters = _counterManager.GetAllCounters();
            _settings.Hotkeys = _hotkeySettings;
            _configManager.Save(_settings);
        }
    }

    public class CounterDisplayModel : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        private string _name = string.Empty;
        private int _value;
        private string _color = "#ffffff";
        private string _hotkeyText = string.Empty;
        private bool _showInRotation = true;

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

        public bool ShowInRotation
        {
            get => _showInRotation;
            set { _showInRotation = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}