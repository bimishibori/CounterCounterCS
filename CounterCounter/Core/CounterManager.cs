// CounterCounter/Core/CounterManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using CounterCounter.Models;

namespace CounterCounter.Core
{
    public class CounterChangeEventArgs : EventArgs
    {
        public string CounterId { get; }
        public int NewValue { get; }
        public int OldValue { get; }
        public HotkeyAction ChangeType { get; }

        public CounterChangeEventArgs(string counterId, int newValue, int oldValue, HotkeyAction changeType)
        {
            CounterId = counterId;
            NewValue = newValue;
            OldValue = oldValue;
            ChangeType = changeType;
        }
    }

    public class CounterManager
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, Counter> _counters;

        public event EventHandler<CounterChangeEventArgs>? CounterChanged;

        public CounterManager()
        {
            _counters = new Dictionary<string, Counter>();
        }

        public void AddCounter(Counter counter)
        {
            lock (_lock)
            {
                if (_counters.ContainsKey(counter.Id))
                {
                    throw new InvalidOperationException($"Counter with ID '{counter.Id}' already exists.");
                }
                _counters[counter.Id] = counter.Clone();
            }
        }

        public void RemoveCounter(string counterId)
        {
            lock (_lock)
            {
                _counters.Remove(counterId);
            }
        }

        public Counter? GetCounter(string counterId)
        {
            lock (_lock)
            {
                return _counters.TryGetValue(counterId, out var counter) ? counter.Clone() : null;
            }
        }

        public List<Counter> GetAllCounters()
        {
            lock (_lock)
            {
                return _counters.Values.Select(c => c.Clone()).ToList();
            }
        }

        public void UpdateCounter(string counterId, string name, string color)
        {
            lock (_lock)
            {
                if (_counters.TryGetValue(counterId, out var counter))
                {
                    counter.Name = name;
                    counter.Color = color;
                }
            }
        }

        public void Increment(string counterId)
        {
            lock (_lock)
            {
                if (_counters.TryGetValue(counterId, out var counter))
                {
                    int oldValue = counter.Value;
                    counter.Value++;
                    CounterChanged?.Invoke(this, new CounterChangeEventArgs(
                        counterId, counter.Value, oldValue, HotkeyAction.Increment));
                }
            }
        }

        public void Decrement(string counterId)
        {
            lock (_lock)
            {
                if (_counters.TryGetValue(counterId, out var counter))
                {
                    int oldValue = counter.Value;
                    counter.Value--;
                    CounterChanged?.Invoke(this, new CounterChangeEventArgs(
                        counterId, counter.Value, oldValue, HotkeyAction.Decrement));
                }
            }
        }

        public void Reset(string counterId)
        {
            lock (_lock)
            {
                if (_counters.TryGetValue(counterId, out var counter))
                {
                    int oldValue = counter.Value;
                    counter.Value = 0;
                    CounterChanged?.Invoke(this, new CounterChangeEventArgs(
                        counterId, counter.Value, oldValue, HotkeyAction.Reset));
                }
            }
        }

        public void SetValue(string counterId, int value)
        {
            lock (_lock)
            {
                if (_counters.TryGetValue(counterId, out var counter))
                {
                    int oldValue = counter.Value;
                    counter.Value = value;
                    CounterChanged?.Invoke(this, new CounterChangeEventArgs(
                        counterId, counter.Value, oldValue, HotkeyAction.Reset));
                }
            }
        }

        public void LoadCounters(List<Counter> counters)
        {
            lock (_lock)
            {
                _counters.Clear();
                foreach (var counter in counters)
                {
                    _counters[counter.Id] = counter.Clone();
                }
            }
        }
    }
}