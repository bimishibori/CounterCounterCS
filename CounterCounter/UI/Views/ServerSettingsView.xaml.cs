// CounterCounter/UI/Views/ServerSettingsView.xaml.cs
using System.Windows;
using System.Windows.Controls;
using CounterCounter.Models;
using ComboBox = System.Windows.Controls.ComboBox;
using WpfUserControl = System.Windows.Controls.UserControl;

namespace CounterCounter.UI.Views
{
    public partial class ServerSettingsView : WpfUserControl
    {
        private readonly CounterSettings _settings;
        private bool _isServerRunning;

        public event EventHandler? SettingsChanged;

        public ServerSettingsView(CounterSettings settings, bool isServerRunning)
        {
            InitializeComponent();
            _settings = settings;
            _isServerRunning = isServerRunning;
            InitializeRotationHotkeyComboBoxes();
            UpdateUI();
        }

        private void InitializeRotationHotkeyComboBoxes()
        {
            InitializeKeyComboBox(RotationKey1);
            InitializeKeyComboBox(RotationKey2);
            InitializeKeyComboBox(RotationKey3);

            if (_settings.NextRotationHotkey != null)
            {
                var keys = ParseHotkeyToKeys(
                    _settings.NextRotationHotkey.Modifiers,
                    _settings.NextRotationHotkey.VirtualKey);

                SetComboBoxByTag(RotationKey1, keys.Length > 0 ? keys[0] : 0);
                SetComboBoxByTag(RotationKey2, keys.Length > 1 ? keys[1] : 0);
                SetComboBoxByTag(RotationKey3, keys.Length > 2 ? keys[2] : 0);
            }
        }

        private void InitializeKeyComboBox(ComboBox comboBox)
        {
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
        }

        private void AddKeyItem(ComboBox comboBox, string text, uint keyCode)
        {
            comboBox.Items.Add(new ComboBoxItem { Content = text, Tag = keyCode });
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

        private void SetComboBoxByTag(ComboBox comboBox, uint tag)
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

        private void UpdateUI()
        {
            PortTextBox.Text = _settings.ServerPort.ToString();
            PortTextBox.IsEnabled = !_isServerRunning;
            PortWarningText.Visibility = _isServerRunning ? Visibility.Visible : Visibility.Collapsed;

            SlideInTextBox.Text = _settings.SlideInIntervalMs.ToString();
            SlideInTextBox.IsEnabled = !_isServerRunning;
            SlideInWarningText.Visibility = _isServerRunning ? Visibility.Visible : Visibility.Collapsed;

            RotationTextBox.Text = _settings.RotationIntervalMs.ToString();
            RotationTextBox.IsEnabled = !_isServerRunning;
            RotationWarningText.Visibility = _isServerRunning ? Visibility.Visible : Visibility.Collapsed;

            RotationKey1.IsEnabled = !_isServerRunning;
            RotationKey2.IsEnabled = !_isServerRunning;
            RotationKey3.IsEnabled = !_isServerRunning;
        }

        private void PortTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(PortTextBox.Text, out int port) && port >= 1024 && port <= 65535)
            {
                _settings.ServerPort = port;
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SlideInTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(SlideInTextBox.Text, out int interval) && interval >= 1000)
            {
                _settings.SlideInIntervalMs = interval;
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void RotationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(RotationTextBox.Text, out int interval) && interval >= 1000)
            {
                _settings.RotationIntervalMs = interval;
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void RotationHotkey_Changed(object sender, SelectionChangedEventArgs e)
        {
            var keys = ExtractKeysFromComboBoxes();
            if (keys.Count == 0)
            {
                _settings.NextRotationHotkey = null;
            }
            else
            {
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

                if (modifiers != 0 && virtualKey != 0)
                {
                    _settings.NextRotationHotkey = new HotkeySettings(
                        "", HotkeyAction.NextRotation, modifiers, virtualKey);
                }
                else
                {
                    _settings.NextRotationHotkey = null;
                }
            }

            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private List<uint> ExtractKeysFromComboBoxes()
        {
            var keys = new List<uint>();

            if (RotationKey1.SelectedItem is ComboBoxItem item1 && (uint)item1.Tag != 0)
                keys.Add((uint)item1.Tag);
            if (RotationKey2.SelectedItem is ComboBoxItem item2 && (uint)item2.Tag != 0)
                keys.Add((uint)item2.Tag);
            if (RotationKey3.SelectedItem is ComboBoxItem item3 && (uint)item3.Tag != 0)
                keys.Add((uint)item3.Tag);

            return keys;
        }

        public void UpdateServerStatus(bool isRunning)
        {
            _isServerRunning = isRunning;
            UpdateUI();
        }

        public int GetHttpPort()
        {
            return int.TryParse(PortTextBox.Text, out int port) ? port : 9000;
        }

        public int GetSlideInInterval()
        {
            return int.TryParse(SlideInTextBox.Text, out int interval) ? interval : 5000;
        }

        public int GetRotationInterval()
        {
            return int.TryParse(RotationTextBox.Text, out int interval) ? interval : 5000;
        }

        public HotkeySettings? GetNextRotationHotkey()
        {
            return _settings.NextRotationHotkey;
        }
    }
}