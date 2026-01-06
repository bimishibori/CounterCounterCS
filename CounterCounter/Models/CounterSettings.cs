// CounterCounter/Models/CounterSettings.cs
using System.Collections.Generic;

namespace CounterCounter.Models
{
    public class CounterSettings
    {
        public List<Counter> Counters { get; set; }
        public List<HotkeySettings> Hotkeys { get; set; }
        public int ServerPort { get; set; }

        public CounterSettings()
        {
            Counters = new List<Counter>();
            Hotkeys = new List<HotkeySettings>();
            ServerPort = 9000;
        }

        public static CounterSettings CreateDefault()
        {
            var settings = new CounterSettings();

            var defaultCounter = new Counter
            {
                Id = "default",
                Name = "Default Counter",
                Value = 0,
                Color = "#00ff00"
            };
            settings.Counters.Add(defaultCounter);

            settings.Hotkeys.Add(new HotkeySettings(
                "default", HotkeyAction.Increment, 0x0002 | 0x0004, 0x26));
            settings.Hotkeys.Add(new HotkeySettings(
                "default", HotkeyAction.Decrement, 0x0002 | 0x0004, 0x28));
            settings.Hotkeys.Add(new HotkeySettings(
                "default", HotkeyAction.Reset, 0x0002 | 0x0004, 0x52));

            return settings;
        }
    }
}