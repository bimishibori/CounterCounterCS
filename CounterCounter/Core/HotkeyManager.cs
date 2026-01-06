// CounterCounter/Core/HotkeyManager.cs
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using CounterCounter.Models;

namespace CounterCounter.Core
{
    public class HotkeyPressedEventArgs : EventArgs
    {
        public string CounterId { get; }
        public HotkeyAction Action { get; }

        public HotkeyPressedEventArgs(string counterId, HotkeyAction action)
        {
            CounterId = counterId;
            Action = action;
        }
    }

    public class HotkeyManager : IDisposable
    {
        private const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private IntPtr _hwnd;
        private HwndSource? _source;
        private int _nextHotkeyId = 1;
        private readonly Dictionary<int, HotkeyRegistration> _registrations;

        public event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

        private class HotkeyRegistration
        {
            public int Id { get; set; }
            public string CounterId { get; set; }
            public HotkeyAction Action { get; set; }
            public uint Modifiers { get; set; }
            public uint VirtualKey { get; set; }

            public HotkeyRegistration(int id, string counterId, HotkeyAction action, uint modifiers, uint vk)
            {
                Id = id;
                CounterId = counterId;
                Action = action;
                Modifiers = modifiers;
                VirtualKey = vk;
            }
        }

        public HotkeyManager()
        {
            _registrations = new Dictionary<int, HotkeyRegistration>();
        }

        public void Initialize(IntPtr hwnd)
        {
            _hwnd = hwnd;
            _source = HwndSource.FromHwnd(_hwnd);
            if (_source != null)
            {
                _source.AddHook(HwndHook);
            }
        }

        public bool RegisterHotkey(string counterId, HotkeyAction action, uint modifiers, uint vk)
        {
            if (_hwnd == IntPtr.Zero)
            {
                return false;
            }

            if (IsHotkeyAlreadyRegistered(modifiers, vk))
            {
                return false;
            }

            int id = _nextHotkeyId++;
            bool success = RegisterHotKey(_hwnd, id, modifiers, vk);

            if (success)
            {
                _registrations[id] = new HotkeyRegistration(id, counterId, action, modifiers, vk);
            }

            return success;
        }

        public void UnregisterAllHotkeys()
        {
            if (_hwnd == IntPtr.Zero)
            {
                return;
            }

            foreach (int id in _registrations.Keys)
            {
                UnregisterHotKey(_hwnd, id);
            }

            _registrations.Clear();
            _nextHotkeyId = 1;
        }

        public bool IsHotkeyAlreadyRegistered(uint modifiers, uint vk)
        {
            foreach (var reg in _registrations.Values)
            {
                if (reg.Modifiers == modifiers && reg.VirtualKey == vk)
                {
                    return true;
                }
            }
            return false;
        }

        public List<HotkeySettings> GetAllRegisteredHotkeys()
        {
            var result = new List<HotkeySettings>();
            foreach (var reg in _registrations.Values)
            {
                result.Add(new HotkeySettings(
                    reg.CounterId,
                    reg.Action,
                    reg.Modifiers,
                    reg.VirtualKey));
            }
            return result;
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (_registrations.TryGetValue(id, out var reg))
                {
                    HotkeyPressed?.Invoke(this, new HotkeyPressedEventArgs(reg.CounterId, reg.Action));
                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        public void Dispose()
        {
            UnregisterAllHotkeys();
            if (_source != null)
            {
                _source.RemoveHook(HwndHook);
                _source = null;
            }
        }
    }
}