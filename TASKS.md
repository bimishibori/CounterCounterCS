# カウンター・カウンター タスク管理表

**プロジェクト名**: Counter Counter（カウンター・カウンター）  
**技術スタック**: C# + .NET 8 + WPF  
**最終更新日**: 2026-01-04 (セッション3 - 複数カウンター対応完了)

---

## 📊 全体進捗

| フェーズ | 進捗 | 状態 |
|---------|------|------|
| Phase 1: 環境構築 | 100% | ✅ 完了 |
| Phase 2: コア機能実装 | 100% | ✅ 完了 |
| Phase 3: GUI実装 | 90% | 🔄 ほぼ完了 |
| Phase 4: アニメーション | 10% | 🔄 進行中 |
| Phase 5: EXE化・配布 | 0% | ⏳ 未着手 |

**全体進捗: 85%完了** 🎉

---

## 🎯 重大な設計変更 (セッション3)

### 複数カウンター対応への全面リニューアル

**変更前**: 単一の `CounterState` で1つのカウンターのみ管理  
**変更後**: `CounterManager` で複数カウンターを管理

この変更により、以下の機能が実現：
- ✅ カウンターの自由な追加・削除・編集
- ✅ カウンター毎に名前・色を設定
- ✅ カウンター毎のホットキー設定（UI未実装）
- ✅ OBSで複数カウンターを同時表示

---

## Phase 1: 環境構築 【100%】✅

### 1-1. 開発環境準備
- [x] .NET 8 SDK インストール
- [x] Visual Studio 2022 インストール
  - [x] .NET デスクトップ開発 ワークロード選択
  - [x] ASP.NET と Web 開発 ワークロード選択
- [x] Visual Studio起動確認

### 1-2. プロジェクト作成
- [x] Visual Studio で新規プロジェクト作成
  - [x] テンプレート: 「WPF アプリ (.NET)」
  - [x] プロジェクト名: CounterCounter
  - [x] フレームワーク: .NET 8
- [x] ソリューション構成確認

### 1-3. NuGetパッケージインストール
- [x] `WebSocketSharp-netstandard` インストール
- [x] `System.Text.Json` 確認（標準で含まれる）

### 1-4. プロジェクト構造作成
- [x] フォルダ構造作成
  - [x] `Core/` フォルダ（コア機能）
  - [x] `Server/` フォルダ（サーバー機能）
  - [x] `UI/` フォルダ（UI機能）
  - [x] `Models/` フォルダ（データモデル）
  - [x] `wwwroot/` フォルダ（Webファイル）
  - [x] `wwwroot/css/` フォルダ
  - [x] `wwwroot/js/` フォルダ
  - [x] `Resources/` フォルダ
- [ ] アイコンファイル追加（`Resources/icon.ico`）

### 1-5. 基本ファイル作成
- [x] `Core/CounterManager.cs` 作成 ✅
- [x] `Core/HotkeyManager.cs` 作成 ✅
- [x] `Core/ConfigManager.cs` 作成 ✅
- [x] `Server/WebServer.cs` 作成 ✅
- [x] `Server/WebSocketServer.cs` 作成 ✅
- [x] `Server/ApiHandler.cs` 作成 ✅
- [x] `Server/HtmlContentProvider.cs` 作成 ✅
- [x] `Server/StaticFileProvider.cs` 作成 ✅
- [x] `UI/TrayIcon.cs` 作成 ✅
- [x] `UI/MainWindow.xaml` 作成 ✅
- [x] `UI/MainWindow.xaml.cs` 作成 ✅
- [x] `UI/CounterEditDialog.xaml` 作成 ✅
- [x] `UI/CounterEditDialog.xaml.cs` 作成 ✅
- [x] `Models/Counter.cs` 作成 ✅
- [x] `Models/HotkeySettings.cs` 作成 ✅
- [x] `Models/CounterSettings.cs` 作成 ✅

---

## Phase 2: コア機能実装 【100%】✅

### 2-1. データモデル実装 ✅
- [x] `Models/Counter.cs` 実装
  - [x] Id, Name, Value, Color プロパティ
  - [x] Clone()メソッド
