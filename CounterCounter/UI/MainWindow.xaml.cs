// CounterCounter/UI/MainWindow.xaml.cs
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using CounterCounter.Core;
using CounterCounter.Models;
using CounterCounter.Server;
using CounterCounter.UI.Dialogs;
using CounterCounter.UI.Views;
using WpfOpenFileDialog = Microsoft.Win32.OpenFileDialog;
using WpfSaveFileDialog = Microsoft.Win32.SaveFileDialog;
using WpfButton = System.Windows.Controls.Button;
using WpfMessageBox = CounterCounter.UI.Dialogs.ModernMessageBox;
using WpfMessageBoxButton = System.Windows.MessageBoxButton;
using WpfMessageBoxImage = System.Windows.MessageBoxImage;
using WpfMessageBoxResult = System.Windows.MessageBoxResult;
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;
using WpfLinearGradientBrush = System.Windows.Media.LinearGradientBrush;
using WpfGradientStop = System.Windows.Media.GradientStop;
using Point = System.Windows.Point;

namespace CounterCounter.UI
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly CounterManager _counterManager;
        private readonly ConfigManager _configManager;
        private CounterSettings _settings;
        private AppSettings _appSettings;
        private WebServer? _webServer;
        private HotkeyManager? _hotkeyManager;
        private IntPtr _hwnd;
        private int _httpPort;
        private bool _isServerRunning;
        private bool _hasUnsavedChanges;
        private bool _hasFilePath;

        private CounterManagementView? _counterManagementView;
        private ServerSettingsView? _serverSettingsView;
        private ConnectionInfoView? _connectionInfoView;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow(CounterManager counterManager, ConfigManager configManager, CounterSettings settings, AppSettings appSettings)
        {
            InitializeComponent();
            DataContext = this;

            _counterManager = counterManager;
            _configManager = configManager;
            _settings = settings;
            _appSettings = appSettings;
            _httpPort = settings.ServerPort;
            _isServerRunning = false;
            _hasUnsavedChanges = false;

            if (string.IsNullOrEmpty(_appSettings.LastOpenedFilePath) ||
                !File.Exists(_appSettings.LastOpenedFilePath))
            {
                _hasFilePath = false;
            }
            else
            {
                _hasFilePath = true;
            }

            UpdateServerToggleButton();
            UpdateServerStatus();
            UpdateCurrentFileDisplay();
            UpdateSaveMenuState();
            ShowCountersView();
        }

        public void SetHwnd(IntPtr hwnd)
        {
            _hwnd = hwnd;
        }

        public void ShowWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var result = WpfMessageBox.Show(
                    "保存されていない変更があります。保存しますか？",
                    "確認",
                    WpfMessageBoxButton.YesNoCancel,
                    WpfMessageBoxImage.Question,
                    this
                );

                if (result == WpfMessageBoxResult.Yes)
                {
                    SaveSettings();
                }
                else if (result == WpfMessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (_isServerRunning)
            {
                e.Cancel = true;
                Hide();

                var app = System.Windows.Application.Current as App;
                app?.ShowTrayNotification(
                    "Counter Counter",
                    "ウィンドウを閉じました。タスクトレイから再表示できます。",
                    3000);
            }
            else
            {
                e.Cancel = false;
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var result = WpfMessageBox.Show(
                    "保存されていない変更があります。保存しますか？",
                    "確認",
                    WpfMessageBoxButton.YesNoCancel,
                    WpfMessageBoxImage.Question,
                    this
                );

                if (result == WpfMessageBoxResult.Yes)
                {
                    SaveSettings();
                }
                else if (result == WpfMessageBoxResult.Cancel)
                {
                    return;
                }
            }

            if (_isServerRunning)
            {
                WpfMessageBox.Show(
                    "サーバー起動中は新規作成できません。\n先にサーバーを停止してください。",
                    "エラー",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Warning,
                    this
                );
                return;
            }

            _settings = CounterSettings.CreateDefault();
            _counterManager.LoadCounters(_settings.Counters);
            _configManager.ResetToDefault();
            _hasUnsavedChanges = false;
            _hasFilePath = false;

            _appSettings.LastOpenedFilePath = null;
            _configManager.SaveAppSettings(_appSettings);

            UpdateCurrentFileDisplay();
            UpdateSaveMenuState();
            RefreshAllViews();

            WpfMessageBox.Show(
                "新規ファイルを作成しました",
                "完了",
                WpfMessageBoxButton.OK,
                WpfMessageBoxImage.Information,
                this
            );
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var result = WpfMessageBox.Show(
                    "保存されていない変更があります。保存しますか？",
                    "確認",
                    WpfMessageBoxButton.YesNoCancel,
                    WpfMessageBoxImage.Question,
                    this
                );

                if (result == WpfMessageBoxResult.Yes)
                {
                    SaveSettings();
                }
                else if (result == WpfMessageBoxResult.Cancel)
                {
                    return;
                }
            }

            if (_isServerRunning)
            {
                WpfMessageBox.Show(
                    "サーバー起動中はファイルを開けません。\n先にサーバーを停止してください。",
                    "エラー",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Warning,
                    this
                );
                return;
            }

            var openFileDialog = new WpfOpenFileDialog
            {
                Title = "設定ファイルを開く",
                Filter = "JSONファイル (*.json)|*.json|すべてのファイル (*.*)|*.*",
                DefaultExt = "json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _settings = _configManager.LoadFromFile(openFileDialog.FileName);
                    _counterManager.LoadCounters(_settings.Counters);
                    _httpPort = _settings.ServerPort;
                    _hasUnsavedChanges = false;
                    _hasFilePath = true;

                    _appSettings.LastOpenedFilePath = openFileDialog.FileName;
                    _configManager.SaveAppSettings(_appSettings);

                    UpdateCurrentFileDisplay();
                    UpdateSaveMenuState();
                    RefreshAllViews();

                    WpfMessageBox.Show(
                        $"ファイルを読み込みました\n{openFileDialog.FileName}",
                        "完了",
                        WpfMessageBoxButton.OK,
                        WpfMessageBoxImage.Information,
                        this
                    );
                }
                catch (Exception ex)
                {
                    WpfMessageBox.Show(
                        $"ファイルの読み込みに失敗しました\n{ex.Message}",
                        "エラー",
                        WpfMessageBoxButton.OK,
                        WpfMessageBoxImage.Error,
                        this
                    );
                }
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (!_hasFilePath)
            {
                SaveAsFile_Click(sender, e);
                return;
            }

            SaveSettings();

            WpfMessageBox.Show(
                $"保存しました\n{_configManager.CurrentConfigPath}",
                "完了",
                WpfMessageBoxButton.OK,
                WpfMessageBoxImage.Information,
                this
            );
        }

        private void SaveAsFile_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new WpfSaveFileDialog
            {
                Title = "名前をつけて保存",
                Filter = "JSONファイル (*.json)|*.json|すべてのファイル (*.*)|*.*",
                DefaultExt = "json",
                FileName = "config.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _settings.Counters = _counterManager.GetAllCounters();
                    bool saved = _configManager.SaveToFile(_settings, saveFileDialog.FileName);

                    if (saved)
                    {
                        _hasUnsavedChanges = false;
                        _hasFilePath = true;

                        _appSettings.LastOpenedFilePath = saveFileDialog.FileName;
                        _configManager.SaveAppSettings(_appSettings);

                        UpdateCurrentFileDisplay();
                        UpdateSaveMenuState();

                        WpfMessageBox.Show(
                            $"保存しました\n{saveFileDialog.FileName}",
                            "完了",
                            WpfMessageBoxButton.OK,
                            WpfMessageBoxImage.Information,
                            this
                        );
                    }
                    else
                    {
                        WpfMessageBox.Show(
                            "ファイルの保存に失敗しました",
                            "エラー",
                            WpfMessageBoxButton.OK,
                            WpfMessageBoxImage.Error,
                            this
                        );
                    }
                }
                catch (Exception ex)
                {
                    WpfMessageBox.Show(
                        $"ファイルの保存に失敗しました\n{ex.Message}",
                        "エラー",
                        WpfMessageBoxButton.OK,
                        WpfMessageBoxImage.Error,
                        this
                    );
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            WpfMessageBox.Show(
                "Counter Counter v1.0.0\n\nOBS配信者向けリアルタイムカウンターアプリケーション\n\nMade with ❤️ for Streamers",
                "バージョン情報",
                WpfMessageBoxButton.OK,
                WpfMessageBoxImage.Information,
                this
            );
        }

        private void UpdateCurrentFileDisplay()
        {
            string fileName = Path.GetFileName(_configManager.CurrentConfigPath);
            CurrentFileText.Text = fileName;
        }

        private void UpdateSaveMenuState()
        {
            SaveMenuItem.IsEnabled = _hasFilePath;
        }

        private void RefreshAllViews()
        {
            _counterManagementView = null;
            _serverSettingsView = null;
            _connectionInfoView = null;

            ShowCountersView();
        }

        private void MarkAsModified()
        {
            _hasUnsavedChanges = true;
            UpdateSaveMenuState();
        }

        public void StartServerFromTray()
        {
            if (_isServerRunning)
            {
                return;
            }

            try
            {
                SaveSettings();
                StartServer(_httpPort);
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(
                    $"サーバーの起動に失敗しました: {ex.Message}",
                    "エラー",
                    WpfMessageBoxButton.OK,
                    WpfMessageBoxImage.Error,
                    this
                );
            }
        }

        public void StopServerFromTray()
        {
            if (!_isServerRunning)
            {
                return;
            }

            StopServer();
        }

        private void ServerToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_isServerRunning)
            {
                var result = WpfMessageBox.Show(
                    "サーバーを停止しますか?\nOBSからの接続が切断されます。",
                    "確認",
                    WpfMessageBoxButton.YesNo,
                    WpfMessageBoxImage.Question,
                    this
                );

                if (result == WpfMessageBoxResult.Yes)
                {
                    StopServer();
                }
            }
            else
            {
                try
                {
                    SaveSettings();
                    StartServer(_httpPort);
                }
                catch (Exception ex)
                {
                    WpfMessageBox.Show(
                        $"サーバーの起動に失敗しました: {ex.Message}",
                        "エラー",
                        WpfMessageBoxButton.OK,
                        WpfMessageBoxImage.Error,
                        this
                    );
                }
            }
        }

        private void NavCounters_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            ShowCountersView();
            UpdateNavButtons(NavCountersButton);
        }

        private void NavServer_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            ShowServerSettingsView();
            UpdateNavButtons(NavServerButton);
        }

        private void NavConnection_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            ShowConnectionInfoView();
            UpdateNavButtons(NavConnectionButton);
        }

        private void UpdateNavButtons(WpfButton activeButton)
        {
            var inactiveBrush = new WpfSolidColorBrush((WpfColor)WpfColorConverter.ConvertFromString("#444444"));
            var activeBrush = new WpfSolidColorBrush((WpfColor)WpfColorConverter.ConvertFromString("#00d4ff"));

            NavCountersButton.Background = inactiveBrush;
            NavServerButton.Background = inactiveBrush;
            NavConnectionButton.Background = inactiveBrush;

            activeButton.Background = activeBrush;
        }

        private void ShowCountersView()
        {
            if (_counterManagementView == null)
            {
                _counterManagementView = new CounterManagementView(
                    _counterManager,
                    _settings.Hotkeys,
                    _configManager,
                    _settings);
                _counterManagementView.ForceDisplayRequested += OnForceDisplayRequested;
            }

            ContentArea.Children.Clear();
            ContentArea.Children.Add(_counterManagementView);
        }

        private void ShowServerSettingsView()
        {
            if (_serverSettingsView == null)
            {
                _serverSettingsView = new ServerSettingsView(_settings, _isServerRunning);
                _serverSettingsView.SettingsChanged += OnServerSettingsChanged;
            }
            else
            {
                _serverSettingsView.UpdateServerStatus(_isServerRunning);
            }

            ContentArea.Children.Clear();
            ContentArea.Children.Add(_serverSettingsView);
        }

        private void ShowConnectionInfoView()
        {
            if (_connectionInfoView == null)
            {
                _connectionInfoView = new ConnectionInfoView(_httpPort, _isServerRunning);
            }
            else
            {
                _connectionInfoView.UpdateServerStatus(_httpPort, _isServerRunning);
            }

            ContentArea.Children.Clear();
            ContentArea.Children.Add(_connectionInfoView);
        }

        private void OnServerSettingsChanged(object? sender, EventArgs e)
        {
            if (_serverSettingsView == null) return;

            _settings.ServerPort = _serverSettingsView.GetHttpPort();
            _settings.SlideInIntervalMs = _serverSettingsView.GetSlideInInterval();
            _settings.RotationIntervalMs = _serverSettingsView.GetRotationInterval();
            _settings.NextRotationHotkey = _serverSettingsView.GetNextRotationHotkey();

            if (!_isServerRunning)
            {
                _httpPort = _settings.ServerPort;
            }

            Console.WriteLine($"[MainWindow] RotationHotkey updated: {_settings.NextRotationHotkey?.GetDisplayText() ?? "null"}");
            MarkAsModified();
            SaveSettings();
        }

        private void OnForceDisplayRequested(object? sender, string counterId)
        {
            _webServer?.BroadcastForceDisplay(counterId);
        }

        private void StartServer(int port)
        {
            _hotkeyManager?.Dispose();
            _hotkeyManager = new HotkeyManager();
            _hotkeyManager.Initialize(_hwnd);

            RegisterHotkeys();

            _webServer = new WebServer(_counterManager, _settings.RotationIntervalMs, _settings.SlideInIntervalMs);
            Task.Run(async () => await _webServer.StartAsync(port));

            _isServerRunning = true;
            _httpPort = port;

            UpdateServerToggleButton();
            UpdateServerStatus();
            UpdateTrayIcon();

            _serverSettingsView?.UpdateServerStatus(true);
            _connectionInfoView?.UpdateServerStatus(_httpPort, true);
        }

        private void StopServer()
        {
            _hotkeyManager?.Dispose();
            _hotkeyManager = null;

            _webServer?.Dispose();
            _webServer = null;

            _isServerRunning = false;

            UpdateServerToggleButton();
            UpdateServerStatus();
            UpdateTrayIcon();

            _serverSettingsView?.UpdateServerStatus(false);
            _connectionInfoView?.UpdateServerStatus(_httpPort, false);
        }

        private void UpdateServerToggleButton()
        {
            if (_isServerRunning)
            {
                ServerToggleButton.Content = "サーバー停止";
                var stopBrush = new WpfLinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1)
                };
                stopBrush.GradientStops.Add(new WpfGradientStop(
                    (WpfColor)WpfColorConverter.ConvertFromString("#ff4757"), 0));
                stopBrush.GradientStops.Add(new WpfGradientStop(
                    (WpfColor)WpfColorConverter.ConvertFromString("#ff1744"), 1));
                ServerToggleButton.Background = stopBrush;
                ServerToggleButton.Tag = WpfColorConverter.ConvertFromString("#ff4757");
            }
            else
            {
                ServerToggleButton.Content = "サーバー起動";
                var startBrush = new WpfLinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1)
                };
                startBrush.GradientStops.Add(new WpfGradientStop(
                    (WpfColor)WpfColorConverter.ConvertFromString("#5fec5f"), 0));
                startBrush.GradientStops.Add(new WpfGradientStop(
                    (WpfColor)WpfColorConverter.ConvertFromString("#2ecc71"), 1));
                ServerToggleButton.Background = startBrush;
                ServerToggleButton.Tag = WpfColorConverter.ConvertFromString("#5fec5f");
            }
        }

        private void UpdateTrayIcon()
        {
            var app = System.Windows.Application.Current as App;
            app?.UpdateTrayIconServerStatus(_isServerRunning, _httpPort);
        }

        private void RegisterHotkeys()
        {
            if (_hotkeyManager == null) return;

            foreach (var hotkey in _settings.Hotkeys)
            {
                bool success = _hotkeyManager.RegisterHotkey(
                    hotkey.CounterId,
                    hotkey.Action,
                    hotkey.Modifiers,
                    hotkey.VirtualKey);

                if (!success)
                {
                    Console.WriteLine($"ホットキー登録失敗: {hotkey.GetDisplayText()}");
                }
            }

            if (_settings.NextRotationHotkey != null)
            {
                bool success = _hotkeyManager.RegisterHotkey(
                    "",
                    HotkeyAction.NextRotation,
                    _settings.NextRotationHotkey.Modifiers,
                    _settings.NextRotationHotkey.VirtualKey);

                if (!success)
                {
                    Console.WriteLine($"ローテーションホットキー登録失敗: {_settings.NextRotationHotkey.GetDisplayText()}");
                }
                else
                {
                    Console.WriteLine($"[MainWindow] ローテーションホットキー登録成功: {_settings.NextRotationHotkey.GetDisplayText()}");
                }
            }
            else
            {
                Console.WriteLine("[MainWindow] ローテーションホットキー: 未設定");
            }

            _hotkeyManager.HotkeyPressed += OnHotkeyPressed;
        }

        private void OnHotkeyPressed(object? sender, HotkeyPressedEventArgs e)
        {
            switch (e.Action)
            {
                case HotkeyAction.Increment:
                    _counterManager.Increment(e.CounterId);
                    break;
                case HotkeyAction.Decrement:
                    _counterManager.Decrement(e.CounterId);
                    break;
                case HotkeyAction.Reset:
                    _counterManager.Reset(e.CounterId);
                    break;
                case HotkeyAction.NextRotation:
                    Console.WriteLine("[MainWindow] NextRotation hotkey pressed!");
                    _webServer?.BroadcastNextRotation();
                    break;
            }
        }

        private void UpdateServerStatus()
        {
            if (_isServerRunning)
            {
                ServerStatusText.Text = "起動中";
                ServerStatusDot.Fill = new WpfSolidColorBrush((WpfColor)WpfColorConverter.ConvertFromString("#5fec5f"));
            }
            else
            {
                ServerStatusText.Text = "停止中";
                ServerStatusDot.Fill = new WpfSolidColorBrush((WpfColor)WpfColorConverter.ConvertFromString("#ff4757"));
            }
        }

        private void SaveSettings()
        {
            _settings.Counters = _counterManager.GetAllCounters();

            bool saved = _configManager.Save(_settings);
            if (saved)
            {
                _hasUnsavedChanges = false;

                if (_hasFilePath)
                {
                    _appSettings.LastOpenedFilePath = _configManager.CurrentConfigPath;
                    _configManager.SaveAppSettings(_appSettings);
                }

                Console.WriteLine($"[MainWindow] Settings saved successfully. RotationHotkey: {_settings.NextRotationHotkey?.GetDisplayText() ?? "null"}");
            }
            else
            {
                Console.WriteLine("[MainWindow] Failed to save settings");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Cleanup()
        {
            SaveSettings();

            _hotkeyManager?.Dispose();
            _webServer?.Dispose();
        }
    }
}