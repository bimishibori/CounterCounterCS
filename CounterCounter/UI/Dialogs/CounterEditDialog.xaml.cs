// CounterCounter/UI/Dialogs/CounterEditDialog.xaml.cs
using System.Windows;
using System.Windows.Input;
using CounterCounter.Models;
using WpfMessageBox = System.Windows.MessageBox;
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;
using WinForms = System.Windows.Forms;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace CounterCounter.UI.Dialogs
{
    public partial class CounterEditDialog : Window
    {
        private string _selectedColor = "#51cf66";
        private bool _isRecordingIncrementKey = false;
        private bool _isRecordingDecrementKey = false;

        private uint _incrementModifiers = 0;
        private uint _incrementVirtualKey = 0;
        private uint _decrementModifiers = 0;
        private uint _decrementVirtualKey = 0;

        public Counter? Counter { get; private set; }
        public string CounterName => NameTextBox.Text.Trim();
        public string CounterColor => _selectedColor;

        public HotkeySettings? IncrementHotkey { get; private set; }
        public HotkeySettings? DecrementHotkey { get; private set; }

        public CounterEditDialog()
        {
            InitializeComponent();
            Title = "新規カウンター追加";
            UpdateColorPreview();
        }

        public CounterEditDialog(Counter counter, List<HotkeySettings> existingHotkeys) : this()
        {
            Title = "カウンター編集";
            Counter = counter;
            NameTextBox.Text = counter.Name;
            _selectedColor = counter.Color;
            UpdateColorPreview();

            var incrementHotkey = existingHotkeys.FirstOrDefault(h =>
                h.CounterId == counter.Id && h.Action == HotkeyAction.Increment);
            var decrementHotkey = existingHotkeys.FirstOrDefault(h =>
                h.CounterId == counter.Id && h.Action == HotkeyAction.Decrement);

            if (incrementHotkey != null)
            {
                _incrementModifiers = incrementHotkey.Modifiers;
                _incrementVirtualKey = incrementHotkey.VirtualKey;
                IncrementKeyTextBox.Text = incrementHotkey.GetDisplayText();
            }

            if (decrementHotkey != null)
            {
                _decrementModifiers = decrementHotkey.Modifiers;
                _decrementVirtualKey = decrementHotkey.VirtualKey;
                DecrementKeyTextBox.Text = decrementHotkey.GetDisplayText();
            }
        }

        public CounterEditDialog(string name, string color, List<HotkeySettings> existingHotkeys) : this()
        {
            Title = "カウンター編集";
            NameTextBox.Text = name;
            _selectedColor = color;
            UpdateColorPreview();
        }

        private void UpdateColorPreview()
        {
            try
            {
                var color = (WpfColor)WpfColorConverter.ConvertFromString(_selectedColor);
                ColorPreview.Background = new WpfSolidColorBrush(color);
            }
            catch
            {
                ColorPreview.Background = new WpfSolidColorBrush(
                    (WpfColor)WpfColorConverter.ConvertFromString("#51cf66"));
            }
        }

        private void SelectColor_Click(object sender, RoutedEventArgs e)
        {
            using var colorDialog = new WinForms.ColorDialog();

            try
            {
                var currentColor = (WpfColor)WpfColorConverter.ConvertFromString(_selectedColor);
                colorDialog.Color = System.Drawing.Color.FromArgb(
                    currentColor.A, currentColor.R, currentColor.G, currentColor.B);
            }
            catch
            {
            }

            colorDialog.FullOpen = true;

            if (colorDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                var drawingColor = colorDialog.Color;
                _selectedColor = $"#{drawingColor.R:X2}{drawingColor.G:X2}{drawingColor.B:X2}";
                UpdateColorPreview();
            }
        }

        private void RecordIncrementKey_Click(object sender, RoutedEventArgs e)
        {
            _isRecordingIncrementKey = true;
            _isRecordingDecrementKey = false;
            IncrementKeyTextBox.Text = "キーを押してください...";
            IncrementKeyTextBox.Focus();
        }

        private void RecordDecrementKey_Click(object sender, RoutedEventArgs e)
        {
            _isRecordingDecrementKey = true;
            _isRecordingIncrementKey = false;
            DecrementKeyTextBox.Text = "キーを押してください...";
            DecrementKeyTextBox.Focus();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (!_isRecordingIncrementKey && !_isRecordingDecrementKey)
            {
                return;
            }

            e.Handled = true;

            if (e.Key == Key.Escape)
            {
                if (_isRecordingIncrementKey)
                {
                    IncrementKeyTextBox.Text = _incrementVirtualKey == 0 ? "未設定" :
                        new HotkeySettings("", HotkeyAction.Increment, _incrementModifiers, _incrementVirtualKey).GetDisplayText();
                    _isRecordingIncrementKey = false;
                }
                else if (_isRecordingDecrementKey)
                {
                    DecrementKeyTextBox.Text = _decrementVirtualKey == 0 ? "未設定" :
                        new HotkeySettings("", HotkeyAction.Decrement, _decrementModifiers, _decrementVirtualKey).GetDisplayText();
                    _isRecordingDecrementKey = false;
                }
                return;
            }

            uint modifiers = 0;
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                modifiers |= 0x0002;
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                modifiers |= 0x0004;
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
                modifiers |= 0x0001;

            if (modifiers == 0)
            {
                WpfMessageBox.Show(
                    "Ctrl、Shift、Altのいずれかの修飾キーを押してください。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            uint virtualKey = (uint)KeyInterop.VirtualKeyFromKey(e.Key);

            if (virtualKey == 0)
            {
                return;
            }

            var tempHotkey = new HotkeySettings("temp", HotkeyAction.Increment, modifiers, virtualKey);
            string displayText = tempHotkey.GetDisplayText();

            if (_isRecordingIncrementKey)
            {
                _incrementModifiers = modifiers;
                _incrementVirtualKey = virtualKey;
                IncrementKeyTextBox.Text = displayText;
                _isRecordingIncrementKey = false;
            }
            else if (_isRecordingDecrementKey)
            {
                _decrementModifiers = modifiers;
                _decrementVirtualKey = virtualKey;
                DecrementKeyTextBox.Text = displayText;
                _isRecordingDecrementKey = false;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                WpfMessageBox.Show("カウンター名を入力してください", "エラー");
                return;
            }

            if (Counter == null)
            {
                Counter = new Counter
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    Value = 0,
                    Color = _selectedColor
                };
            }
            else
            {
                Counter.Name = name;
                Counter.Color = _selectedColor;
            }

            if (_incrementModifiers != 0 && _incrementVirtualKey != 0)
            {
                IncrementHotkey = new HotkeySettings(
                    Counter.Id,
                    HotkeyAction.Increment,
                    _incrementModifiers,
                    _incrementVirtualKey
                );
            }

            if (_decrementModifiers != 0 && _decrementVirtualKey != 0)
            {
                DecrementHotkey = new HotkeySettings(
                    Counter.Id,
                    HotkeyAction.Decrement,
                    _decrementModifiers,
                    _decrementVirtualKey
                );
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}