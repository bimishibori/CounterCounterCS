# カウンター・カウンター 開発引き継ぎ資料

**作成日**: 2026-01-04  
**最終更新**: 2026-01-04 (セッション3 - 複数カウンター対応完了)  
**プロジェクト名**: Counter Counter（カウンター・カウンター）  
**技術スタック**: C# + .NET 8 + WPF

---

## 📊 現在の進捗状況

### 全体進捗: **85%完了** 🎉

| フェーズ | 進捗 | 状態 |
|---------|------|------|
| Phase 1: 環境構築 | 100% | ✅ 完了 |
| Phase 2: コア機能実装 | 100% | ✅ 完了 |
| Phase 3: GUI実装 | 90% | 🔄 ほぼ完了 |
| Phase 4: アニメーション | 10% | 🔄 一部実装 |
| Phase 5: EXE化・配布 | 0% | ⏳ 未着手 |

---

## 🎯 重要な設計変更（セッション3）

### 複数カウンター対応への全面リニューアル

**変更前**: 単一カウンターのみ対応  
**変更後**: 複数カウンター管理システム

この変更により以下が可能に：
- カウンターを自由に追加・削除・編集
- カウンター毎に名前・色を設定
- カウンター毎に異なるホットキー設定（将来実装）
- OBSで複数カウンターを同時表示

---

## ✅ 完了した機能

### 1. プロジェクト基盤
- [x] Visual Studio 2022 プロジェクト作成
- [x] .NET 8 環境構築
- [x] NuGetパッケージ導入
  - WebSocketSharp-netstandard
  - System.Windows.Forms
- [x] 名前空間整理完了 (Core/Server/UI)
- [x] フォルダ構造の整理

### 2. データモデル (Models/)
- [x] `Counter.cs` - カウンター情報モデル
  - Id, Name, Value, Color
  - Clone()メソッド
- [x] `HotkeySettings.cs` - ホットキー設定
  - カウンターIDとホットキーの紐付け
  - GetDisplayText()でキー表示
- [x] `CounterSettings.cs` - 全体設定
  - カウンターリスト
  - ホットキーリスト
  - サーバーポート設定
  - CreateDefault()でデフォルト設定生成

### 3. コア機能 (Core/)
- [x] `CounterManager.cs` 実装完了
  - 複数カウンターの管理
  - AddCounter / RemoveCounter / UpdateCounter
  - Increment / Decrement / Reset / SetValue
  - スレッドセーフな値管理
  - CounterChangedイベント
  - LoadCounters()で設定読み込み
- [x] `HotkeyManager.cs` 実装完了
  - Win32 API `RegisterHotKey` / `UnregisterHotKey`
  - 動的ホットキー登録
  - カウンターID毎のホットキー管理
  - ホットキー競合チェック
  - HotkeyPressedイベント
- [x] `ConfigManager.cs` 実装完了
  - JSON設定ファイル読み書き
  - config.json の管理
  - デフォルト設定生成

### 4. サーバー機能 (Server/)
- [x] `WebServer.cs` 実装完了
  - HttpListenerによるローカルサーバー
  - ポート自動選択（8765から順に試行）
  - 静的ファイル配信（wwwroot/）
  - ルーティング処理の分離
  - CORS対応
- [x] `ApiHandler.cs` 実装完了
  - **GET `/api/counters`** - 全カウンター取得
  - **POST `/api/counters`** - カウンター追加
  - **GET `/api/counter/:id`** - 特定カウンター取得
  - **PUT `/api/counter/:id`** - カウンター更新
  - **DELETE `/api/counter/:id`** - カウンター削除
  - **POST `/api/counter/:id/increment`** - カウンター増加
  - **POST `/api/counter/:id/decrement`** - カウンター減少
  - **POST `/api/counter/:id/reset`** - リセット
- [x] `WebSocketServer.cs` 実装完了
  - WebSocketSharpによるリアルタイム通信
  - HTTPポート+1で自動起動（例: 8766）
  - カウンター変更時の即時ブロードキャスト
  - 接続時に全カウンター送信（initメッセージ）
  - counter_updateメッセージ（カウンターID含む）
- [x] `HtmlContentProvider.cs` 実装完了
  - HTML生成管理
  - WebSocketポート番号の埋め込み
- [x] `StaticFileProvider.cs` 実装完了
  - wwwrootからのファイル読み込み
  - Content-Type自動判定
  - ServeFile()メソッド実装

