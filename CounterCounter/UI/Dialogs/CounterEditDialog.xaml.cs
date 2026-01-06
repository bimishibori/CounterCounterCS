using System.Windows;
using System.Windows.Controls;
using CounterCounter.Models;
using WpfMessageBox = System.Windows.MessageBox;
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;
using WinForms = System.Windows.Forms;
using WpfComboBox = System.Windows.Controls.ComboBox;
using WpfStackPanel = System.Windows.Controls.StackPanel;
using WpfTextBlock = System.Windows.Controls.TextBlock;
using WpfOrientation = System.Windows.Controls.Orientation;

namespace CounterCounter.UI.Dialogs
{
    public partial class CounterEditDialog : Window
    {
        private const int MAX_HOTKEY_SLOTS = 1;

        private string _selectedColor = "#51cf66";
        private readonly List<HotkeySlot> _incrementSlots = new();
        private readonly List<HotkeySlot> _decrementSlots = new();
        private readonly List<HotkeySettings> _existingHotkeys;

        public Counter? Counter { get; private set; }
        public string CounterName => NameTextBox.Text.Trim();
        public string CounterColor => _selectedColor;

        public List<HotkeySettings> IncrementHotkeys { get; private set; } = new();
        public List<HotkeySettings> DecrementHotkeys { get; private set; } = new();

        public CounterEditDialog(List<HotkeySettings> existingHotkeys)
        {
            InitializeComponent();
            Title = "新規カウンター追加";
            _existingHotkeys = existingHotkeys;
            InitializeHotkeySlots();
            UpdateColorPreview();
        }

        public CounterEditDialog(Counter counter, List<HotkeySettings> existingHotkeys) : this(existingHotkeys)
        {
            Title = "カウンター編集";
            Counter = counter;
            NameTextBox.Text = counter.Name;
            _selectedColor = counter.Color;
            UpdateColorPreview();

            var incrementHotkeys = existingHotkeys.Where(h =>
                h.CounterId == counter.Id && h.Action == HotkeyAction.Increment).ToList();
            var decrementHotkeys = existingHotkeys.Where(h =>
                h.CounterId == counter.Id && h.Action == HotkeyAction.Decrement).ToList();

            LoadExistingHotkeys(_incrementSlots, incrementHotkeys);
            LoadExistingHotkeys(_decrementSlots, decrementHotkeys);
        }

        private void InitializeHotkeySlots()
        {
            for (int i = 0; i < MAX_HOTKEY_SLOTS; i++)
            {
                _incrementSlots.Add(CreateHotkeySlot(IncrementHotkeySlotsPanel, i + 1));
                _decrementSlots.Add(CreateHotkeySlot(DecrementHotkeySlotsPanel, i + 1));
            }
        }

        private HotkeySlot CreateHotkeySlot(WpfStackPanel parentPanel, int slotNumber)
        {
            var slotPanel = new WpfStackPanel
            {
                Orientation = WpfOrientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 5)
            };

            var label = new WpfTextBlock
            {
                Text = $"#{slotNumber}:",
                Width = 35,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new WpfSolidColorBrush((WpfColor)WpfColorConverter.ConvertFromString("#b0b0b0"))
            };

            var key1ComboBox = CreateKeyComboBox();
            var key2ComboBox = CreateKeyComboBox();
            var key3ComboBox = CreateKeyComboBox();

            slotPanel.Children.Add(label);
            slotPanel.Children.Add(key1ComboBox);
            slotPanel.Children.Add(key2ComboBox);
            slotPanel.Children.Add(key3ComboBox);

            parentPanel.Children.Add(slotPanel);

            return new HotkeySlot
            {
                Key1 = key1ComboBox,
                Key2 = key2ComboBox,
                Key3 = key3ComboBox
            };
        }

        private WpfComboBox CreateKeyComboBox()
        {
            var comboBox = new WpfComboBox
            {
                Width = 110,
                Margin = new Thickness(5, 0, 5, 0)
            };

            comboBox.Items.Add(new ComboBoxItem { Content = "未選択", Tag = (uint)0 });

            AddKeyItem(comboBox, "Ctrl", 0x0002);
            AddKeyItem(comboBox, "Shift", 0x0004);
            AddKeyItem(comboBox, "Alt", 0x0001);

            AddKeyItem(comboBox, "↑", 0x26);
            AddKeyItem(comboBox, "↓", 0x28);
            AddKeyItem(comboBox, "←", 0x25);
            AddKeyItem(comboBox, "→", 0x27);

            for (int i = 1; i <= 12; i++)
            {
                AddKeyItem(comboBox, $"F{i}", (uint)(0x70 + i - 1));
            }

            for (char c = 'A'; c <= 'Z'; c++)
            {
                AddKeyItem(comboBox, c.ToString(), (uint)c);
            }

            for (int i = 0; i <= 9; i++)
            {
                AddKeyItem(comboBox, i.ToString(), (uint)(0x30 + i));
            }

            comboBox.SelectedIndex = 0;
            return comboBox;
        }

