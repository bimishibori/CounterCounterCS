// CounterCounter/UI/Components/CounterCard.xaml.cs
using System.Windows;
using WpfUserControl = System.Windows.Controls.UserControl;

namespace CounterCounter.UI.Components
{
    public partial class CounterCard : WpfUserControl
    {
        public static readonly DependencyProperty CounterIdProperty =
            DependencyProperty.Register(
                nameof(CounterId),
                typeof(string),
                typeof(CounterCard),
                new PropertyMetadata(string.Empty)
            );

        public string CounterId
        {
            get => (string)GetValue(CounterIdProperty);
            set => SetValue(CounterIdProperty, value);
        }

        public event EventHandler<string>? IncrementRequested;
        public event EventHandler<string>? DecrementRequested;
        public event EventHandler<string>? ResetRequested;
        public event EventHandler<string>? EditRequested;
        public event EventHandler<string>? DeleteRequested;

        public CounterCard()
        {
            InitializeComponent();
        }

        private void IncrementButton_Click(object sender, RoutedEventArgs e)
        {
            IncrementRequested?.Invoke(this, CounterId);
        }

        private void DecrementButton_Click(object sender, RoutedEventArgs e)
        {
            DecrementRequested?.Invoke(this, CounterId);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetRequested?.Invoke(this, CounterId);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditRequested?.Invoke(this, CounterId);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteRequested?.Invoke(this, CounterId);
        }
    }
}