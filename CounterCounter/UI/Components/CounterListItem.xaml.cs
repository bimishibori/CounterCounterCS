// CounterCounter/UI/Components/CounterListItem.xaml.cs
using System.Windows;
using CounterCounter.UI.ViewModels;
using CounterCounter.UI.Dialogs;
using WpfUserControl = System.Windows.Controls.UserControl;
using WpfMessageBox = System.Windows.MessageBox;

namespace CounterCounter.UI.Components
{
    public partial class CounterListItem : WpfUserControl
    {
        public CounterListItem()
        {
            InitializeComponent();
        }

        private CounterViewModel? ViewModel => DataContext as CounterViewModel;

        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.Increment();
        }

        private void Decrement_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.Decrement();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;

            var dialog = new CounterEditDialog(ViewModel.Counter);
            if (dialog.ShowDialog() == true && dialog.Counter != null)
            {
                ViewModel.CounterManager.UpdateCounter(dialog.Counter.Id, dialog.Counter.Name, dialog.Counter.Color);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;

            var result = WpfMessageBox.Show(
                $"カウンター「{ViewModel.Name}」を削除しますか？",
                "削除確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                ViewModel.CounterManager.RemoveCounter(ViewModel.Counter.Id);
            }
        }
    }
}