- [x] `Models/HotkeySettings.cs` 実装
  - [x] CounterId, Action, Modifiers, VirtualKey
  - [x] GetDisplayText()メソッド
- [x] `Models/CounterSettings.cs` 実装
  - [x] Counters リスト
  - [x] Hotkeys リスト
  - [x] ServerPort
  - [x] CreateDefault()メソッド

### 2-2. カウンター管理実装 ✅
- [x] `Core/CounterManager.cs` 実装
  - [x] 複数カウンターの保持（Dictionary）
  - [x] `AddCounter()` メソッド
  - [x] `RemoveCounter()` メソッド
  - [x] `GetCounter()` メソッド
  - [x] `GetAllCounters()` メソッド
  - [x] `UpdateCounter()` メソッド
  - [x] `Increment()` メソッド
  - [x] `Decrement()` メソッド
  - [x] `Reset()` メソッド
  - [x] `SetValue()` メソッド
  - [x] `LoadCounters()` メソッド
  - [x] `CounterChanged` イベント
  - [x] スレッドセーフ対応（lock使用）

### 2-3. HTTPサーバー実装 ✅
- [x] `Server/WebServer.cs` 実装
  - [x] `HttpListener` 初期化
  - [x] ルーティング処理
  - [x] 静的ファイル配信（wwwroot）
  - [x] APIエンドポイント統合
  - [x] ポート自動選択機能
  - [x] 非同期処理対応

### 2-4. APIエンドポイント実装 ✅
- [x] `Server/ApiHandler.cs` 実装
  - [x] `GET /api/counters` (全カウンター取得)
  - [x] `POST /api/counters` (カウンター追加)
  - [x] `GET /api/counter/:id` (特定カウンター取得)
  - [x] `PUT /api/counter/:id` (カウンター更新)
  - [x] `DELETE /api/counter/:id` (カウンター削除)
  - [x] `POST /api/counter/:id/increment` (カウンター+1)
  - [x] `POST /api/counter/:id/decrement` (カウンター-1)
  - [x] `POST /api/counter/:id/reset` (リセット)

### 2-5. WebSocket実装 ✅
- [x] `Server/WebSocketServer.cs` 実装
  - [x] WebSocketSharp統合
  - [x] 接続管理
  - [x] `init` メッセージ送信（全カウンター）
  - [x] `counter_update` イベント送信（カウンターID含む）
  - [x] ブロードキャスト機能

### 2-6. グローバルホットキー実装 ✅
- [x] `Core/HotkeyManager.cs` 実装
  - [x] Win32 API `RegisterHotKey` 呼び出し
  - [x] `UnregisterHotKey` 呼び出し
  - [x] `WndProc` メッセージ処理
  - [x] ホットキーイベント発火（HotkeyPressed）
  - [x] キー競合検出（IsHotkeyAlreadyRegistered）
  - [x] 動的ホットキー登録対応
  - [x] カウンターID毎の管理
  - [x] デフォルトキー登録

### 2-7. 設定管理実装 ✅
- [x] `Core/ConfigManager.cs` 実装
  - [x] JSON読み込み (`System.Text.Json`)
  - [x] JSON保存
  - [x] デフォルト設定生成
  - [x] 設定ファイルパス管理

### 2-8. タスクトレイ実装 ✅
- [x] `UI/TrayIcon.cs` 実装
  - [x] `NotifyIcon` 初期化
  - [x] アイコン画像設定（仮アイコン）
  - [x] コンテキストメニュー作成
  - [x] 「設定を開く」機能
  - [x] 「管理ページを開く」機能
  - [x] 「OBS URLをコピー」機能
  - [x] 「設定を保存」機能 ✅
  - [x] 「終了」機能
  - [x] ツールチップ（ポート番号表示）
  - [x] カウンター値変更通知（バルーン）

