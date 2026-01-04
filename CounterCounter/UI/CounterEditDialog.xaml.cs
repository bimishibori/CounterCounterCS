// CounterCounter/UI/CounterEditDialog.xaml.cs
using System.Windows;
using CounterCounter.Models;
using WpfMessageBox = System.Windows.MessageBox;
using WpfButton = System.Windows.Controls.Button;

namespace CounterCounter.UI
{
    public partial class CounterEditDialog : Window
    {
        public string CounterName => NameTextBox.Text;
        public string CounterColor { get; private set; } = "#00ff00";

        public CounterEditDialog(Counter? counter)
        {
            InitializeComponent();

            if (counter != null)
            {
                NameTextBox.Text = counter.Name;
                CounterColor = counter.Color;
            }
            else
            {
                NameTextBox.Text = "New Counter";
            }
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is WpfButton button && button.Tag is string color)
            {
                CounterColor = color;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                WpfMessageBox.Show("カウンター名を入力してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}