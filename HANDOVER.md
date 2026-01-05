# カウンター・カウンター 開発引き継ぎ資料

**作成日**: 2026-01-04  
**最終更新**: 2026-01-05 (セッション5 - UIフォルダ整理 & 追加仕様確定)  
**プロジェクト名**: Counter Counter（カウンター・カウンター）  
**技術スタック**: C# + .NET 8 + WPF

---

## 📊 現在の進捗状況

### 全体進捗: **92%完了**

| フェーズ | 進捗 | 状態 |
|---------|------|------|
| Phase 1: 環境構築 | 100% | ✅ 完了 |
| Phase 2: コア機能実装 | 100% | ✅ 完了 |
| Phase 3: GUI実装 | 90% | 🔄 ほぼ完了 |
| Phase 4: アニメーション | 10% | 🔄 一部実装 |
| Phase 5: EXE化・配布 | 0% | ⏳ 未着手 |

---

## 🎯 重要な設計変更

### セッション3: 複数カウンター対応
**変更前**: 単一カウンターのみ対応  
**変更後**: 複数カウンター管理システム

### セッション4: モダンUI + サーバー手動起動
**変更前**: タブUI、サーバー自動起動、通知あり、ブラウザ管理画面あり  
**変更後**: サイドバーナビゲーション、サーバー手動起動、通知なし、WPF設定画面のみ

### セッション5: UIフォルダ整理 + 追加仕様
**UIフォルダ構造の整理**:
- Dialogs/ フォルダ新設（CounterEditDialog移動）
- Infrastructure/ フォルダ新設（TrayIcon移動）
- ViewModels/ フォルダ新設（使ってないなら削除候補）
- Components/ フォルダ新設（使ってないなら削除候補）

**追加仕様**:
1. **CounterEditDialog 拡張**
   - ホットキー設定機能追加（増加・減少のキー個別設定）
   - 色選択をカラーピッカー方式に変更（プリセットボタン廃止）

2. **サーバー設定画面変更**
   - 起動/停止ボタンをトグルボタンに統合
   - サーバー起動中はポート番号変更不可

3. **アプリ起動仕様変更**
   - 起動時は画面非表示（タスクトレイのみ表示）

4. **タスクトレイメニュー変更**
   - 「設定を保存」削除（自動保存に変更）
   - 「サーバー起動」「サーバー停止」追加

5. **不要ファイル削除**
   - manager.css 削除
   - manager.js 削除
   - index.html 削除（ブラウザ管理画面廃止）

---

## ⚠️ 重要な注意事項

### 🔴 名前空間の曖昧参照問題【最重要】

**このプロジェクトでは頻繁に `System.Drawing` と `System.Windows.Media` の名前空間衝突が発生します！**

#### 問題の原因
- `System.Windows.Forms.NotifyIcon` を使用 → `System.Drawing` が参照される
- WPF を使用 → `System.Windows.Media` が参照される
- 両方に `Color`, `ColorConverter`, `Brush` などの同名の型が存在

#### 解決方法：必ずエイリアスを使用する

```csharp
// ファイル先頭で必ずエイリアスを定義
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfBrush = System.Windows.Media.Brush;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;

// 使用時は必ずエイリアスを使う
var color = (WpfColor)WpfColorConverter.ConvertFromString("#00d4ff");
var brush = new WpfSolidColorBrush(color);
```

---

## ✅ 完了した機能

### 1. プロジェクト基盤
- [x] Visual Studio 2022 プロジェクト作成
- [x] .NET 8 環境構築
- [x] NuGetパッケージ導入
- [x] 名前空間整理完了 (Core/Server/UI/Models)
- [x] フォルダ構造の整理
- [x] UIフォルダの機能別整理

### 2. データモデル (Models/)
- [x] `Counter.cs` - カウンター情報モデル
- [x] `HotkeySettings.cs` - ホットキー設定
- [x] `CounterSettings.cs` - 全体設定

### 3. コア機能 (Core/)
- [x] `CounterManager.cs` - 複数カウンター管理
- [x] `HotkeyManager.cs` - グローバルホットキー（動的登録）
- [x] `ConfigManager.cs` - 設定ファイル管理

### 4. サーバー機能 (Server/)
- [x] `WebServer.cs` - HTTPサーバー（手動起動）
- [x] `WebSocketServer.cs` - WebSocketサーバー
- [x] `ApiHandler.cs` - APIエンドポイント処理
- [x] `HtmlContentProvider.cs` - HTML生成
- [x] `StaticFileProvider.cs` - 静的ファイル読み込み

### 5. UI機能 (UI/)
- [x] `MainWindow.xaml` - モダンデザイン設定画面
- [x] `MainWindow.xaml.cs` - ビュー管理、サーバー制御
- [x] `Dialogs/CounterEditDialog.xaml` - カウンター編集ダイアログ
- [x] `Dialogs/CounterEditDialog.xaml.cs` - 編集ロジック
- [x] `Infrastructure/TrayIcon.cs` - タスクトレイ管理
- [x] `Views/CounterManagementView.xaml` - カウンター一覧・操作
- [x] `Views/CounterManagementView.xaml.cs` - カウンター管理ロジック
- [x] `Views/ServerSettingsView.xaml` - サーバー設定画面
- [x] `Views/ServerSettingsView.xaml.cs` - サーバー起動/停止制御
- [x] `Views/ConnectionInfoView.xaml` - 接続情報表示
- [x] `Views/ConnectionInfoView.xaml.cs` - URL表示・コピー機能
- [x] 名前空間の曖昧参照エラー修正完了