### 2-9. アプリケーション統合 ✅
- [x] `App.xaml.cs` 修正
  - [x] スタートアップ処理
  - [x] 設定読み込み（ConfigManager）
  - [x] CounterManagerの初期化
  - [x] サーバー起動
  - [x] WebSocket起動
  - [x] トレイアイコン表示
  - [x] ホットキー登録
  - [x] ウィンドウ非表示設定
  - [x] 終了時に設定自動保存
  - [x] エラーハンドリング

---

## Phase 3: GUI実装（WPF） 【90%】🔄

### 3-1. MainWindow基本レイアウト ✅
- [x] `UI/MainWindow.xaml` 作成
  - [x] タブコントロール配置（3タブ）
  - [x] ダークテーマ適用
  - [x] ウィンドウサイズ・位置設定
  - [x] スタイル定義（Button, TextBox, ListBox, TextBlock）

### 3-2. タブ1: カウンター管理 ✅
- [x] カウンター管理UI
  - [x] 「新規カウンター追加」ボタン
  - [x] カウンター一覧（ListBox）
  - [x] 各カウンターに名前・値・色表示
  - [x] 各カウンターに編集・削除ボタン
- [x] リアルタイム更新機能
- [x] カウンター操作エリア
  - [x] 選択中のカウンターで+/-/リセット
- [x] カウンター追加機能（AddCounter_Click）
- [x] カウンター編集機能（EditCounter_Click）
- [x] カウンター削除機能（DeleteCounter_Click）

### 3-3. タブ2: ホットキー設定 ✅
- [x] ホットキー表示UI
  - [x] カウンター毎のグループボックス
  - [x] 増加・減少・リセットのホットキー表示
  - [x] 「未設定」表示対応
- [ ] ホットキー編集機能（未実装）
  - [ ] 編集ボタン
  - [ ] ホットキー設定ダイアログ
  - [ ] キー入力待機
  - [ ] キー競合チェック

### 3-4. タブ3: 接続情報 ✅
- [x] OBS用URL表示
- [x] 「URLをコピー」ボタン
- [x] 管理ページURL表示
- [x] 「ブラウザで開く」ボタン
- [x] サーバー状態表示
- [ ] 接続クライアント数表示（未実装）

### 3-5. CounterEditDialog ✅
- [x] `UI/CounterEditDialog.xaml` 作成
  - [x] カウンター名入力
  - [x] 色選択（5色プリセット）
  - [x] OK/キャンセルボタン
- [x] `UI/CounterEditDialog.xaml.cs` 実装
  - [x] 編集モード対応
  - [x] 新規追加モード対応
  - [x] バリデーション（名前空チェック）

### 3-6. 名前空間衝突解決 ✅
- [x] WPF vs WinForms の衝突解決
  - [x] エイリアス使用（WpfButton, WpfMessageBox等）
  - [x] 全箇所で統一

---

## Phase 4: Web UI実装 【100%】✅

### 4-1. 管理画面（index.html） ✅
- [x] `wwwroot/index.html` 作成
  - [x] 基本レイアウト
  - [x] カウンター操作エリア
  - [x] 接続情報エリア

### 4-2. 管理画面 CSS ✅
- [x] `wwwroot/css/manager.css` 作成
  - [x] ダークテーマ
  - [x] グリッドレイアウト
  - [x] カウンターカード表示
  - [x] レスポンシブデザイン
  - [x] ボタンスタイル
  - [x] ホバーエフェクト

### 4-3. 管理画面 JavaScript ✅
- [x] `wwwroot/js/manager.js` 作成
  - [x] WebSocket接続
  - [x] 複数カウンター対応
  - [x] initメッセージ処理
  - [x] counter_updateメッセージ処理
  - [x] カウンター操作イベント（カウンターID指定）
  - [x] リアルタイム更新
  - [x] 自動再接続機能

### 4-4. OBS表示画面（obs.html） ✅
- [x] `wwwroot/obs.html` 作成
  - [x] シンプルレイアウト
  - [x] カウンター表示エリア

### 4-5. OBS表示画面 CSS ✅
- [x] `wwwroot/css/obs.css` 作成
  - [x] 中央配置
  - [x] 縦並びレイアウト
  - [x] 背景透過対応
  - [x] フォント・カラー設定（カウンター毎）

