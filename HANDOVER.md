# カウンター・カウンター 開発引き継ぎ資料

**作成日**: 2026-01-04  
**最終更新**: 2026-01-05 (セッション4 - モダンUI実装完了)  
**プロジェクト名**: Counter Counter（カウンター・カウンター）  
**技術スタック**: C# + .NET 8 + WPF

---

## 📊 現在の進捗状況

### 全体進捗: **90%完了** 🎉

| フェーズ | 進捗 | 状態 |
|---------|------|------|
| Phase 1: 環境構築 | 100% | ✅ 完了 |
| Phase 2: コア機能実装 | 100% | ✅ 完了 |
| Phase 3: GUI実装 | 95% | 🔄 ほぼ完了 |
| Phase 4: アニメーション | 10% | 🔄 一部実装 |
| Phase 5: EXE化・配布 | 0% | ⏳ 未着手 |

---

## 🎯 重要な設計変更（セッション3〜4）

### セッション3: 複数カウンター対応
**変更前**: 単一カウンターのみ対応  
**変更後**: 複数カウンター管理システム

### セッション4: モダンUI + サーバー手動起動
**変更前**: タブUI、サーバー自動起動、通知あり、ブラウザ管理画面あり  
**変更後**: サイドバーナビゲーション、サーバー手動起動、通知なし、WPF設定画面のみ

#### 主要変更点
1. **サーバー起動方式の変更**
   - アプリ起動時はサーバー停止状態
   - ユーザーが「サーバー起動」ボタンで手動起動
   - 停止時はホットキーも無効