### 6. Webインターフェース (wwwroot/)
- [x] `obs.html` - OBS表示画面
- [x] `css/obs.css` - OBS表示スタイル
- [x] `js/obs.js` - OBS表示ロジック

### 7. アプリケーション統合
- [x] `App.xaml.cs` - 起動処理（サーバー自動起動なし）

---

## 🔧 現在のシステム構成

```
CounterCounter.exe
├─ タスクトレイ常駐 ✅
├─ 初期起動時は画面非表示 ← 新仕様（未実装）
├─ サーバー手動起動方式 ✅
│  ├─ HTTPサーバー (Port: 8765) - 手動起動
│  ├─ WebSocketサーバー (Port: 8766) - 手動起動
│  └─ グローバルホットキー - サーバー起動時に登録
├─ 複数カウンター管理 ✅
├─ 設定の自動保存 (config.json) ← 新仕様（未実装）
└─ モダンWPF設定画面 ✅
   ├─ サイドバーナビゲーション ✅
   ├─ カウンター管理ビュー ✅
   ├─ サーバー設定ビュー（トグルボタン） ← 変更予定
   └─ 接続情報ビュー ✅

OBS表示 (http://localhost:8765/obs.html)
└─ WebSocket接続で複数カウンターをリアルタイム表示 ✅
```

---

## 📁 プロジェクト構造（整理後）

```
CounterCounter/
├── Core/
│   ├── CounterManager.cs
│   ├── HotkeyManager.cs
│   └── ConfigManager.cs
├── Server/
│   ├── ApiHandler.cs
│   ├── HtmlContentProvider.cs
│   ├── StaticFileProvider.cs
│   ├── WebServer.cs
│   └── WebSocketServer.cs
├── UI/
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── Dialogs/                   # 新設
│   │   ├── CounterEditDialog.xaml
│   │   └── CounterEditDialog.xaml.cs
│   ├── Infrastructure/                    # 新設
│   │   └── TrayIcon.cs
│   ├── ViewModels/                # 新設（削除候補）
│   │   └── CounterViewModel.cs
│   ├── Components/                # 新設（削除候補）
│   │   ├── CounterListItem.xaml
│   │   └── CounterListItem.xaml.cs
│   └── Views/
│       ├── CounterManagementView.xaml
│       ├── CounterManagementView.xaml.cs
│       ├── ServerSettingsView.xaml
│       ├── ServerSettingsView.xaml.cs
│       ├── HotkeySettingsView.xaml
│       ├── HotkeySettingsView.xaml.cs
│       ├── ConnectionInfoView.xaml
│       └── ConnectionInfoView.xaml.cs
├── Models/
│   ├── Counter.cs
│   ├── HotkeySettings.cs
│   └── CounterSettings.cs
├── wwwroot/
│   ├── obs.html
│   ├── css/
│   │   ├── obs.css
│   │   └── manager.css            # 削除予定
│   └── js/
│       ├── obs.js
│       └── manager.js             # 削除予定
├── Resources/
├── App.xaml
├── App.xaml.cs
├── CounterCounter.csproj
└── config.json
```

---

## 🎯 次に実装すべき機能

### 優先度：最高 🔥🔥🔥

#### 1. CounterEditDialog の拡張実装
- [ ] カラーピッカーの実装
  - `System.Windows.Forms.ColorDialog` を使用
  - カラーコード（#RRGGBB）取得
  - プリセットボタン削除
- [ ] ホットキー設定機能の追加
  - 増加キー設定UI
  - 減少キー設定UI
  - 「記録」ボタンでキー入力待機
  - キー競合チェック

#### 2. ServerSettingsView の変更
- [ ] 起動/停止を1つのトグルボタンに統合
- [ ] サーバー起動中はポート番号テキストボックスを無効化
- [ ] ボタンのラベルを動的に変更

#### 3. App.xaml.cs の修正
- [ ] 起動時にMainWindowを非表示にする
- [ ] タスクトレイからのみ表示可能にする

#### 4. TrayIcon の修正
- [ ] 「設定を保存」メニュー項目削除
- [ ] 「サーバー起動」メニュー項目追加
- [ ] 「サーバー停止」メニュー項目追加
- [ ] サーバー起動/停止のイベント実装

#### 5. 不要ファイルの削除
- [ ] `wwwroot/index.html` 削除
- [ ] `wwwroot/css/manager.css` 削除
- [ ] `wwwroot/js/manager.js` 削除
- [ ] `UI/ViewModels/CounterViewModel.cs` 削除（使ってない場合）
- [ ] `UI/Components/CounterListItem.xaml` + `.cs` 削除（使ってない場合）

