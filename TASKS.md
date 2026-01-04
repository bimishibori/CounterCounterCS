# カウンター・カウンター タスク管理表

**プロジェクト名**: Counter Counter（カウンター・カウンター）  
**技術スタック**: C# + .NET 6/8 + WPF  
**最終更新日**: 2026-01-04

---

## 📊 全体進捗

| フェーズ | 進捗 | 状態 |
|---------|------|------|
| Phase 1: 環境構築 | 100% | ✅ 完了 |
| Phase 2: コア機能実装 | 10% | 🔄 進行中 |
| Phase 3: GUI実装 | 0% | ⏳ 未着手 |
| Phase 4: アニメーション | 0% | ⏳ 未着手 |
| Phase 5: EXE化・配布 | 0% | ⏳ 未着手 |

---

## Phase 1: 環境構築 【100%】✅

### 1-1. 開発環境準備
- [x] .NET 6 SDK インストール（.NET 8推奨）
- [x] Visual Studio 2022 インストール
  - [x] .NET デスクトップ開発 ワークロード選択
  - [x] ASP.NET と Web 開発 ワークロード選択（オプション）
- [x] Visual Studio起動確認

### 1-2. プロジェクト作成
- [x] Visual Studio で新規プロジェクト作成
  - [x] テンプレート: 「WPF アプリ (.NET)」
  - [x] プロジェクト名: CounterCounter
  - [x] フレームワーク: .NET 6 以上
- [x] ソリューション構成確認

### 1-3. NuGetパッケージインストール
- [x] `WebSocketSharp-netstandard` インストール
- [x] `System.Text.Json` 確認（標準で含まれる）
- [ ] `MaterialDesignThemes` インストール（オプション・後回し）

### 1-4. プロジェクト構造作成
- [x] フォルダ構造作成
  - [x] `Models/` フォルダ
  - [x] `wwwroot/` フォルダ
  - [x] `wwwroot/css/` フォルダ
  - [x] `wwwroot/js/` フォルダ
  - [x] `Resources/` フォルダ
- [ ] アイコンファイル追加（`Resources/icon.ico`）

### 1-5. 基本ファイル作成
- [x] `TrayIcon.cs` 作成 ✅
- [ ] `WebServer.cs` 作成
- [ ] `WebSocketServer.cs` 作成
- [ ] `CounterState.cs` 作成
- [ ] `HotkeyManager.cs` 作成
- [ ] `ConfigManager.cs` 作成
- [ ] `Models/CounterSettings.cs` 作成
- [ ] `Models/DisplaySettings.cs` 作成

---

## Phase 2: コア機能実装 【10%】

### 2-1. カウンター状態管理
- [ ] `CounterState.cs` 実装 ← 次はここ！
  - [ ] カウンター値の保持（プロパティ）
  - [ ] `Increment()` メソッド
  - [ ] `Decrement()` メソッド
  - [ ] `Reset()` メソッド
  - [ ] `GetValue()` メソッド
  - [ ] `ValueChanged` イベント

### 2-2. HTTPサーバー実装
- [ ] `WebServer.cs` 実装
  - [ ] `HttpListener` 初期化
  - [ ] ルーティング処理
  - [ ] 静的ファイル配信（wwwroot）
  - [ ] APIエンドポイント実装
  - [ ] ポート自動選択機能
  - [ ] 非同期処理対応

### 2-3. APIエンドポイント実装
- [ ] `GET /` (index.html配信)
- [ ] `GET /obs.html` (OBS表示画面)
- [ ] `GET /api/counter` (カウンター値取得)
- [ ] `POST /api/counter/increment` (カウンター+1)
- [ ] `POST /api/counter/decrement` (カウンター-1)
- [ ] `POST /api/counter/reset` (リセット)
- [ ] `GET /api/settings` (設定取得)
- [ ] `POST /api/settings` (設定更新)

### 2-4. WebSocket実装
- [ ] `WebSocketServer.cs` 実装
  - [ ] WebSocketSharp統合
  - [ ] 接続管理
  - [ ] `counter_update` イベント送信
  - [ ] `settings_update` イベント送信
  - [ ] ブロードキャスト機能

### 2-5. グローバルホットキー実装
- [ ] `HotkeyManager.cs` 実装
  - [ ] Win32 API `RegisterHotKey` 呼び出し
  - [ ] `UnregisterHotKey` 呼び出し
  - [ ] `WndProc` メッセージ処理
  - [ ] ホットキーイベント発火
  - [ ] キー競合検出
  - [ ] デフォルトキー登録

### 2-6. 設定管理実装
- [ ] `ConfigManager.cs` 実装
  - [ ] JSON読み込み (`System.Text.Json`)
  - [ ] JSON保存
  - [ ] デフォルト設定生成
  - [ ] 設定変更検知

### 2-7. タスクトレイ実装
- [x] `TrayIcon.cs` 実装 ✅
  - [x] `NotifyIcon` 初期化
  - [x] アイコン画像設定（仮アイコン）
  - [x] コンテキストメニュー作成
  - [x] 「設定を開く」機能（仮実装）
  - [x] 「管理ページを開く」機能
  - [x] 「OBS URLをコピー」機能
  - [x] 「再起動」機能（仮実装）
  - [x] 「終了」機能
  - [ ] ツールチップ（ポート番号表示）

### 2-8. アプリケーション統合
- [x] `App.xaml.cs` 修正 ✅
  - [x] スタートアップ処理
  - [ ] サーバー起動
  - [x] トレイアイコン表示
  - [x] ウィンドウ非表示設定
  - [x] エラーハンドリング（基本）

---

## Phase 3: GUI実装（WPF） 【0%】

