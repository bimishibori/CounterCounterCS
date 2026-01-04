// CounterCounter/Core/CounterManager.cs
using System.Collections.Concurrent;
using CounterCounter.Models;

namespace CounterCounter.Core
{
    public class CounterManager
    {
        private readonly ConcurrentDictionary<string, Counter> _counters = new();
        private readonly object _lock = new object();

        public event EventHandler<CounterChangedEventArgs>? CounterChanged;

        public void LoadCounters(List<Counter> counters)
        {
            lock (_lock)
            {
                _counters.Clear();
                foreach (var counter in counters)
                {
                    _counters[counter.Id] = counter;
                }
            }
        }

        public void AddCounter(Counter counter)
        {
            lock (_lock)
            {
                _counters[counter.Id] = counter;
                CounterChanged?.Invoke(this, new CounterChangedEventArgs(counter, counter.Value, counter.Value, "add"));
            }
        }

        public void RemoveCounter(string id)
        {
            lock (_lock)
            {
                if (_counters.TryRemove(id, out var counter))
                {
                    CounterChanged?.Invoke(this, new CounterChangedEventArgs(counter, counter.Value, counter.Value, "remove"));
                }
            }
        }

        public Counter? GetCounter(string id)
        {
            return _counters.TryGetValue(id, out var counter) ? counter : null;
        }

        public List<Counter> GetAllCounters()
        {
            lock (_lock)
            {
                return _counters.Values.Select(c => c.Clone()).ToList();
            }
        }

        public void UpdateCounter(string id, string name, string color)
        {
            lock (_lock)
            {
                if (_counters.TryGetValue(id, out var counter))
                {
                    counter.Name = name;
                    counter.Color = color;
                    CounterChanged?.Invoke(this, new CounterChangedEventArgs(counter, counter.Value, counter.Value, "update"));
                }
            }
        }

        public void Increment(string id)
        {
            lock (_lock)
            {
                if (_counters.TryGetValue(id, out var counter))
                {
                    int oldValue = counter.Value;
                    counter.Value++;
                    CounterChanged?.Invoke(this, new CounterChangedEventArgs(counter, oldValue, counter.Value, "increment"));
                }
            }
        }

        public void Decrement(string id)
        {
            lock (_lock)
            {
                if (_counters.TryGetValue(id, out var counter))
                {
                    int oldValue = counter.Value;
                    counter.Value--;
                    CounterChanged?.Invoke(this, new CounterChangedEventArgs(counter, oldValue, counter.Value, "decrement"));
                }
            }
        }

        public void Reset(string id)
        {
            lock (_lock)
            {
                if (_counters.TryGetValue(id, out var counter))
                {
                    int oldValue = counter.Value;
                    counter.Value = 0;
                    CounterChanged?.Invoke(this, new CounterChangedEventArgs(counter, oldValue, 0, "reset"));
                }
            }
        }

        public void SetValue(string id, int value)
        {
            lock (_lock)
            {
                if (_counters.TryGetValue(id, out var counter))
                {
                    int oldValue = counter.Value;
                    counter.Value = value;
                    CounterChanged?.Invoke(this, new CounterChangedEventArgs(counter, oldValue, value, "set"));
                }
            }
        }
    }

    public class CounterChangedEventArgs : EventArgs
    {
        public Counter Counter { get; }
        public int OldValue { get; }
        public int NewValue { get; }
        public string ChangeType { get; }

        public CounterChangedEventArgs(Counter counter, int oldValue, int newValue, string changeType)
        {
            Counter = counter;
            OldValue = oldValue;
            NewValue = newValue;
            ChangeType = changeType;
        }
    }
}