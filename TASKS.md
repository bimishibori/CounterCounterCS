# カウンター・カウンター タスク管理表

**プロジェクト名**: Counter Counter（カウンター・カウンター）  
**技術スタック**: C# + .NET 8 + WPF  
**最終更新日**: 2026-01-04 (セッション2)

---

## 📊 全体進捗

| フェーズ | 進捗 | 状態 |
|---------|------|------|
| Phase 1: 環境構築 | 100% | ✅ 完了 |
| Phase 2: コア機能実装 | 80% | 🔄 進行中 |
| Phase 3: GUI実装 | 60% | 🔄 進行中 |
| Phase 4: アニメーション | 10% | 🔄 進行中 |
| Phase 5: EXE化・配布 | 0% | ⏳ 未着手 |

**全体進捗: 60%完了**

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
  - [x] `Models/` フォルダ（データモデル・今後使用）
  - [x] `wwwroot/` フォルダ（Webファイル）
  - [x] `wwwroot/css/` フォルダ
  - [x] `wwwroot/js/` フォルダ
  - [x] `Resources/` フォルダ
- [ ] アイコンファイル追加（`Resources/icon.ico`）

### 1-5. 基本ファイル作成
- [x] `Core/CounterState.cs` 作成 ✅
- [x] `Server/WebServer.cs` 作成 ✅
- [x] `Server/WebSocketServer.cs` 作成 ✅
- [x] `Server/ApiHandler.cs` 作成 ✅
- [x] `Server/HtmlContentProvider.cs` 作成 ✅
- [x] `Server/StaticFileProvider.cs` 作成 ✅
- [x] `UI/TrayIcon.cs` 作成 ✅
- [x] `UI/MainWindow.xaml` 作成 ✅
- [x] `UI/MainWindow.xaml.cs` 作成 ✅
- [ ] `Core/HotkeyManager.cs` 作成
- [ ] `Core/ConfigManager.cs` 作成
- [ ] `Models/CounterSettings.cs` 作成
- [ ] `Models/DisplaySettings.cs` 作成

---

## Phase 2: コア機能実装 【80%】

### 2-1. カウンター状態管理 ✅
- [x] `Core/CounterState.cs` 実装
  - [x] カウンター値の保持（プロパティ）
  - [x] `Increment()` メソッド
  - [x] `Decrement()` メソッド
  - [x] `Reset()` メソッド
  - [x] `GetValue()` メソッド
  - [x] `ValueChanged` イベント
  - [x] スレッドセーフ対応（lock使用）

### 2-2. HTTPサーバー実装 ✅
- [x] `Server/WebServer.cs` 実装
  - [x] `HttpListener` 初期化
  - [x] ルーティング処理
  - [x] 静的ファイル配信（wwwroot）
  - [x] APIエンドポイント実装
  - [x] ポート自動選択機能
  - [x] 非同期処理対応

### 2-3. APIエンドポイント実装 ✅
- [x] `GET /` (index.html配信)
- [x] `GET /obs.html` (OBS表示画面)
- [x] `GET /api/counter` (カウンター値取得)
- [x] `POST /api/counter/increment` (カウンター+1)
- [x] `POST /api/counter/decrement` (カウンター-1)
- [x] `POST /api/counter/reset` (リセット)
- [ ] `GET /api/settings` (設定取得)
- [ ] `POST /api/settings` (設定更新)

### 2-4. WebSocket実装 ✅
- [x] `Server/WebSocketServer.cs` 実装
  - [x] WebSocketSharp統合
  - [x] 接続管理
  - [x] `counter_update` イベント送信
  - [ ] `settings_update` イベント送信
  - [x] ブロードキャスト機能

### 2-5. グローバルホットキー実装 ← 次はここ！
- [ ] `Core/HotkeyManager.cs` 実装
  - [ ] Win32 API `RegisterHotKey` 呼び出し
  - [ ] `UnregisterHotKey` 呼び出し
  - [ ] `WndProc` メッセージ処理
  - [ ] ホットキーイベント発火
  - [ ] キー競合検出
  - [ ] デフォルトキー登録