### 3-1. MainWindow基本レイアウト
- [ ] `MainWindow.xaml` 作成
  - [ ] タブコントロール配置
  - [ ] ダークテーマ適用
  - [ ] ウィンドウサイズ・位置設定

### 3-2. タブ1: カウンター設定
- [ ] カウンター操作UI
  - [ ] 現在値表示（TextBlock）
  - [ ] 「+」ボタン
  - [ ] 「-」ボタン
  - [ ] 「Reset」ボタン
- [ ] 初期値設定UI
  - [ ] NumericUpDown または TextBox
- [ ] 起動時動作設定
  - [ ] CheckBox（前回値復元）

### 3-3. タブ2: 表示設定
- [ ] フォント選択（ComboBox）
- [ ] 文字色選択（ColorPicker）
- [ ] 文字サイズ（Slider）
- [ ] 背景色選択（ColorPicker）
- [ ] プレビュー表示

### 3-4. タブ3: 演出設定
- [ ] スライド演出 ON/OFF（CheckBox）
- [ ] パーティクル演出 ON/OFF（CheckBox）
- [ ] アニメーション速度（Slider）
- [ ] リアルタイムプレビュー

### 3-5. タブ4: ホットキー設定
- [ ] ホットキー設定UI
  - [ ] 各機能のキー表示
  - [ ] 「記録」ボタン
  - [ ] キー入力待機機能
  - [ ] キー競合チェック表示

### 3-6. タブ5: 接続情報
- [ ] OBS用URL表示（TextBox - ReadOnly）
- [ ] 「URLをコピー」ボタン
- [ ] 使用ポート表示
- [ ] 接続クライアント数表示

### 3-7. MVVM実装
- [ ] ViewModelクラス作成
- [ ] INotifyPropertyChanged実装
- [ ] データバインディング設定
- [ ] Command実装

---

## Phase 4: Web UI実装 【0%】

### 4-1. 管理画面（index.html）
- [ ] `wwwroot/index.html` 作成
  - [ ] 基本レイアウト
  - [ ] カウンター操作エリア
  - [ ] 設定エリア
  - [ ] 接続情報エリア

### 4-2. 管理画面 CSS
- [ ] `wwwroot/css/manager.css` 作成
  - [ ] ダークテーマ
  - [ ] レスポンシブデザイン
  - [ ] ボタンスタイル
  - [ ] ホバーエフェクト

### 4-3. 管理画面 JavaScript
- [ ] `wwwroot/js/manager.js` 作成
  - [ ] WebSocket接続
  - [ ] カウンター操作イベント
  - [ ] 設定変更イベント
  - [ ] リアルタイム更新
  - [ ] URLコピー機能

### 4-4. OBS表示画面（obs.html）
- [ ] `wwwroot/obs.html` 作成
  - [ ] シンプルレイアウト
  - [ ] カウンター表示エリア
  - [ ] パーティクル用Canvas

### 4-5. OBS表示画面 CSS
- [ ] `wwwroot/css/obs.css` 作成
  - [ ] 中央配置
  - [ ] 背景透過対応
  - [ ] フォント・カラー設定

### 4-6. OBS表示画面 JavaScript
- [ ] `wwwroot/js/obs.js` 作成
  - [ ] WebSocket接続
  - [ ] カウンター値更新
  - [ ] 設定反映処理

---

## Phase 5: アニメーション実装 【0%】

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

### 5-3. 演出テスト
- [ ] 各アニメーション動作確認
- [ ] パフォーマンス確認（60fps維持）
- [ ] OBSでの表示確認

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
*現在なし*

### 優先度: 中
*現在なし*

### 優先度: 低
*現在なし*

---

## 📝 メモ・TODO

### 実装時の注意点
- グローバルホットキーは管理者権限不要で動作するか確認
- WebSocketの自動再接続機能を忘れずに
- ポート番号の衝突対策を確実に
- OBS表示画面の背景透過を確認
- WPFウィンドウを完全に非表示にする（タスクバーに出さない）

### C#特有の注意点
- `HttpListener` は管理者権限が必要な場合あり（localhost例外設定）
- WPF + NotifyIcon の組み合わせ（System.Windows.Forms参照必要）
- 単一ファイル発行時の埋め込みリソースパス問題
- `RegisterHotKey` の HWnd 取得方法

### 将来的な拡張候補
- 複数カウンター対応
- カスタムテーマ機能
- 効果音連動
- プラグインシステム

---

## 🎯 現在の作業

**現在のフェーズ**: Phase 2 - コア機能実装（10%完了）  
**次のタスク**: CounterState.cs 実装（カウンター状態管理）

**最近の成果**: 
- ✅ タスクトレイアイコン実装完了！
- ✅ アプリケーション起動・常駐動作確認OK！

---

## 📅 マイルストーン

| マイルストーン | 目標日 | 状態 |
|---------------|--------|------|
| プロトタイプ完成（コア機能のみ） | 未定 | ⏳ 未着手 |
| GUI完成 | 未定 | ⏳ 未着手 |
| アニメーション実装完了 | 未定 | ⏳ 未着手 |
| 初回リリース (v0.1.0) | 未定 | ⏳ 未着手 |

---

## 🔧 技術メモ

### グローバルホットキー実装例
```csharp
[DllImport("user32.dll")]
private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

// 使用例
RegisterHotKey(windowHandle, 1, MOD_CONTROL | MOD_SHIFT, VK_UP);
```

### HttpListener のポート例外設定（管理者権限不要化）
```cmd
netsh http add urlacl url=http://localhost:8765/ user=Everyone
```

### 単一ファイル発行コマンド
```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

---

**更新履歴**
- 2026-01-04: C# 版としてタスク管理表作成