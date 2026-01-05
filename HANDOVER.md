# カウンター・カウンター 開発引き継ぎ資料

**最終更新**: 2026-01-06 (セッション6)  
**プロジェクト**: Counter Counter  
**技術**: C# + .NET 8 + WPF

---

## 📊 現在の進捗: 95%完了

| フェーズ | 進捗 | 状態 |
|---------|------|------|
| Phase 1: 環境構築 | 100% | ✅ |
| Phase 2: コア機能実装 | 100% | ✅ |
| Phase 3: GUI実装 | 100% | ✅ |
| Phase 4: アニメーション | 10% | 🔄 |
| Phase 5: EXE化・配布 | 0% | ⏳ |

---

## 🎯 セッション6で完了したこと

### 1. TrayIconのトグル化 ✅
- サーバー起動/停止を1つのメニュー項目で切り替え
- `UpdateServerStatus(bool, int)` でメニュー更新

### 2. App.xaml.cs修正 ✅
- 起動時に画面非表示（タスクトレイのみ）
- TrayIconイベント連携完了

### 3. 不要ファイル削除 ✅
削除したファイル:
- `wwwroot/index.html`
- `wwwroot/css/manager.css`
- `wwwroot/js/manager.js`
- `UI/ViewModels/CounterViewModel.cs`
- `UI/Components/CounterListItem.xaml(.cs)`

### 4. CounterCardコンポーネント化 ✅
- `UI/Components/CounterCard` 新規作成
- イベント駆動設計で保守性向上

### 5. ServerSettingsViewトグルボタン化 ✅
- 起動/停止を1つのボタンで制御
- 起動中はポート変更不可
- 色とラベルが動的変更（緑/赤）

---

## 🔴 次に実装すべき機能

### 最優先: CounterEditDialog 拡張【40分】
1. **カラーピッカー実装**
   - `System.Windows.Forms.ColorDialog` 使用
   - プリセットボタン削除
   
2. **ホットキー設定機能**
   - 増加/減少キー個別設定
   - 「記録」ボタンでキー入力待機
   - キー競合チェック

### 高優先度: 設定自動保存【15分】
- カウンター編集時に自動保存
- サーバー設定変更時に自動保存
- 手動保存ボタンは残す

---

## 📁 現在のプロジェクト構造
```
CounterCounter/
├── Core/
│   ├── CounterManager.cs
│   ├── HotkeyManager.cs
│   └── ConfigManager.cs
├── Server/
│   ├── WebServer.cs
│   ├── WebSocketServer.cs
│   ├── ApiHandler.cs
│   ├── HtmlContentProvider.cs
│   └── StaticFileProvider.cs
├── UI/
│   ├── MainWindow.xaml(.cs)
│   ├── Dialogs/
│   │   └── CounterEditDialog.xaml(.cs)
│   ├── Infrastructure/
│   │   └── TrayIcon.cs
│   ├── Components/
│   │   └── CounterCard.xaml(.cs)
│   └── Views/
│       ├── CounterManagementView.xaml(.cs)
│       ├── ServerSettingsView.xaml(.cs)
│       └── ConnectionInfoView.xaml(.cs)
├── Models/
│   ├── Counter.cs
│   ├── HotkeySettings.cs
│   └── CounterSettings.cs
├── wwwroot/
│   ├── obs.html
│   ├── css/obs.css
│   └── js/obs.js
└── App.xaml(.cs)
```

---

## ⚠️ 重要: 名前空間エイリアス

必ず各ファイル先頭で定義:
```csharp
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;
using WpfMessageBox = System.Windows.MessageBox;
using WinForms = System.Windows.Forms;
```

---

## 💡 重要な実装メモ

### TrayIcon連携
```csharp
// App.xaml.cs
_trayIcon.ServerStartRequested += OnServerStartRequested;
_trayIcon.ServerStopRequested += OnServerStopRequested;

// MainWindow.xaml.cs
public void StartServerFromTray() { }
public void StopServerFromTray() { }
```

### ServerSettingsViewの状態更新
```csharp
// MainWindowから呼び出し
_serverSettingsView?.UpdateServerStatus(bool isRunning, int httpPort);
```

### CounterCardのイベント
```csharp
<components:CounterCard 
    IncrementRequested="CounterCard_IncrementRequested"
    DeleteRequested="CounterCard_DeleteRequested"/>
```

---

## 🐛 既知の問題

### 未実装（優先度: 高）
- CounterEditDialog拡張（カラーピッカー/ホットキー設定）
- 設定自動保存
- アイコンが仮アイコン

### 未実装（優先度: 中）
- アニメーション機能
- カウンター並び替え

---

## 📝 config.json構造
```json
{
  "Counters": [
    {
      "Id": "default",
      "Name": "Default Counter",
      "Value": 0,
      "Color": "#00ff00"
    }
  ],
  "Hotkeys": [
    {
      "CounterId": "default",
      "Action": 0,
      "Modifiers": 6,
      "VirtualKey": 38
    }
  ],
  "ServerPort": 8765
}
```

---

## 🔧 次回セッションの開始手順

1. プロジェクトを開く
2. ビルドして動作確認
3. CounterEditDialog拡張から開始
4. または設定自動保存から開始

**次回の最優先タスク**: CounterEditDialog拡張実装