### 2-6. 設定管理実装
- [ ] `Core/ConfigManager.cs` 実装
  - [ ] JSON読み込み (`System.Text.Json`)
  - [ ] JSON保存
  - [ ] デフォルト設定生成
  - [ ] 設定変更検知

### 2-7. タスクトレイ実装 ✅
- [x] `UI/TrayIcon.cs` 実装
  - [x] `NotifyIcon` 初期化
  - [x] アイコン画像設定（仮アイコン）
  - [x] コンテキストメニュー作成
  - [x] 「設定を開く」機能
  - [x] 「管理ページを開く」機能
  - [x] 「OBS URLをコピー」機能
  - [x] 「終了」機能
  - [x] ツールチップ（ポート番号表示）
  - [x] カウンター値変更通知

### 2-8. アプリケーション統合 ✅
- [x] `App.xaml.cs` 修正
  - [x] スタートアップ処理
  - [x] サーバー起動
  - [x] トレイアイコン表示
  - [x] ウィンドウ非表示設定
  - [x] エラーハンドリング（基本）

---

## Phase 3: GUI実装（WPF） 【60%】

### 3-1. MainWindow基本レイアウト ✅
- [x] `UI/MainWindow.xaml` 作成
  - [x] タブコントロール配置
  - [x] ダークテーマ適用
  - [x] ウィンドウサイズ・位置設定

### 3-2. タブ1: カウンター操作 ✅
- [x] カウンター操作UI
  - [x] 現在値表示（TextBlock）
  - [x] 「+」ボタン
  - [x] 「-」ボタン
  - [x] 「Reset」ボタン
- [x] リアルタイム更新機能
- [ ] 初期値設定UI
  - [ ] NumericUpDown または TextBox
- [ ] 起動時動作設定
  - [ ] CheckBox（前回値復元）

### 3-3. タブ2: 接続情報 ✅
- [x] OBS用URL表示
- [x] 「URLをコピー」ボタン
- [x] 使用ポート表示
- [x] サーバー状態表示
- [x] 管理ページを開くボタン
- [ ] 接続クライアント数表示

### 3-4. タブ3: 設定（今後実装）
- [ ] ホットキー設定UI
  - [ ] 各機能のキー表示
  - [ ] 「記録」ボタン
  - [ ] キー入力待機機能
  - [ ] キー競合チェック表示
- [ ] 表示設定UI
  - [ ] フォント選択（ComboBox）
  - [ ] 文字色選択（ColorPicker）
  - [ ] 文字サイズ（Slider）
  - [ ] 背景色選択（ColorPicker）
  - [ ] プレビュー表示
- [ ] 演出設定UI
  - [ ] スライド演出 ON/OFF（CheckBox）
  - [ ] パーティクル演出 ON/OFF（CheckBox）
  - [ ] アニメーション速度（Slider）
  - [ ] リアルタイムプレビュー

### 3-5. MVVM実装（オプション）
- [ ] ViewModelクラス作成
- [ ] INotifyPropertyChanged実装
- [ ] データバインディング設定
- [ ] Command実装

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
  - [x] レスポンシブデザイン
  - [x] ボタンスタイル
  - [x] ホバーエフェクト

### 4-3. 管理画面 JavaScript ✅
- [x] `wwwroot/js/manager.js` 作成
  - [x] WebSocket接続
  - [x] カウンター操作イベント
  - [x] リアルタイム更新
  - [x] 自動再接続機能

### 4-4. OBS表示画面（obs.html） ✅
- [x] `wwwroot/obs.html` 作成
  - [x] シンプルレイアウト
  - [x] カウンター表示エリア

### 4-5. OBS表示画面 CSS ✅
- [x] `wwwroot/css/obs.css` 作成
  - [x] 中央配置
  - [x] 背景透過対応
  - [x] フォント・カラー設定