### 5. UI機能 (UI/)
- [x] `TrayIcon.cs` 実装完了
  - NotifyIconによる常駐
  - コンテキストメニュー
  - 「設定を開く」→ WPFウィンドウ表示
  - 「管理ページを開く」→ ブラウザ起動
  - 「OBS URLをコピー」→ クリップボードコピー
  - 「設定を保存」→ config.json保存
  - 「終了」→ アプリケーション終了
  - ツールチップ（ポート番号表示）
  - カウンター値の変更通知（バルーン）
- [x] `MainWindow.xaml` 実装完了
  - ダークテーマWPF設定画面
  - タブ構成（3タブ）
    1. **カウンター管理タブ**
       - カウンター一覧表示（リスト）
       - 新規カウンター追加ボタン
       - 各カウンターに編集・削除ボタン
       - 選択中のカウンター操作（+/-/リセット）
    2. **ホットキー設定タブ**
       - カウンター毎のホットキー表示
       - グループボックスで整理
    3. **接続情報タブ**
       - OBS URL表示＆コピー機能
       - 管理ページURL表示＆ブラウザ起動
       - サーバー情報表示
  - リアルタイムカウンター表示
  - 閉じても非表示化（終了しない）
  - 名前空間衝突の完全解決
- [x] `CounterEditDialog.xaml` 実装完了
  - カウンター追加・編集ダイアログ
  - 名前入力
  - 色選択（5色プリセット）
  - OK/キャンセルボタン

### 6. Webインターフェース (wwwroot/)
- [x] `index.html` - 管理画面（ブラウザ版）
- [x] `obs.html` - OBS表示画面
- [x] `css/manager.css` - 管理画面スタイル
  - グリッドレイアウト
  - カウンターカード表示
  - ダークテーマ
- [x] `css/obs.css` - OBS表示スタイル
  - 縦並びレイアウト
  - フラッシュアニメーション
- [x] `js/manager.js` - 管理画面ロジック
  - 複数カウンター対応
  - WebSocket接続
  - 自動再接続
  - カウンター毎の操作ボタン
- [x] `js/obs.js` - OBS表示ロジック
  - 複数カウンター表示
  - リアルタイム更新
  - フラッシュエフェクト

### 7. ビルド設定
- [x] .csprojの設定完了
  - wwwrootフォルダの自動コピー設定
  - UseWPF / UseWindowsForms 有効化

---

## 🔧 現在のシステム構成

```
CounterCounter.exe
├─ タスクトレイ常駐 ✅
├─ HTTPサーバー (Port: 8765) ✅
├─ WebSocketサーバー (Port: 8766) ✅
├─ 複数カウンター管理 ✅
├─ グローバルホットキー ✅
├─ 設定の永続化 (config.json) ✅
├─ WPF設定画面 ✅
└─ カウンター追加・編集・削除 ✅

管理画面
├─ WPF版 (設定画面) ✅
└─ ブラウザ版 (http://localhost:8765/) ✅

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
│   ├── WebServer.cs               # HTTPサーバー
│   └── WebSocketServer.cs         # WebSocketサーバー
├── UI/                             # UI機能
│   ├── MainWindow.xaml            # WPF設定画面
│   ├── MainWindow.xaml.cs         # 設定画面ロジック
│   ├── CounterEditDialog.xaml     # カウンター編集ダイアログ
│   ├── CounterEditDialog.xaml.cs  # ダイアログロジック
│   └── TrayIcon.cs                # タスクトレイ管理
├── Models/                         # データモデル
│   ├── Counter.cs                 # カウンター情報
│   ├── HotkeySettings.cs          # ホットキー設定
│   └── CounterSettings.cs         # 全体設定
├── wwwroot/                        # Webファイル
│   ├── index.html                 # 管理画面（ブラウザ版）
│   ├── obs.html                   # OBS表示画面
│   ├── css/
│   │   ├── manager.css            # 管理画面スタイル
│   │   └── obs.css                # OBS表示スタイル
│   └── js/
│       ├── manager.js             # 管理画面ロジック
│       └── obs.js                 # OBS表示ロジック
├── Resources/                      # リソースファイル（今後使用）
├── App.xaml                       # WPFアプリケーション定義
├── App.xaml.cs                    # アプリケーション起動処理
├── CounterCounter.csproj          # プロジェクト設定
└── config.json                    # 設定ファイル（実行時生成）
```

---