### 4-6. OBS表示画面 JavaScript ✅
- [x] `wwwroot/js/obs.js` 作成
  - [x] WebSocket接続
  - [x] 複数カウンター対応
  - [x] カウンター値更新
  - [x] フラッシュエフェクト
  - [x] 自動再接続機能

---

## Phase 5: アニメーション実装 【10%】🔄

### 5-1. スライドイン演出
- [ ] CSS Transition 実装（obs.css）
- [ ] 増加時アニメーション（下→上）
- [ ] 減少時アニメーション（上→下）
- [ ] アニメーション速度設定対応

### 5-2. パーティクル演出
- [ ] Canvas初期化（obs.js）
- [ ] パーティクル生成ロジック
- [ ] パーティクルアニメーション
- [ ] requestAnimationFrame 実装
- [ ] ON/OFF切り替え対応

### 5-3. フラッシュエフェクト ✅
- [x] 基本的なフラッシュ実装
- [x] transform: scale(1.2)
- [x] 0.3秒のトランジション

### 5-4. 演出テスト
- [ ] 各アニメーション動作確認
- [ ] パフォーマンス確認（60fps維持）
- [x] OBSでの表示確認（基本動作）

---

## Phase 6: EXE化・配布準備 【0%】⏳

### 6-1. アイコン準備
- [ ] `Resources/icon.ico` 作成（複数サイズ含む）
- [ ] アプリケーションアイコン設定

### 6-2. 発行設定
- [ ] 発行プロファイル作成
  - [ ] ターゲット: win-x64
  - [ ] 単一ファイル: 有効
  - [ ] 自己完結型: 有効
  - [ ] ReadyToRun: 有効（オプション）
  - [ ] トリミング: 有効（慎重に）

### 6-3. ビルドテスト
- [ ] Release ビルド
- [ ] 発行実行
- [ ] EXE動作確認
- [ ] サイズ確認（目標: 30MB以下）

### 6-4. インストーラ作成（オプション）
- [ ] Inno Setup または WiX 導入
- [ ] インストーラスクリプト作成
- [ ] スタートアップ登録オプション
- [ ] アンインストール機能

### 6-5. 配布パッケージ作成
- [ ] README.txt 作成
  - [ ] 使い方説明
  - [ ] システム要件
  - [ ] トラブルシューティング
- [ ] LICENSE.txt 作成
- [ ] ZIP圧縮（or インストーラ）

### 6-6. 動作テスト
- [ ] クリーンなWindows環境でテスト
- [ ] 初回起動テスト
- [ ] OBS連携テスト
- [ ] グローバルホットキーテスト
- [ ] 設定保存・復元テスト
- [ ] 各機能動作確認

---

## 🐛 バグ・課題管理

### 優先度: 高
- ⚠️ **ホットキー設定UIが未実装** - 現在はconfig.json手動編集が必要
- ⚠️ **カウンター毎のホットキー登録機能** - デフォルトカウンターのみ動作

### 優先度: 中
- ⚠️ **アイコンが仮アイコン** - 独自アイコンの作成が必要
- ⚠️ **カウンター並び替え機能なし** - ドラッグ&ドロップ未実装

### 優先度: 低
- WebSocketSharpの警告（pragma directiveで抑制済み）

### 解決済み ✅
- ✅ 単一カウンターのみ対応 → 複数カウンター対応完了
- ✅ 設定の永続化未対応 → ConfigManager実装完了
- ✅ グローバルホットキー未実装 → HotkeyManager実装完了
- ✅ StaticFileProvider.ServeFileメソッド欠落 → 実装完了
- ✅ WPF/WinForms名前空間衝突 → エイリアスで全解決

---

## 📝 メモ・TODO

### 実装時の注意点
- [x] グローバルホットキーは管理者権限不要で動作
- [x] WebSocketの自動再接続機能を実装済み
- [x] ポート番号の衝突対策を実装済み
- [x] OBS表示画面の背景透過を確認済み
- [x] WPFウィンドウを完全に非表示にする（タスクバーに出さない）
- [x] 設定の自動保存（終了時）