2. **UI設計の刷新**
   - タブ → サイドバーナビゲーション
   - モダン・スタイリッシュなデザイン
   - グラデーション、ドロップシャドウ、角丸の多用
   - ダークテーマ (#0f0f0f 背景)

3. **機能の削除**
   - カウンター値変更時の通知バルーン削除
   - ブラウザ管理画面削除（WPF設定画面のみ）

4. **機能の追加**
   - カウンター一覧にホットキー表示
   - OBS URL の「ブラウザで開く」ボタン
   - OBS URL の「URLをコピー」ボタン

---

## ⚠️ 重要な注意事項

### 🔴 名前空間の曖昧参照問題【最重要】

**このプロジェクトでは頻繁に `System.Drawing` と `System.Windows.Media` の名前空間衝突が発生します！**

#### 問題の原因
- `System.Windows.Forms.NotifyIcon` を使用 → `System.Drawing` が参照される
- WPF を使用 → `System.Windows.Media` が参照される
- 両方に `Color`, `ColorConverter`, `Brush` などの同名の型が存在

#### 典型的なエラーメッセージ
```
'Color' は、'System.Drawing.Color' と 'System.Windows.Media.Color' 間のあいまいな参照です
'ColorConverter' は、'System.Drawing.ColorConverter' と 'System.Windows.Media.ColorConverter' 間のあいまいな参照です
'Brush' は、'System.Drawing.Brush' と 'System.Windows.Media.Brush' 間のあいまいな参照です
```

#### 解決方法：必ずエイリアスを使用する

**✅ 正しいコード例**
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

**❌ 間違ったコード例**
```csharp
// エイリアスなしで使用 → エラー発生！
var color = (Color)ColorConverter.ConvertFromString("#00d4ff");
var brush = new SolidColorBrush(color);
```

#### 影響を受けるファイル
- `MainWindow.xaml.cs`
- `CounterManagementView.xaml.cs`
- `TrayIcon.cs`（`System.Drawing` のみ使用）
- その他、色を扱う全てのファイル

#### チェックリスト
- [ ] 新しいファイルを作成したら、必ず先頭でエイリアスを定義
- [ ] `Color`, `ColorConverter`, `Brush` を使う前に、エイリアスで修飾されているか確認
- [ ] ビルドエラーが出たら、まず名前空間の曖昧参照を疑う
- [ ] `System.Drawing` を使う必要がある場合は、明示的に `System.Drawing.Color` と書く

---

## ✅ 完了した機能

### 1. プロジェクト基盤
- [x] Visual Studio 2022 プロジェクト作成
- [x] .NET 8 環境構築
- [x] NuGetパッケージ導入
  - WebSocketSharp-netstandard
  - System.Windows.Forms
- [x] 名前空間整理完了 (Core/Server/UI/Models)
- [x] フォルダ構造の整理

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
  - サイドバーナビゲーション
  - グラデーションボタン
  - ダークテーマ
- [x] `MainWindow.xaml.cs` - ビュー管理、サーバー制御
- [x] `CounterManagementView.xaml` - カウンター一覧・操作
- [x] `CounterManagementView.xaml.cs` - カウンター管理ロジック
- [x] `ServerSettingsView.xaml` - サーバー設定画面
- [x] `ServerSettingsView.xaml.cs` - サーバー起動/停止制御
- [x] `ConnectionInfoView.xaml` - 接続情報表示
- [x] `ConnectionInfoView.xaml.cs` - URL表示・コピー機能
- [x] `CounterEditDialog.xaml` - カウンター編集ダイアログ
- [x] `CounterEditDialog.xaml.cs` - 編集ロジック
- [x] `TrayIcon.cs` - タスクトレイ管理（通知なし）

### 6. Webインターフェース (wwwroot/)
- [x] `obs.html` - OBS表示画面
- [x] `css/obs.css` - OBS表示スタイル
- [x] `js/obs.js` - OBS表示ロジック
- [x] `css/manager.css` - 管理画面スタイル（使用されていない）
- [x] `js/manager.js` - 管理画面ロジック（使用されていない）

### 7. アプリケーション統合
- [x] `App.xaml.cs` - 起動処理（サーバー自動起動なし）

---

## 🔧 現在のシステム構成

```
CounterCounter.exe
├─ タスクトレイ常駐 ✅
├─ サーバー手動起動方式 ✅
│  ├─ HTTPサーバー (Port: 8765) - 手動起動
│  ├─ WebSocketサーバー (Port: 8766) - 手動起動
│  └─ グローバルホットキー - サーバー起動時に登録
├─ 複数カウンター管理 ✅
├─ 設定の永続化 (config.json) ✅
└─ モダンWPF設定画面 ✅
   ├─ サイドバーナビゲーション ✅
   ├─ カウンター管理ビュー ✅
   ├─ サーバー設定ビュー ✅
   └─ 接続情報ビュー ✅

OBS表示 (http://localhost:8765/obs.html)
└─ WebSocket接続で複数カウンターをリアルタイム表示 ✅
```

---

## 📁 プロジェクト構造

```
CounterCounter/
├── Core/                           # コア機能
│   ├── CounterManager.cs          # 複数カウンター管理
│   ├── HotkeyManager.cs           # グローバルホットキー（動的登録）
│   └── ConfigManager.cs           # 設定ファイル管理
├── Server/                         # サーバー機能
│   ├── ApiHandler.cs              # APIエンドポイント処理
│   ├── HtmlContentProvider.cs     # HTML生成
│   ├── StaticFileProvider.cs      # 静的ファイル読み込み
│   ├── WebServer.cs               # HTTPサーバー（手動起動）
│   └── WebSocketServer.cs         # WebSocketサーバー
├── UI/                             # UI機能
│   ├── MainWindow.xaml            # モダン設定画面
│   ├── MainWindow.xaml.cs         # ビュー管理・サーバー制御
│   ├── CounterEditDialog.xaml     # カウンター編集ダイアログ
│   ├── CounterEditDialog.xaml.cs  # ダイアログロジック
│   ├── TrayIcon.cs                # タスクトレイ管理（通知なし）
│   └── Views/                     # ビューコンポーネント
│       ├── CounterManagementView.xaml
│       ├── CounterManagementView.xaml.cs
│       ├── ServerSettingsView.xaml
│       ├── ServerSettingsView.xaml.cs
│       ├── ConnectionInfoView.xaml
│       └── ConnectionInfoView.xaml.cs
├── Models/                         # データモデル
│   ├── Counter.cs                 # カウンター情報
│   ├── HotkeySettings.cs          # ホットキー設定
│   └── CounterSettings.cs         # 全体設定
├── wwwroot/                        # Webファイル
│   ├── obs.html                   # OBS表示画面
│   ├── css/
│   │   ├── obs.css                # OBS表示スタイル
│   │   └── manager.css            # 管理画面スタイル（未使用）
│   └── js/
│       ├── obs.js                 # OBS表示ロジック
│       └── manager.js             # 管理画面ロジック（未使用）
├── Resources/                      # リソースファイル（今後使用）
├── App.xaml                       # WPFアプリケーション定義
├── App.xaml.cs                    # アプリケーション起動処理
├── CounterCounter.csproj          # プロジェクト設定
└── config.json                    # 設定ファイル（実行時生成）
```

---

## 🎯 次に実装すべき機能

### 優先度：高 🔥

#### 1. 名前空間の曖昧参照の完全修正
- [ ] 全ファイルで `System.Drawing` vs `System.Windows.Media` の衝突を確認
- [ ] エイリアスの追加漏れを修正
- [ ] ビルドエラー 0 を達成

#### 2. サーバー起動/停止の動作確認
- [ ] サーバー起動ボタンの動作テスト
- [ ] サーバー停止ボタンの動作テスト
- [ ] ホットキー登録/解除の確認

#### 3. OBS表示のテスト
- [ ] OBSでブラウザソース追加
- [ ] URL入力・表示確認
- [ ] WebSocket接続確認
- [ ] リアルタイム更新確認

### 優先度：中

#### 4. アニメーション強化
- [ ] スライドイン演出（上下方向）
- [ ] パーティクルエフェクト（Canvas）
- [ ] アニメーション速度設定対応

#### 5. ホットキー設定UI
- [ ] カウンター毎のホットキー設定ダイアログ
- [ ] キー入力待機機能
- [ ] キー競合表示

### 優先度：低

#### 6. アイコンの作成
- [ ] `Resources/icon.ico` 作成
- [ ] 複数サイズのアイコン含む
- [ ] TrayIconに適用

#### 7. 単体テストの追加
- [ ] CounterManagerのテスト
- [ ] ApiHandlerのテスト
- [ ] HotkeyManagerのテスト

---

## 🐛 既知の問題・制限事項

### 未解決（要修正）
- ⚠️ **名前空間の曖昧参照エラーが複数残っている**
  - `MainWindow.xaml.cs` の一部
  - `CounterManagementView.xaml.cs` の一部
  - 新規作成ファイルでも発生の可能性
- ⚠️ ホットキー設定UIが未実装（現在はconfig.json手動編集が必要）
- ⚠️ アイコンが仮アイコン（SystemIcons.Application）
- ⚠️ カウンターの並び替え機能なし

### 解決済み ✅
- ✅ 単一カウンターのみ対応 → 複数カウンター対応完了
- ✅ 設定の永続化未対応 → ConfigManager実装完了
- ✅ グローバルホットキー未実装 → HotkeyManager実装完了
- ✅ StaticFileProvider.ServeFileメソッド欠落 → 実装完了
- ✅ WPF/WinForms名前空間衝突 → エイリアスで解決（一部）
- ✅ サーバー自動起動 → 手動起動に変更
- ✅ カウンター値変更通知 → 削除
- ✅ ブラウザ管理画面 → 削除

---

## 💻 開発環境情報

### 必須環境
- **OS**: Windows 10/11
- **IDE**: Visual Studio 2022
- **.NET**: .NET 8.0
- **ワークロード**: 
  - .NET デスクトップ開発
  - ASP.NET と Web 開発（オプション）

### NuGetパッケージ
```xml
<PackageReference Include="WebSocketSharp-netstandard" Version="1.0.1" />
```

### プロジェクト設定（.csproj）
```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFramework>net8.0-windows</TargetFramework>
  <UseWPF>true</UseWPF>
  <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>

<ItemGroup>
  <None Update="wwwroot\**\*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

---

## 🎮 動作確認方法

### 1. アプリケーション起動
```
1. Visual StudioでF5キー（デバッグ実行）
2. タスクトレイにアイコンが表示される
3. サーバーは停止状態
4. config.jsonが自動生成される
```

### 2. WPF設定画面テスト
```
1. タスクトレイアイコンをダブルクリック
2. モダンなWPF設定画面が表示される
3. サイドバーで各ビューを切り替え
4. 「カウンター管理」でカウンター一覧を確認
5. 「新規カウンター追加」で追加ダイアログが開く
6. 「サーバー設定」で「サーバー起動」ボタンをクリック
7. サーバーが起動し、ホットキーが登録される
8. 「接続情報」でOBS URLを確認・コピー
```

### 3. OBS表示テスト
```
1. サーバーを起動
2. OBSで「ソース追加」→「ブラウザ」
3. URL: http://localhost:8765/obs.html
4. 幅: 800px、高さ: 600px
5. 全カウンターが縦並びで表示される
6. WPF設定画面でボタンを押すと即座に更新される
7. 数値変化時にフラッシュエフェクトが発生
```

### 4. グローバルホットキーテスト
```
1. サーバーを起動（重要！）
2. デフォルトカウンターで動作確認
3. Ctrl+Shift+↑ でカウンター増加
4. Ctrl+Shift+↓ でカウンター減少
5. Ctrl+Shift+R でリセット
6. OBS画面とWPF画面が即座に更新される
7. サーバーを停止するとホットキーが無効化
```

### 5. 設定永続化テスト
```
1. カウンターを追加・編集
2. アプリを終了
3. config.jsonが保存されていることを確認
4. アプリを再起動
5. 前回のカウンターが復元されることを確認
```

---

## 📝 重要な設計決定

### 1. 名前空間設計
```csharp
CounterCounter              // ルート（App.xaml.cs のみ）
├── CounterCounter.Core     // コア機能
├── CounterCounter.Server   // サーバー機能
├── CounterCounter.UI       // UI機能
│   └── CounterCounter.UI.Views  // ビューコンポーネント
└── CounterCounter.Models   // データモデル
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
- **起動時**: サーバー停止、ホットキー未登録
- **手動起動**: ユーザーがボタンをクリック
- **起動処理**: HTTPサーバー → WebSocketサーバー → ホットキー登録
- **停止処理**: ホットキー解除 → WebSocketサーバー停止 → HTTPサーバー停止

### 4. ポート設定
- **HTTPサーバー**: 8765（自動選択、競合時は8766, 8767...）
- **WebSocketサーバー**: HTTPポート+1（例: 8766）

### 5. モダンデザイン方針
- **カラースキーム**:
  - 背景: #0f0f0f（濃い黒）
  - サイドバー: #1a1a1a（ダークグレー）
  - カード: #1a1a1a + ドロップシャドウ
  - アクセント: #00d4ff（シアン）
  - 危険: #ff4757（赤）
  - 成功: #5fec5f（緑）
- **エフェクト**:
  - グラデーションボタン
  - ドロップシャドウ（BlurRadius: 15-20）
  - 角丸（CornerRadius: 6-12）
  - ホバーエフェクト

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
    },
    {
      "Id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "Name": "Death Counter",
      "Value": 12,
      "Color": "#ff0000"
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
      "Action": 1,
      "Modifiers": 6,
      "VirtualKey": 40
    },
    {
      "CounterId": "default",
      "Action": 2,
      "Modifiers": 6,
      "VirtualKey": 82
    }
  ],
  "ServerPort": 8765
}
```

**説明**:
- `Action`: 0=Increment, 1=Decrement, 2=Reset
- `Modifiers`: 2=Ctrl, 4=Shift, 6=Ctrl+Shift
- `VirtualKey`: 38=↑, 40=↓, 82=R

---

## 📚 参考情報

### コードの場所
- **引き継ぎ資料**: `HANDOVER.md`（このファイル）
- **要求仕様書**: `REQUIREMENTS.md`
- **タスク管理表**: `TASKS.md`
- **リードミー**: `README.md`

### 外部リンク
- [WebSocketSharp GitHub](https://github.com/sta/websocket-sharp)
- [RegisterHotKey API](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey)
- [WPF チュートリアル](https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/)

---

## 💡 開発のヒント

### 名前空間の曖昧参照デバッグ手順
1. エラーメッセージで型名を確認（例: `Color`）
2. ファイル先頭の `using` を確認
3. `System.Drawing` と `System.Windows.Media` の両方が含まれているか確認
4. エイリアスを追加:
   ```csharp
   using WpfColor = System.Windows.Media.Color;
   using WpfColorConverter = System.Windows.Media.ColorConverter;
   ```
5. コード内で `Color` → `WpfColor` に置換
6. ビルドして確認

### 新規ファイル作成時のチェックリスト
- [ ] 名前空間を `CounterCounter.XXX` 形式で設定
- [ ] 色を扱う場合は、必ずエイリアスを定義
- [ ] `System.Windows.Forms` を使う場合は `WinForms` エイリアス
- [ ] WPFコントロールを使う場合は `Wpf` プレフィックスのエイリアス

### WebSocketメッセージフォーマット
```javascript
// 初期化メッセージ
{
    "type": "init",
    "counters": [
        { "Id": "default", "Name": "Counter 1", "Value": 0, "Color": "#00ff00" }
    ]
}

