// CounterCounter/UI/Views/CounterManagementView.xaml.cs
using System.Windows;
using CounterCounter.Core;
using CounterCounter.Models;
using WpfButton = System.Windows.Controls.Button;
using WpfMessageBox = System.Windows.MessageBox;
using WpfUserControl = System.Windows.Controls.UserControl;
using WpfSelectionChangedEventArgs = System.Windows.Controls.SelectionChangedEventArgs;

namespace CounterCounter.UI.Views
{
    public partial class CounterManagementView : WpfUserControl
    {
        private readonly CounterManager _counterManager;

        public CounterManagementView(CounterManager counterManager)
        {
            InitializeComponent();
            _counterManager = counterManager;
            _counterManager.CounterChanged += OnCounterChanged;
            RefreshCounterList();
        }

        private void OnCounterChanged(object? sender, CounterChangeEventArgs e)
        {
            Dispatcher.Invoke(() => RefreshCounterList());
        }

        public void RefreshCounterList()
        {
            var counters = _counterManager.GetAllCounters();
            CounterListBox.ItemsSource = null;
            CounterListBox.ItemsSource = counters;
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

        private void CounterListBox_SelectionChanged(object sender, WpfSelectionChangedEventArgs e)
        {
        }

        private void IncrementCounter_Click(object sender, RoutedEventArgs e)
        {
            if (CounterListBox.SelectedItem is Counter counter)
            {
                _counterManager.Increment(counter.Id);
            }
        }

        private void DecrementCounter_Click(object sender, RoutedEventArgs e)
        {
            if (CounterListBox.SelectedItem is Counter counter)
            {
                _counterManager.Decrement(counter.Id);
            }
        }

        private void ResetCounter_Click(object sender, RoutedEventArgs e)
        {
            if (CounterListBox.SelectedItem is Counter counter)
            {
                _counterManager.Reset(counter.Id);
            }
        }
    }
}