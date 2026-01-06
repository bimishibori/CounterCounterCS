// CounterCounter/Models/Counter.cs
using System;

namespace CounterCounter.Models
{
    public class Counter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public string Color { get; set; }
        public bool ShowInRotation { get; set; }

        public Counter()
        {
            Id = Guid.NewGuid().ToString();
            Name = "New Counter";
            Value = 0;
            Color = "#ffffff";
            ShowInRotation = true;
        }

        public Counter(string id, string name, int value, string color, bool showInRotation = true)
        {
            Id = id;
            Name = name;
            Value = value;
            Color = color;
            ShowInRotation = showInRotation;
        }

        public Counter Clone()
        {
            return new Counter(Id, Name, Value, Color, ShowInRotation);
        }
    }

    public class HotkeySettings
    {
        public string CounterId { get; set; }
        public HotkeyAction Action { get; set; }
        public uint Modifiers { get; set; }
        public uint VirtualKey { get; set; }

        public HotkeySettings()
        {
            CounterId = string.Empty;
            Action = HotkeyAction.Increment;
            Modifiers = 0;
            VirtualKey = 0;
        }

        public HotkeySettings(string counterId, HotkeyAction action, uint modifiers, uint vk)
        {
            CounterId = counterId;
            Action = action;
            Modifiers = modifiers;
            VirtualKey = vk;
        }

        public string GetDisplayText()
        {
            string mod = string.Empty;
            if ((Modifiers & 0x0002) != 0) mod += "Ctrl+";
            if ((Modifiers & 0x0004) != 0) mod += "Shift+";
            if ((Modifiers & 0x0001) != 0) mod += "Alt+";

            string key = GetKeyName(VirtualKey);
            return mod + key;
        }

        private string GetKeyName(uint vk)
        {
            return vk switch
            {
                0x26 => "↑",
                0x28 => "↓",
                0x25 => "←",
                0x27 => "→",
                >= 0x30 and <= 0x39 => ((char)vk).ToString(),
                >= 0x41 and <= 0x5A => ((char)vk).ToString(),
                >= 0x70 and <= 0x87 => $"F{vk - 0x6F}",
                _ => $"Key{vk:X2}"
            };
        }
    }

    public enum HotkeyAction
    {
        Increment,
        Decrement,
        Reset
    }
}