### C#特有の注意点
- [x] `HttpListener` は管理者権限が必要な場合あり（localhost例外設定）
- [x] WPF + NotifyIcon の組み合わせ（System.Windows.Forms参照必要）
- [ ] 単一ファイル発行時の埋め込みリソースパス問題
- [x] `RegisterHotKey` の HWnd 取得方法（隠しウィンドウ使用）

### リファクタリング完了事項 ✅
- [x] WebServer.cs を分割（300行→220行）
- [x] HTML/CSS/JSの外部ファイル化
- [x] 名前空間の整理（Core/Server/UI/Models）
- [x] 名前空間の衝突解決（WPF vs WinForms）
- [x] 単一カウンター → 複数カウンター対応

### 将来的な拡張候補
- カウンター並び替え（ドラッグ&ドロップ）
- カスタムテーマ機能
- 効果音連動
- プラグインシステム
- Twitch/YouTube Chat連携

---

## 🎯 現在の作業

**現在のフェーズ**: Phase 3 - GUI実装（90%完了）  
**次のタスク**: ホットキー設定UI実装 (`UI/HotkeyEditDialog.xaml`)

**最近の成果**: 
- ✅ 複数カウンター対応への全面リニューアル完了！
- ✅ データモデル新規作成（Counter, HotkeySettings, CounterSettings）
- ✅ CounterManager実装完了！
- ✅ HotkeyManager動的登録対応完了！
- ✅ ConfigManager実装完了！
- ✅ WebServer/API/WebSocket全面改修完了！
- ✅ MainWindow大幅拡張完了！
- ✅ CounterEditDialog実装完了！
- ✅ Web UI（manager.js/obs.js）複数カウンター対応完了！
- ✅ 全エラー・警告の修正完了！

---

## 📅 マイルストーン

| マイルストーン | 目標日 | 状態 |
|---------------|--------|------|
| プロトタイプ完成（コア機能のみ） | - | ✅ 完了 |
| GUI完成（WPF設定画面） | - | 🔄 90%完了 |
| 複数カウンター対応 | - | ✅ 完了 |
| グローバルホットキー実装 | - | ✅ 完了 |
| 設定永続化実装 | - | ✅ 完了 |
| ホットキー設定UI実装 | 次回 | ⏳ 未着手 |
| アニメーション実装完了 | 未定 | ⏳ 未着手 |
| 初回リリース (v0.1.0) | 未定 | ⏳ 未着手 |

---

## 🔧 技術メモ

### config.json の構造例
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
    }
  ],
  "ServerPort": 8765
}
```

### ホットキー設定UI実装例
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

### 単一ファイル発行コマンド
```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

---

## 📊 セッション履歴

### セッション1 (2026-01-04)
- プロジェクト作成
- 基本機能実装完了
- タスクトレイ常駐動作確認

### セッション2 (2026-01-04)
- WebServer.csリファクタリング（300行→220行）
- HTML/CSS/JSの外部ファイル化
- 名前空間の整理（Core/Server/UI）
- WPF設定画面実装
- 名前空間の衝突解決
- 全体進捗60%達成

### セッション3 (2026-01-04)
- **重大な設計変更**: 単一カウンター → 複数カウンター対応
- データモデル作成（Counter, HotkeySettings, CounterSettings）
- CounterManager実装（複数カウンター管理）
- HotkeyManager全面書き換え（動的登録対応）
- ConfigManager実装（JSON永続化）
- WebServer/API/WebSocket全面改修
- MainWindow大幅拡張（カウンター管理UI）
- CounterEditDialog実装
- Web UI複数カウンター対応
- 全エラー・警告の修正
- 全体進捗85%達成 🎉

---

**更新履歴**
- 2026-01-04: C# 版としてタスク管理表作成（セッション1）
- 2026-01-04: リファクタリング完了、WPF実装完了（セッション2）
- 2026-01-04: 複数カウンター対応完了、85%達成（セッション3）