// CounterCounter/UI/CounterEditDialog.xaml.cs
using System.Windows;
using CounterCounter.Models;
using WpfKeyEventArgs = System.Windows.Input.KeyEventArgs;
using WpfKey = System.Windows.Input.Key;
using WpfKeyboard = System.Windows.Input.Keyboard;
using WpfModifierKeys = System.Windows.Input.ModifierKeys;
using WpfKeyInterop = System.Windows.Input.KeyInterop;
using WpfMessageBox = System.Windows.MessageBox;
using WpfButton = System.Windows.Controls.Button;

namespace CounterCounter.UI
{
    public partial class CounterEditDialog : Window
    {
        private string _selectedColor = "#51cf66";
        private HotkeySettings? _incrementHotkey;
        private HotkeySettings? _decrementHotkey;
        private bool _isWaitingForIncrementKey;
        private bool _isWaitingForDecrementKey;

        public Counter? Counter { get; private set; }
        public List<HotkeySettings> Hotkeys { get; } = new List<HotkeySettings>();

        public CounterEditDialog(Counter? counter)
        {
            InitializeComponent();

            if (counter != null)
            {
                Title = "カウンター編集";
                Counter = counter;
                NameTextBox.Text = counter.Name;
                _selectedColor = counter.Color;
                UpdateColorSelection();
            }
            else
            {
                Title = "新規カウンター追加";
            }

            KeyDown += Dialog_KeyDown;
        }

        private void UpdateColorSelection()
        {
            ColorRedButton.Opacity = _selectedColor == "#ff6b6b" ? 1.0 : 0.5;
            ColorGreenButton.Opacity = _selectedColor == "#51cf66" ? 1.0 : 0.5;
            ColorBlueButton.Opacity = _selectedColor == "#339af0" ? 1.0 : 0.5;
            ColorYellowButton.Opacity = _selectedColor == "#ffd43b" ? 1.0 : 0.5;
            ColorPurpleButton.Opacity = _selectedColor == "#cc5de8" ? 1.0 : 0.5;
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is WpfButton button && button.Tag is string color)
            {
                _selectedColor = color;
                UpdateColorSelection();
            }
        }

        private void SetIncrementKey_Click(object sender, RoutedEventArgs e)
        {
            _isWaitingForIncrementKey = true;
            _isWaitingForDecrementKey = false;
            IncrementKeyTextBox.Text = "キーを押してください...";
            IncrementKeyTextBox.Focus();
        }

        private void SetDecrementKey_Click(object sender, RoutedEventArgs e)
        {
            _isWaitingForIncrementKey = false;
            _isWaitingForDecrementKey = true;
            DecrementKeyTextBox.Text = "キーを押してください...";
            DecrementKeyTextBox.Focus();
        }

        private void Dialog_KeyDown(object sender, WpfKeyEventArgs e)
        {
            if (!_isWaitingForIncrementKey && !_isWaitingForDecrementKey)
                return;

            if (e.Key == WpfKey.Escape)
            {
                CancelKeyWaiting();
                return;
            }

            if (e.Key == WpfKey.System || e.Key == WpfKey.LeftCtrl || e.Key == WpfKey.RightCtrl ||
                e.Key == WpfKey.LeftShift || e.Key == WpfKey.RightShift ||
                e.Key == WpfKey.LeftAlt || e.Key == WpfKey.RightAlt)
            {
                return;
            }

            uint modifiers = 0;
            if (WpfKeyboard.Modifiers.HasFlag(WpfModifierKeys.Control)) modifiers |= 0x0002;
            if (WpfKeyboard.Modifiers.HasFlag(WpfModifierKeys.Shift)) modifiers |= 0x0004;
            if (WpfKeyboard.Modifiers.HasFlag(WpfModifierKeys.Alt)) modifiers |= 0x0001;

            if (modifiers == 0)
            {
                WpfMessageBox.Show(
                    "Ctrl、Shift、Altのいずれかと組み合わせてキーを押してください。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                CancelKeyWaiting();
                return;
            }

            uint vk = (uint)WpfKeyInterop.VirtualKeyFromKey(e.Key);
            var displayText = GetKeyDisplayText(modifiers, e.Key);

            if (_isWaitingForIncrementKey)
            {
                _incrementHotkey = new HotkeySettings
                {
                    CounterId = Counter?.Id ?? string.Empty,
                    Action = HotkeyAction.Increment,
                    Modifiers = modifiers,
                    VirtualKey = vk
                };
                IncrementKeyTextBox.Text = displayText;
                _isWaitingForIncrementKey = false;
            }
            else if (_isWaitingForDecrementKey)
            {
                _decrementHotkey = new HotkeySettings
                {
                    CounterId = Counter?.Id ?? string.Empty,
                    Action = HotkeyAction.Decrement,
                    Modifiers = modifiers,
                    VirtualKey = vk
                };
                DecrementKeyTextBox.Text = displayText;
                _isWaitingForDecrementKey = false;
            }

            e.Handled = true;
        }

        private void CancelKeyWaiting()
        {
            if (_isWaitingForIncrementKey)
            {
                IncrementKeyTextBox.Text = _incrementHotkey?.GetDisplayText() ?? "未設定";
                _isWaitingForIncrementKey = false;
            }
            else if (_isWaitingForDecrementKey)
            {
                DecrementKeyTextBox.Text = _decrementHotkey?.GetDisplayText() ?? "未設定";
                _isWaitingForDecrementKey = false;
            }
        }

        private string GetKeyDisplayText(uint modifiers, WpfKey key)
        {
            var parts = new List<string>();
            if ((modifiers & 0x0002) != 0) parts.Add("Ctrl");
            if ((modifiers & 0x0004) != 0) parts.Add("Shift");
            if ((modifiers & 0x0001) != 0) parts.Add("Alt");
            parts.Add(key.ToString());
            return string.Join("+", parts);
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

            Hotkeys.Clear();
            if (_incrementHotkey != null)
            {
                _incrementHotkey.CounterId = Counter.Id;
                Hotkeys.Add(_incrementHotkey);
            }
            if (_decrementHotkey != null)
            {
                _decrementHotkey.CounterId = Counter.Id;
                Hotkeys.Add(_decrementHotkey);
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