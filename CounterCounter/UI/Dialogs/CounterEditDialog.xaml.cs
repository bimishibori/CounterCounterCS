// CounterCounter/UI/Dialogs/CounterEditDialog.xaml.cs
using System.Windows;
using CounterCounter.Models;
using WpfMessageBox = System.Windows.MessageBox;
using WpfButton = System.Windows.Controls.Button;

namespace CounterCounter.UI.Dialogs
{
    public partial class CounterEditDialog : Window
    {
        private string _selectedColor = "#51cf66";

        public Counter? Counter { get; private set; }
        public string CounterName => NameTextBox.Text.Trim();
        public string CounterColor => _selectedColor;

        public CounterEditDialog()
        {
            InitializeComponent();
            Title = "新規カウンター追加";
        }

        public CounterEditDialog(Counter counter) : this()
        {
            Title = "カウンター編集";
            Counter = counter;
            NameTextBox.Text = counter.Name;
            _selectedColor = counter.Color;
            UpdateColorSelection();
        }

        public CounterEditDialog(string name, string color) : this()
        {
            Title = "カウンター編集";
            NameTextBox.Text = name;
            _selectedColor = color;
            UpdateColorSelection();
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