        private void AddKeyItem(WpfComboBox comboBox, string text, uint keyCode)
        {
            comboBox.Items.Add(new ComboBoxItem { Content = text, Tag = keyCode });
        }

        private void LoadExistingHotkeys(List<HotkeySlot> slots, List<HotkeySettings> hotkeys)
        {
            for (int i = 0; i < Math.Min(hotkeys.Count, slots.Count); i++)
            {
                var hotkey = hotkeys[i];
                var keys = ParseHotkeyToKeys(hotkey.Modifiers, hotkey.VirtualKey);

                SetComboBoxByTag(slots[i].Key1, keys.Length > 0 ? keys[0] : 0);
                SetComboBoxByTag(slots[i].Key2, keys.Length > 1 ? keys[1] : 0);
                SetComboBoxByTag(slots[i].Key3, keys.Length > 2 ? keys[2] : 0);
            }
        }

        private uint[] ParseHotkeyToKeys(uint modifiers, uint virtualKey)
        {
            var keys = new List<uint>();

            if ((modifiers & 0x0002) != 0) keys.Add(0x0002);
            if ((modifiers & 0x0004) != 0) keys.Add(0x0004);
            if ((modifiers & 0x0001) != 0) keys.Add(0x0001);

            if (virtualKey != 0) keys.Add(virtualKey);

            return keys.ToArray();
        }

        private void SetComboBoxByTag(WpfComboBox comboBox, uint tag)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i] is ComboBoxItem item && (uint)item.Tag == tag)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }
            comboBox.SelectedIndex = 0;
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

            IncrementHotkeys = BuildHotkeysFromSlots(_incrementSlots, HotkeyAction.Increment);
            DecrementHotkeys = BuildHotkeysFromSlots(_decrementSlots, HotkeyAction.Decrement);

            if (!ValidateHotkeys())
            {
                return;
            }

            DialogResult = true;
            Close();
        }

        private List<HotkeySettings> BuildHotkeysFromSlots(List<HotkeySlot> slots, HotkeyAction action)
        {
            var hotkeys = new List<HotkeySettings>();

            foreach (var slot in slots)
            {
                var keys = ExtractKeysFromSlot(slot);
                if (keys.Count == 0) continue;

                uint modifiers = 0;
                uint virtualKey = 0;

                foreach (var key in keys)
                {
                    if (key == 0x0002 || key == 0x0004 || key == 0x0001)
                    {
                        modifiers |= key;
                    }
                    else
                    {
                        virtualKey = key;
                    }
                }

                if (modifiers == 0 || virtualKey == 0)
                {
                    continue;
                }

                hotkeys.Add(new HotkeySettings(Counter!.Id, action, modifiers, virtualKey));
            }

            return hotkeys;
        }

        private List<uint> ExtractKeysFromSlot(HotkeySlot slot)
        {
            var keys = new List<uint>();

            if (slot.Key1.SelectedItem is ComboBoxItem item1 && (uint)item1.Tag != 0)
                keys.Add((uint)item1.Tag);
            if (slot.Key2.SelectedItem is ComboBoxItem item2 && (uint)item2.Tag != 0)
                keys.Add((uint)item2.Tag);
            if (slot.Key3.SelectedItem is ComboBoxItem item3 && (uint)item3.Tag != 0)
                keys.Add((uint)item3.Tag);

            return keys;
        }

        private bool ValidateHotkeys()
        {
            var allHotkeys = IncrementHotkeys.Concat(DecrementHotkeys).ToList();

            foreach (var hotkey in allHotkeys)
            {
                var conflicting = _existingHotkeys.FirstOrDefault(h =>
                    h.CounterId != Counter!.Id &&
                    h.Modifiers == hotkey.Modifiers &&
                    h.VirtualKey == hotkey.VirtualKey);

                if (conflicting != null)
                {
                    var hotkeyDisplay = new HotkeySettings("", HotkeyAction.Increment,
                        hotkey.Modifiers, hotkey.VirtualKey).GetDisplayText();
                    WpfMessageBox.Show(
                        $"ホットキー「{hotkeyDisplay}」は他のカウンターで既に使用されています。",
                        "エラー",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private class HotkeySlot
        {
            public WpfComboBox Key1 { get; set; } = null!;
            public WpfComboBox Key2 { get; set; } = null!;
            public WpfComboBox Key3 { get; set; } = null!;
        }
    }
}