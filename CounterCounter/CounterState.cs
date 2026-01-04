using System;

namespace CounterCounter
{
    public class CounterState
    {
        private int _value;
        private readonly object _lock = new object();

        // カウンター値が変更されたときのイベント
        public event EventHandler<CounterChangedEventArgs>? ValueChanged;

        // 初期値
        public int InitialValue { get; set; } = 0;

        // 現在のカウンター値
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

        // カウンター増加
        public void Increment()
        {
            Value++;
        }

        // カウンター減少
        public void Decrement()
        {
            Value--;
        }

        // リセット
        public void Reset()
        {
            Value = InitialValue;
        }

        // 値を直接設定
        public void SetValue(int value)
        {
            Value = value;
        }

        // 現在の値を取得
        public int GetValue()
        {
            return Value;
        }

        // イベント発火
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

    // イベント引数
    public class CounterChangedEventArgs : EventArgs
    {
        public int OldValue { get; set; }
        public int NewValue { get; set; }
        public ChangeType ChangeType { get; set; }
    }

    // 変更タイプ
    public enum ChangeType
    {
        Increment,  // 増加
        Decrement,  // 減少
        Reset       // リセット
    }
}