## 🎯 次に実装すべき機能

### 優先度：高 🔥

#### 1. ホットキー設定UI
- [ ] カウンター毎のホットキー設定ダイアログ
- [ ] キー入力待機機能
- [ ] キー競合表示
- [ ] 設定保存機能

**実装方針**:
- `UI/HotkeyEditDialog.xaml` を作成
- キーボードイベントをキャプチャ
- `HotkeyManager.RegisterHotkey()` を呼び出し
- 成功時に `CounterSettings.Hotkeys` に追加

#### 2. OBSレイアウトカスタマイズ
- [ ] 横並び・縦並び・グリッド表示選択
- [ ] カウンター毎の表示ON/OFF
- [ ] フォントサイズ・色のカスタマイズ

### 優先度：中

#### 3. アニメーション強化
- [ ] スライドイン演出（上下方向）
- [ ] パーティクルエフェクト（Canvas）
- [ ] アニメーション速度設定対応

#### 4. カウンター機能拡張
- [ ] カウンター並び替え（ドラッグ&ドロップ）
- [ ] カウンター値の直接入力
- [ ] 最小値・最大値の設定

### 優先度：低

#### 5. アイコンの作成
- [ ] `Resources/icon.ico` 作成
- [ ] 複数サイズのアイコン含む
- [ ] TrayIconに適用

#### 6. 単体テストの追加
- [ ] CounterManagerのテスト
- [ ] ApiHandlerのテスト
- [ ] HotkeyManagerのテスト

---

## 🐛 既知の問題・制限事項

### 解決済み
- ✅ ポート競合エラー → HTTPとWebSocketでポート分離（8765, 8766）
- ✅ Application型のあいまいな参照 → エイリアス使用
- ✅ HTML/CSS/JSが埋め込みコード → 外部ファイル化完了
- ✅ 名前空間の衝突 → Core/Server/UIに分離
- ✅ 単一カウンターのみ → 複数カウンター対応完了
- ✅ StaticFileProvider.ServeFileメソッド欠落 → 実装完了
- ✅ WPF/WinForms名前空間衝突 → エイリアスで全解決
- ✅ WebSocketSharp Obsolete警告 → pragma directiveで抑制

### 未解決
- ⚠️ ホットキー設定UIが未実装（現在はconfig.json手動編集が必要）
- ⚠️ アイコンが仮アイコン（SystemIcons.Application）
- ⚠️ カウンターの並び替え機能なし

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
3. ツールチップに "HTTP:8765 WS:8766" と表示される
4. config.jsonが自動生成される
```

### 2. WPF設定画面テスト
```
1. タスクトレイアイコンをダブルクリック
2. WPF設定画面が表示される
3. 「カウンター管理」タブでカウンター一覧を確認
4. 「新規カウンター追加」ボタンで追加ダイアログが開く
5. 名前と色を設定してOKで追加
6. カウンターの編集・削除が動作する
7. 選択中のカウンターで+/-/リセットが動作する
8. 「ホットキー設定」タブでホットキー表示を確認
9. 「接続情報」タブでURL表示＆コピーができる
10. ウィンドウを閉じても終了せず非表示化される
```

### 3. ブラウザ管理画面テスト
```
1. タスクトレイアイコンを右クリック
2. 「管理ページを開く」をクリック
3. ブラウザで http://localhost:8765/ が開く
4. 「接続中」と表示される
5. 全カウンターがグリッド表示される
6. 各カウンターの+/-/リセットボタンが動作する
7. カウンターがリアルタイム更新される
```

### 4. OBS表示テスト
```
1. ブラウザで http://localhost:8765/obs.html を開く
2. 全カウンターが縦並びで表示される
3. WPFまたはブラウザ管理画面でボタンを押すと即座に更新される
4. 数値変化時にフラッシュエフェクトが発生
5. カウンター毎に設定した色で表示される
```

### 5. グローバルホットキーテスト
```
1. デフォルトカウンターで動作確認
2. Ctrl+Shift+↑ でカウンター増加
3. Ctrl+Shift+↓ でカウンター減少
4. Ctrl+Shift+R でリセット
5. OBS画面とWPF画面が即座に更新される
```

### 6. 設定永続化テスト
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
├── CounterCounter.Core     // コア機能（カウンター・ホットキー・設定管理）
├── CounterCounter.Server   // サーバー機能（HTTP/WebSocket/API）
├── CounterCounter.UI       // UI機能（WPF/TrayIcon/ダイアログ）
└── CounterCounter.Models   // データモデル（Counter/Settings）
```

