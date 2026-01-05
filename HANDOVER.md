# カウンター・カウンター 開発引き継ぎ資料

**最終更新**: 2026-01-06 (セッション7)  
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

## 🎯 セッション7で完了したこと

### 1. CounterEditDialog拡張 ✅
- **カラーピッカー実装**
  - プリセットボタン削除
  - `System.Windows.Forms.ColorDialog`統合
  - リアルタイムカラープレビュー機能

- **ホットキー設定機能**
  - 増加キー/減少キー個別設定
  - 「記録」ボタンでキー入力待機
  - Ctrl/Alt/Shift組み合わせ対応
  - Escapeキーでキャンセル機能
  - `OnPreviewKeyDown`でキー入力処理

### 2. CounterManagementView更新 ✅
- 新しいダイアログに対応
- ホットキー情報の表示（増加/減少両方）
- 自動保存機能実装
- ConfigManager/CounterSettings連携

### 3. MainWindow更新 ✅
- ConfigManager/CounterSettingsを渡すように修正
- 終了時の自動保存
- ホットキー登録時の設定反映

---

## 🔴 次に実装すべき機能

### 最優先: ホットキー競合チェック【推定20分】
現在、ホットキー登録時に他のカウンターと重複していてもエラーが出ない。
HotkeyManagerに重複チェック機能があるので、CounterEditDialogから呼び出す。

```csharp
// 実装イメージ
private bool CheckHotkeyConflict(uint modifiers, uint vk, string currentCounterId)
{
    foreach (var hotkey in _existingHotkeys)
    {
        if (hotkey.CounterId != currentCounterId &&
            hotkey.Modifiers == modifiers &&
            hotkey.VirtualKey == vk)
        {
            return true; // 競合あり
        }
    }
    return false; // 競合なし
}
```

### 高優先度: サーバー起動時のホットキー再登録【推定15分】
現在、サーバー停止→再起動時にホットキーが正しく再登録されない可能性がある。
MainWindow.StartServer()でホットキー登録を確実に行う。

### 中優先度: アニメーション実装
- スライドイン演出
- パーティクル演出

### 低優先度: その他
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
│   │   └── CounterEditDialog.xaml(.cs) ← セッション7で拡張
│   ├── Infrastructure/
│   │   └── TrayIcon.cs
│   ├── Components/
│   │   └── CounterCard.xaml(.cs)
│   └── Views/
│       ├── CounterManagementView.xaml(.cs) ← セッション7で更新
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
using WinForms = System.Windows.Forms;
```

### チェックリスト
- ✅ 新しいファイルを作成したら、必ず先頭でエイリアスを定義
- ✅ Color, ColorConverter, Brush を使う前に、エイリアスで修飾されているか確認
- ✅ ビルドエラーが出たら、まず名前空間の曖昧参照を疑う

---

## 💡 重要な実装メモ

### CounterEditDialog の主要機能

#### 1. カラーピッカー
```csharp
private void SelectColor_Click(object sender, RoutedEventArgs e)
{
    using var colorDialog = new WinForms.ColorDialog();
    colorDialog.FullOpen = true;
    
    if (colorDialog.ShowDialog() == WinForms.DialogResult.OK)
    {
        var drawingColor = colorDialog.Color;
        _selectedColor = $"#{drawingColor.R:X2}{drawingColor.G:X2}{drawingColor.B:X2}";
        UpdateColorPreview();
    }
}
```

#### 2. ホットキー記録
```csharp
protected override void OnPreviewKeyDown(KeyEventArgs e)
{
    if (!_isRecordingIncrementKey && !_isRecordingDecrementKey)
        return;

    e.Handled = true;

    // Escapeでキャンセル
    if (e.Key == Key.Escape) { /* ... */ }

    // 修飾キー取得
    uint modifiers = 0;
    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        modifiers |= 0x0002;
    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
        modifiers |= 0x0004;
    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
        modifiers |= 0x0001;

    // VirtualKey取得
    uint virtualKey = (uint)KeyInterop.VirtualKeyFromKey(e.Key);
}
```

#### 3. ホットキー保存
```csharp
// OKボタンで HotkeySettings を生成
if (_incrementModifiers != 0 && _incrementVirtualKey != 0)
{
    IncrementHotkey = new HotkeySettings(
        Counter.Id,
        HotkeyAction.Increment,
        _incrementModifiers,
        _incrementVirtualKey
    );
}
```

### CounterManagementView の自動保存
```csharp
private void AutoSaveSettings()
{
    _settings.Counters = _counterManager.GetAllCounters();
    _settings.Hotkeys = _hotkeySettings;
    _configManager.Save(_settings);
}
```

カウンター編集・追加・削除時に自動で呼び出される。

---

## 🐛 既知の問題

### 未実装（優先度: 高）
- ⚠️ ホットキー競合チェック未実装
- ⚠️ サーバー再起動時のホットキー再登録が不安定な可能性
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
- ✅ ホットキー設定機能

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

**説明**:
- `Action`: 0=Increment, 1=Decrement, 2=Reset
- `Modifiers`: 1=Alt, 2=Ctrl, 4=Shift（ビットフラグ、組み合わせ可能）
- `VirtualKey`: Win32 VirtualKey コード

---

## 🔧 次回セッションの開始手順

1. プロジェクトを開く
2. ビルドして動作確認
3. カウンター追加でColorDialogとホットキー設定をテスト
4. **次の優先タスク**: ホットキー競合チェック実装

---

## 🎉 セッション7の成果

- CounterEditDialog拡張完了（カラーピッカー + ホットキー設定）
- 自動保存機能実装
- ホットキー表示の改善
- プロジェクト完成度: **95% → 98%**

**次回の最優先タスク**: ホットキー競合チェック実装（20分程度）

---

**開発中のため、機能や仕様は予告なく変更される可能性があります**

Made with ❤️ for Streamers