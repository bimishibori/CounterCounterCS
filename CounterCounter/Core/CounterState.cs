// CounterCounter/Core/CounterState.cs
using System;

namespace CounterCounter.Core
{
    public class CounterState
    {
        private int _value;
        private readonly object _lock = new object();

        public event EventHandler<CounterChangedEventArgs>? ValueChanged;

        public int InitialValue { get; set; } = 0;

        public int Value
        {
            get
            {
                lock (_lock)
                {
                    return _value;
                }
            }
            private set
            {
                lock (_lock)
                {
                    int oldValue = _value;
                    _value = value;
                    OnValueChanged(oldValue, _value);
                }
            }
        }

        public CounterState()
        {
            _value = InitialValue;
        }

        public void Increment()
        {
            Value++;
        }

        public void Decrement()
        {
            Value--;
        }

        public void Reset()
        {
            Value = InitialValue;
        }

        public void SetValue(int value)
        {
            Value = value;
        }

        public int GetValue()
        {
            return Value;
        }

        protected virtual void OnValueChanged(int oldValue, int newValue)
        {
            ValueChanged?.Invoke(this, new CounterChangedEventArgs
            {
                OldValue = oldValue,
                NewValue = newValue,
                ChangeType = newValue > oldValue ? ChangeType.Increment :
                            newValue < oldValue ? ChangeType.Decrement :
                            ChangeType.Reset
            });
        }
    }

    public class CounterChangedEventArgs : EventArgs
    {
        public int OldValue { get; set; }
        public int NewValue { get; set; }
        public ChangeType ChangeType { get; set; }
    }

    public enum ChangeType
    {
        Increment,
        Decrement,
        Reset
    }
}