// カウンター更新メッセージ
{
    "type": "counter_update",
    "counterId": "default",
    "value": 5,
    "oldValue": 4,
    "changeType": "increment",
    "counter": { "Id": "default", "Name": "Counter 1", "Value": 5, "Color": "#00ff00" }
}
```

---

## 🎉 完成イメージ

最終的に以下ができるアプリ：
1. ✅ 起動すると自動でタスクトレイに常駐
2. ✅ ユーザーが手動でサーバーを起動
3. ✅ グローバルホットキーでゲーム中でもカウンター操作
4. ✅ 複数カウンターを自由に追加・削除・編集
5. ✅ OBSブラウザソースで複数カウンターをリアルタイム表示
6. ✅ スタイリッシュなモダンWPF設定画面
7. ✅ 簡単な設定管理（config.json）
8. ⏳ カウンター毎のホットキー設定（未実装）
9. ⏳ 単一EXEで配布可能（未実装）

---

## 📞 引き継ぎ時の確認事項

次のセッションで開発を続ける場合、以下を確認：
- [x] プロジェクト構造の理解（Core/Server/UI/Models）
- [x] 複数カウンター対応の設計
- [x] 名前空間の整理状況
- [x] モダンUI設計の理解
- [x] サーバー手動起動方式の理解
- [ ] **名前空間の曖昧参照エラーの修正（最優先）**
- [ ] 次の実装対象（ホットキー設定UI or アニメーション）
- [ ] 未解決の問題やエラーの確認

---

## 📈 セッション履歴

### セッション1 (2026-01-04 初回)
- プロジェクト作成
- 基本機能実装（タスクトレイ、HTTP、WebSocket、カウンター状態）
- HTML埋め込み版の実装

### セッション2 (2026-01-04 リファクタリング)
- WebServer.csのリファクタリング（300行→220行）
- HTML/CSS/JSの外部ファイル化
- 名前空間の整理（Core/Server/UI）
- WPF設定画面の実装
- 名前空間の衝突解決（WPF vs WinForms）

### セッション3 (2026-01-04 複数カウンター対応)
- **重大な設計変更**: 単一カウンター → 複数カウンター管理
- データモデルの新規作成（Counter, HotkeySettings, CounterSettings）
- CounterManagerの実装（複数カウンター管理）
- HotkeyManagerの全面書き換え（動的登録対応）
- ConfigManagerの実装（JSON永続化）
- WebServer/API/WebSocketの全面改修
- MainWindowの大幅拡張（カウンター一覧、追加・編集・削除UI）
- CounterEditDialogの実装（カウンター編集ダイアログ）
- TrayIconの更新（設定保存機能追加）
- Web UI（manager.js/obs.js）の複数カウンター対応
- 全エラー・警告の修正（名前空間衝突、StaticFileProvider等）
- 全体進捗85%達成

### セッション4 (2026-01-05 モダンUI実装)
- **UI設計の全面刷新**: タブ → サイドバーナビゲーション
- **サーバー起動方式の変更**: 自動起動 → 手動起動
- **機能削除**: カウンター値変更通知、ブラウザ管理画面
- **機能追加**: ホットキー表示、OBS URL操作ボタン
- モダンデザイン実装:
  - MainWindow: サイドバー + グラデーションボタン
  - CounterManagementView: カード型UI + ホットキー表示
  - ServerSettingsView: サーバー起動/停止制御
  - ConnectionInfoView: URL表示・コピー・ブラウザ起動
- App.xaml.csの簡略化（サーバー自動起動削除）
- TrayIcon.csの簡略化（通知削除）
- 要求仕様書の更新（REQUIREMENTS.md v1.1）
- 全体進捗90%達成
- **課題**: 名前空間の曖昧参照エラーが複数残存

---

**最終更新日**: 2026-01-05  
**次回セッションの最優先タスク**: 名前空間の曖昧参照エラーの完全修正