#### 6. 設定の自動保存実装
- [ ] カウンター編集時に自動保存
- [ ] サーバー設定変更時に自動保存
- [ ] 手動保存ボタンの削除

---

## 🐛 既知の問題・制限事項

### 未解決（要修正）
- ⚠️ CounterEditDialog 拡張が未実装
- ⚠️ ServerSettingsView のトグルボタン化が未実装
- ⚠️ アプリ起動時の画面非表示が未実装
- ⚠️ TrayIcon のメニュー変更が未実装
- ⚠️ 不要ファイルの削除が未実施
- ⚠️ アイコンが仮アイコン
- ⚠️ カウンターの並び替え機能なし

### 解決済み ✅
- ✅ 名前空間の曖昧参照エラー完全修正
- ✅ UIフォルダの構造整理完了
- ✅ 複数カウンター対応完了
- ✅ グローバルホットキー実装完了
- ✅ 設定の永続化完了

---

## 💻 開発環境情報

### 必須環境
- **OS**: Windows 10/11
- **IDE**: Visual Studio 2022
- **.NET**: .NET 8.0

### NuGetパッケージ
```xml
<PackageReference Include="WebSocketSharp-netstandard" Version="1.0.1" />
```

---

## 📝 重要な設計決定

### 1. 名前空間設計
```csharp
CounterCounter
├── CounterCounter.Core
├── CounterCounter.Server
├── CounterCounter.UI
│   ├── CounterCounter.UI.Dialogs
│   ├── CounterCounter.UI.Infrastructure
│   └── CounterCounter.UI.Views
└── CounterCounter.Models
```

### 2. エイリアス使用規則
```csharp
// WPF関連
using WpfButton = System.Windows.Controls.Button;
using WpfMessageBox = System.Windows.MessageBox;
using WpfUserControl = System.Windows.Controls.UserControl;
using WpfClipboard = System.Windows.Clipboard;

// 色関連（最重要）
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfBrush = System.Windows.Media.Brush;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;

// WinForms関連
using WinForms = System.Windows.Forms;
```

### 3. サーバー起動制御
- **起動時**: サーバー停止、ホットキー未登録、WPF画面非表示
- **手動起動**: ユーザーがトグルボタンまたはタスクトレイメニューからクリック
- **起動処理**: HTTPサーバー → WebSocketサーバー → ホットキー登録
- **停止処理**: ホットキー解除 → WebSocketサーバー停止 → HTTPサーバー停止

### 4. ポート設定
- **HTTPサーバー**: 8765（自動選択、競合時は8766, 8767...）
- **WebSocketサーバー**: HTTPポート+1（例: 8766）
- **サーバー起動中はポート番号変更不可**

---

## 💡 config.json の構造

```json
{
  "Counters": [
    {
      "Id": "default",
      "Name": "Default Counter",
      "Value": 5,
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
- `Modifiers`: 2=Ctrl, 4=Shift, 6=Ctrl+Shift, 1=Alt
- `VirtualKey`: 38=↑, 40=↓, 82=R

---

## 💡 開発のヒント

### ColorDialog 使用例
```csharp
using WinForms = System.Windows.Forms;
using WpfColor = System.Windows.Media.Color;

private void SelectColor_Click(object sender, RoutedEventArgs e)
{
    var colorDialog = new WinForms.ColorDialog();
    colorDialog.FullOpen = true;
    
    if (colorDialog.ShowDialog() == WinForms.DialogResult.OK)
    {
        var winColor = colorDialog.Color;
        string hexColor = $"#{winColor.R:X2}{winColor.G:X2}{winColor.B:X2}";
        _selectedColor = hexColor;
    }
}
```

### ホットキー入力待機の実装例
```csharp
private bool _isRecording = false;

private void RecordHotkey_Click(object sender, RoutedEventArgs e)
{
    _isRecording = true;
    RecordButton.Content = "キーを押してください...";
    this.KeyDown += OnKeyDownWhileRecording;
}

private void OnKeyDownWhileRecording(object sender, KeyEventArgs e)
{
    if (!_isRecording) return;
    
    _isRecording = false;
    this.KeyDown -= OnKeyDownWhileRecording;
    
    uint modifiers = 0;
    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) modifiers |= 0x0002;
    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) modifiers |= 0x0004;
    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) modifiers |= 0x0001;
    
    uint vk = (uint)KeyInterop.VirtualKeyFromKey(e.Key);
    
    RecordButton.Content = $"{GetModifierString(modifiers)}{e.Key}";
}
```

---

## 📞 引き継ぎ時の確認事項

次のセッションで開発を続ける場合、以下を確認：
- [x] プロジェクト構造の理解
- [x] 複数カウンター対応の設計
- [x] 名前空間の整理状況
- [x] モダンUI設計の理解
- [x] サーバー手動起動方式の理解
- [x] UIフォルダの機能別整理
- [x] 名前空間の曖昧参照エラー完全修正
- [ ] 次の実装対象の確認
- [ ] 未解決の問題やエラーの確認

---

**最終更新日**: 2026-01-05  
**次回セッションの最優先タスク**: CounterEditDialog拡張実装