**理由**: 
- 責任の明確な分離
- 名前空間の衝突回避（WPF vs WinForms）
- 拡張性の確保

### 2. ポート設定
- **HTTPサーバー**: 8765（自動選択、競合時は8766, 8767...）
- **WebSocketサーバー**: HTTPポート+1（例: 8766）

**理由**: 同一ポートでHTTPとWebSocketの競合を回避

### 3. スレッドセーフ設計
- `CounterManager` は `lock` でスレッドセーフを保証
- 複数クライアントからの同時アクセスに対応

### 4. イベント駆動アーキテクチャ
- カウンター変更時にイベント発火
- WebSocketが自動的に全クライアントにブロードキャスト
- 疎結合で拡張しやすい設計

### 5. エイリアスの使用
```csharp
using WinForms = System.Windows.Forms;
using WpfClipboard = System.Windows.Clipboard;
using WpfMessageBox = System.Windows.MessageBox;
using WpfButton = System.Windows.Controls.Button;
using WpfGroupBox = System.Windows.Controls.GroupBox;
using WpfOrientation = System.Windows.Controls.Orientation;
using WpfStackPanel = System.Windows.Controls.StackPanel;
using WpfTextBlock = System.Windows.Controls.TextBlock;
```

**理由**: WPFとWinFormsの名前空間衝突を完全回避

### 6. 複数カウンター設計
- カウンターはGUID（string）でID管理
- カウンター毎に独立した設定（名前・色・値）
- ホットキーはカウンターIDと紐付け
- デフォルトカウンターは "default" ID

---

## 🔄 次のセッションで実装すべきこと

### 最優先タスク（順番に）

1. **ホットキー設定UI** (`UI/HotkeyEditDialog.xaml`)
   - カウンター毎のホットキー設定ダイアログ
   - キー入力待機機能
   - 約80-120行のXAML + C#

2. **アニメーション強化** (`wwwroot/css/obs.css`, `wwwroot/js/obs.js`)
   - スライドインアニメーション
   - パーティクルエフェクト
   - 約50-80行のCSS/JS

3. **アイコン作成** (`Resources/icon.ico`)
   - 専用アイコンの作成
   - TrayIconへの適用

4. **EXE化** (発行設定)
   - 単一ファイルEXE化
   - 動作テスト

### 実装の優先順位
```
ホットキー設定UI → アニメーション強化 → アイコン作成 → EXE化
```

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
- **引き継ぎ資料**: `HANDOVER.md`
- **要求仕様書**: `REQUIREMENTS.md`
- **タスク管理表**: `TASKS.md`

### 外部リンク
- [WebSocketSharp GitHub](https://github.com/sta/websocket-sharp)
- [RegisterHotKey API](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey)
- [WPF チュートリアル](https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/)

---

## 💡 開発のヒント

### ホットキー設定UI実装時の注意
```csharp
// PreviewKeyDown イベントでキー入力をキャプチャ
protected override void OnPreviewKeyDown(KeyEventArgs e)
{
    uint modifiers = 0;
    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) modifiers |= 0x0002;
    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) modifiers |= 0x0004;
    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) modifiers |= 0x0001;
    
    uint vk = (uint)KeyInterop.VirtualKeyFromKey(e.Key);
    
    // ホットキー登録を試行
    bool success = _hotkeyManager.RegisterHotkey(counterId, action, modifiers, vk);
}
```

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
2. ✅ グローバルホットキーでゲーム中でもカウンター操作
3. ✅ 複数カウンターを自由に追加・削除・編集
4. ✅ OBSブラウザソースで複数カウンターをリアルタイム表示
5. ✅ スタイリッシュなWPF設定画面
6. ✅ 簡単な設定管理（config.json）
7. ⏳ カウンター毎のホットキー設定（未実装）
8. ⏳ 単一EXEで配布可能（未実装）

---

## 📞 引き継ぎ時の確認事項

次のセッションで開発を続ける場合、以下を確認：
- [x] プロジェクト構造の理解（Core/Server/UI/Models）
- [x] 複数カウンター対応の設計
- [x] 名前空間の整理状況
- [x] wwwrootフォルダの配置
- [x] config.jsonの構造
- [ ] 次の実装対象（ホットキー設定UI）
- [ ] 未解決の問題やエラー

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