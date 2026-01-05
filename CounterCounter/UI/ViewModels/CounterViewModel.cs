// CounterCounter/UI/ViewModels/CounterViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CounterCounter.Core;
using CounterCounter.Models;
using WpfBrush = System.Windows.Media.Brush;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfColor = System.Windows.Media.Color;

namespace CounterCounter.UI.ViewModels
{
    public class CounterViewModel : INotifyPropertyChanged
    {
        public Counter Counter { get; }
        public CounterManager CounterManager { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public CounterViewModel(Counter counter, CounterManager counterManager)
        {
            Counter = counter;
            CounterManager = counterManager;
        }

        public string Name => Counter.Name;
        public int Value => Counter.Value;
        public string Color => Counter.Color;

        public WpfBrush ColorBrush
        {
            get
            {
                var color = (WpfColor)WpfColorConverter.ConvertFromString(Counter.Color);
                return new WpfSolidColorBrush(color);
            }
        }

        public void Increment()
        {
            CounterManager.Increment(Counter.Id);
        }

        public void Decrement()
        {
            CounterManager.Decrement(Counter.Id);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}