### 4-6. OBS表示画面 JavaScript ✅
- [x] `wwwroot/js/obs.js` 作成
  - [x] WebSocket接続
  - [x] カウンター値更新
  - [x] 自動再接続機能

---

## Phase 5: アニメーション実装 【10%】

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

## Phase 6: EXE化・配布準備 【0%】

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
- ⚠️ **グローバルホットキー未実装** - 最優先で実装が必要
- ⚠️ **設定の永続化未対応** - 再起動で設定が消える

### 優先度: 中
- ⚠️ **WebSocketSharpの警告** - AddWebSocketServiceのdeprecatedメッセージ
- ⚠️ **アイコンが仮アイコン** - 独自アイコンの作成が必要

### 優先度: 低
- 特になし

---

## 📝 メモ・TODO

### 実装時の注意点
- [x] グローバルホットキーは管理者権限不要で動作するか確認
- [x] WebSocketの自動再接続機能を実装済み
- [x] ポート番号の衝突対策を実装済み
- [x] OBS表示画面の背景透過を確認済み
- [x] WPFウィンドウを完全に非表示にする（タスクバーに出さない）

### C#特有の注意点
- [x] `HttpListener` は管理者権限が必要な場合あり（localhost例外設定）
- [x] WPF + NotifyIcon の組み合わせ（System.Windows.Forms参照必要）
- [ ] 単一ファイル発行時の埋め込みリソースパス問題
- [ ] `RegisterHotKey` の HWnd 取得方法

### リファクタリング完了事項 ✅
- [x] WebServer.cs を分割（300行→220行）
- [x] HTML/CSS/JSの外部ファイル化
- [x] 名前空間の整理（Core/Server/UI）
- [x] 名前空間の衝突解決（WPF vs WinForms）

### 将来的な拡張候補
- 複数カウンター対応
- カスタムテーマ機能
- 効果音連動
- プラグインシステム

---

## 🎯 現在の作業

**現在のフェーズ**: Phase 2 - コア機能実装（80%完了）  
**次のタスク**: グローバルホットキー実装 (`Core/HotkeyManager.cs`)

**最近の成果**: 
- ✅ WebServer.csのリファクタリング完了！
- ✅ HTML/CSS/JSの外部ファイル化完了！
- ✅ 名前空間の整理完了（Core/Server/UI）！
- ✅ WPF設定画面実装完了！
- ✅ 名前空間の衝突解決完了！

---

## 📅 マイルストーン

| マイルストーン | 目標日 | 状態 |
|---------------|--------|------|
| プロトタイプ完成（コア機能のみ） | - | ✅ 完了 |
| GUI完成（WPF設定画面） | - | 🔄 60%完了 |
| グローバルホットキー実装 | 次回 | ⏳ 未着手 |
| 設定永続化実装 | 次回 | ⏳ 未着手 |
| アニメーション実装完了 | 未定 | ⏳ 未着手 |
| 初回リリース (v0.1.0) | 未定 | ⏳ 未着手 |

---

## 🔧 技術メモ

### グローバルホットキー実装例
```csharp
// Win32 API のインポート
[DllImport("user32.dll")]
private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

[DllImport("user32.dll")]
private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

// WPFでのHwnd取得
protected override void OnSourceInitialized(EventArgs e)
{
    base.OnSourceInitialized(e);
    var helper = new WindowInteropHelper(this);
    var source = HwndSource.FromHwnd(helper.Handle);
    source.AddHook(HwndHook);
}

// 使用例
RegisterHotKey(windowHandle, 1, MOD_CONTROL | MOD_SHIFT, VK_UP);
```

### 設定ファイルパス例
```csharp
string configPath = Path.Combine(
    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
    "config.json"
);
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

---

**更新履歴**
- 2026-01-04: C# 版としてタスク管理表作成（セッション1）
- 2026-01-04: リファクタリング完了、WPF実装完了（セッション2）