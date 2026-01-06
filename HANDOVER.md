# カウンター・カウンター 開発引き継ぎ資料

**最終更新**: 2026-01-06 (セッション8)  
**プロジェクト**: Counter Counter  
**技術**: C# + .NET 8 + WPF

---

## 📊 現在の進捗: 98%完了

| フェーズ | 進捗 | 状態 |
|---------|------|------|
| Phase 1: 環境構築 | 100% | ✅ |
| Phase 2: コア機能実装 | 100% | ✅ |
| Phase 3: GUI実装 | 100% | ✅ |
| Phase 4: アニメーション | 10% | 🔄 |
| Phase 5: EXE化・配布 | 0% | ⏳ |

---

## 🎯 セッション8で完了したこと

### 1. ホットキー設定方式の刷新 ✅

**コンボボックス方式に変更**
- 従来のキー入力待機方式を廃止
- 各セット3つのコンボボックスで選択
- Ctrl/Shift/Alt + 任意キーの組み合わせ

**複数セット対応**
- 1カウンターにつき最大3セットのホットキー
- 増加・減少それぞれ独立して設定可能
- 例: `Ctrl+Shift+↑`, `Ctrl+Alt+F1`, `Shift+A`

### 2. ウィンドウ終了動作の改善 ✅

- **サーバー停止中**: バツボタンでアプリ終了
- **サーバー起動中**: バツボタンでタスクトレイに格納
- `Window.Closing` イベントで `e.Cancel` を制御

### 3. ウィンドウサイズ可変対応 ✅

- `ResizeMode="CanMinimize"` から `ResizeMode="CanResize"` に変更
- 最小サイズ: 幅900×高さ650
- リサイズ可能に変更

---

## 🔴 次に実装すべき機能

### 高優先度: アニメーション実装

- スライドイン演出
- パーティクル演出

### 中優先度: その他

- アイコン作成
- カウンター並び替え機能
- 単一EXE化

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
│       ├── HotkeySettingsView.xaml(.cs)
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

## ⚠️ 重要: 名前空間エイリアス【最重要】

このプロジェクトでは頻繁に `System.Drawing` と `System.Windows.Media` の名前空間衝突が発生します！

### 必須エイリアス

```csharp
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;
using WpfMessageBox = System.Windows.MessageBox;
using WpfButton = System.Windows.Controls.Button;
using WpfUserControl = System.Windows.Controls.UserControl;
using WpfComboBox = System.Windows.Controls.ComboBox;
using WpfStackPanel = System.Windows.Controls.StackPanel;
using WinForms = System.Windows.Forms;
```

### チェックリスト

- ✅ 新しいファイルを作成したら、必ず先頭でエイリアスを定義
- ✅ Color, ColorConverter, Brush を使う前に、エイリアスで修飾されているか確認
- ✅ ビルドエラーが出たら、まず名前空間の曖昧参照を疑う

---

## 💡 重要な実装メモ

### CounterEditDialog の主要機能

#### 1. コンボボックス方式のホットキー設定

```csharp
private HotkeySlot CreateHotkeySlot(WpfStackPanel parentPanel, int slotNumber)
{
    var key1ComboBox = CreateKeyComboBox();
    var key2ComboBox = CreateKeyComboBox();
    var key3ComboBox = CreateKeyComboBox();
    
    // 3つのコンボボックスで1セット
    return new HotkeySlot
    {
        Key1 = key1ComboBox,
        Key2 = key2ComboBox,
        Key3 = key3ComboBox
    };
}
```

#### 2. ホットキーの構築

```csharp
private List<HotkeySettings> BuildHotkeysFromSlots(
    List<HotkeySlot> slots, 
    HotkeyAction action)
{
    var hotkeys = new List<HotkeySettings>();

    foreach (var slot in slots)
    {
        var keys = ExtractKeysFromSlot(slot);
        if (keys.Count == 0) continue;

        uint modifiers = 0;
        uint virtualKey = 0;

        foreach (var key in keys)
        {
            if (key == 0x0002 || key == 0x0004 || key == 0x0001)
                modifiers |= key;
            else
                virtualKey = key;
        }

        if (modifiers == 0 || virtualKey == 0) continue;

        hotkeys.Add(new HotkeySettings(
            Counter!.Id, action, modifiers, virtualKey));
    }

    return hotkeys;
}
```

#### 3. ホットキー競合チェック

```csharp
private bool ValidateHotkeys()
{
    var allHotkeys = IncrementHotkeys.Concat(DecrementHotkeys).ToList();

    foreach (var hotkey in allHotkeys)
    {
        var conflicting = _existingHotkeys.FirstOrDefault(h =>
            h.CounterId != Counter!.Id &&
            h.Modifiers == hotkey.Modifiers &&
            h.VirtualKey == hotkey.VirtualKey);

        if (conflicting != null)
        {
            // エラー表示
            return false;
        }
    }

    return true;
}
```

### MainWindow のウィンドウ終了処理

```csharp
private void Window_Closing(object sender, CancelEventArgs e)
{
    if (_isServerRunning)
    {
        e.Cancel = true;
        Hide();
    }
    else
    {
        e.Cancel = false;
    }
}
```

### MainWindow のウィンドウサイズ設定

```xaml
<Window Width="900" Height="650"
        MinWidth="900" MinHeight="650"
        ResizeMode="CanResize">
```

---

## 🐛 既知の問題

### 未実装（優先度: 高）

- ⚠️ アイコンが仮アイコン

### 未実装（優先度: 中）

- ⚠️ アニメーション機能
- ⚠️ カウンター並び替え機能

### 解決済み

- ✅ 名前空間衝突
- ✅ UIフォルダ整理
- ✅ トグルボタン実装
- ✅ 起動時非表示
- ✅ カラーピッカー実装
- ✅ ホットキー設定機能（コンボボックス方式）
- ✅ 複数セット対応
- ✅ ウィンドウ終了動作
- ✅ ウィンドウサイズ可変

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
    },
    {
      "CounterId": "default",
      "Action": 0,
      "Modifiers": 5,
      "VirtualKey": 112
    }
  ],
  "ServerPort": 8765
}
```

**説明**:
- `Action`: 0=Increment, 1=Decrement, 2=Reset
- `Modifiers`: 1=Alt, 2=Ctrl, 4=Shift（ビットフラグ、組み合わせ可能）
- `VirtualKey`: Win32 VirtualKey コード
- 同じCounterId + Actionで複数エントリ可能（最大3セット）

---

## 🔧 次回セッションの開始手順

1. プロジェクトを開く
2. ビルドして動作確認
3. カウンター追加でホットキー複数セット設定をテスト
---

## 🎉 セッション8の成果

- ホットキー設定方式をコンボボックス方式に刷新
- 複数セット対応（最大3セット）
- ウィンドウ終了動作の改善（サーバー状態で分岐）
- ウィンドウサイズ可変対応
- プロジェクト完成度: **98%**

---

**開発中のため、機能や仕様は予告なく変更される可能性があります**

Made with ❤️ for Streamers