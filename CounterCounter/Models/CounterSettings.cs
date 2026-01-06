// CounterCounter/Models/CounterSettings.cs
using System.Collections.Generic;

namespace CounterCounter.Models
{
    public class CounterSettings
    {
        public List<Counter> Counters { get; set; }
        public List<HotkeySettings> Hotkeys { get; set; }
        public int ServerPort { get; set; }
        public int SlideInIntervalMs { get; set; }
        public int RotationIntervalMs { get; set; }
        public HotkeySettings? NextRotationHotkey { get; set; }

        public CounterSettings()
        {
            Counters = new List<Counter>();
            Hotkeys = new List<HotkeySettings>();
            ServerPort = 9000;
            SlideInIntervalMs = 3000;
            RotationIntervalMs = 5000;
            NextRotationHotkey = null;
        }

        public static CounterSettings CreateDefault()
        {
            var settings = new CounterSettings();

            var defaultCounter = new Counter
            {
                Id = "default",
                Name = "Default Counter",
                Value = 0,
                Color = "#00ff00",
                ShowInRotation = true
            };
            settings.Counters.Add(defaultCounter);

            settings.Hotkeys.Add(new HotkeySettings(
                "default", HotkeyAction.Increment, 0x0002 | 0x0004, 0x26));
            settings.Hotkeys.Add(new HotkeySettings(
                "default", HotkeyAction.Decrement, 0x0002 | 0x0004, 0x28));
            settings.Hotkeys.Add(new HotkeySettings(
                "default", HotkeyAction.Reset, 0x0002 | 0x0004, 0x52));

            settings.NextRotationHotkey = new HotkeySettings(
                "", HotkeyAction.NextRotation, 0x0002 | 0x0004, 0x4E);

            return settings;
        }
    }
}