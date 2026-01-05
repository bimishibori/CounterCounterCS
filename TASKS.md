# カウンター・カウンター タスク管理表

**プロジェクト名**: Counter Counter（カウンター・カウンター）  
**技術スタック**: C# + .NET 8 + WPF  
**最終更新日**: 2026-01-05 (セッション4 - モダンUI実装完了)

---

## 📊 全体進捗

| フェーズ | 進捗 | 状態 |
|---------|------|------|
| Phase 1: 環境構築 | 100% | ✅ 完了 |
| Phase 2: コア機能実装 | 100% | ✅ 完了 |
| Phase 3: GUI実装 | 95% | 🔄 ほぼ完了 |
| Phase 4: アニメーション | 10% | 🔄 進行中 |
| Phase 5: EXE化・配布 | 0% | ⏳ 未着手 |

**全体進捗: 90%完了** 🎉

---

## 🎯 重大な設計変更

### セッション3: 複数カウンター対応への全面リニューアル
**変更前**: 単一の `CounterState` で1つのカウンターのみ管理  
**変更後**: `CounterManager` で複数カウンターを管理

### セッション4: モダンUI + サーバー手動起動
**変更前**: タブUI、サーバー自動起動、通知あり、ブラウザ管理画面あり  
**変更後**: サイドバーUI、サーバー手動起動、通知なし、WPF設定画面のみ

---

## 🔴 最優先タスク（次回セッション）

### 1. 名前空間の曖昧参照エラーの完全修正 【緊急】
- [ ] `MainWindow.xaml.cs` の残存エラー修正
- [ ] `CounterManagementView.xaml.cs` の残存エラー修正
- [ ] 新規作成ファイルのエイリアス確認
- [ ] ビルドエラー 0 達成

**エラー内容**:
```
'Color' は、'System.Drawing.Color' と 'System.Windows.Media.Color' 間のあいまいな参照です
'ColorConverter' は、'System.Drawing.ColorConverter' と 'System.Windows.Media.ColorConverter' 間のあいまいな参照です
```

**解決方法**: 必ずファイル先頭でエイリアスを定義
```csharp
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;
```

---

## Phase 1: 環境構築 【100%】✅

### 1-1. 開発環境準備
- [x] .NET 8 SDK インストール
- [x] Visual Studio 2022 インストール
- [x] Visual Studio起動確認

### 1-2. プロジェクト作成
- [x] Visual Studio で新規プロジェクト作成
- [x] ソリューション構成確認

### 1-3. NuGetパッケージインストール
- [x] `WebSocketSharp-netstandard` インストール
- [x] `System.Text.Json` 確認（標準で含まれる）

### 1-4. プロジェクト構造作成
- [x] フォルダ構造作成（Core/Server/UI/Models）
- [ ] アイコンファイル追加（`Resources/icon.ico`）

### 1-5. 基本ファイル作成
- [x] 全ての基本ファイル作成完了

---

## Phase 2: コア機能実装 【100%】✅

### 2-1. データモデル実装 ✅
- [x] `Models/Counter.cs` 実装
- [x] `Models/HotkeySettings.cs` 実装
- [x] `Models/CounterSettings.cs` 実装

### 2-2. カウンター管理実装 ✅
- [x] `Core/CounterManager.cs` 実装
  - [x] 複数カウンター管理
  - [x] AddCounter(Counter) / AddCounter(name, color)
  - [x] スレッドセーフ対応

### 2-3. HTTPサーバー実装 ✅
- [x] `Server/WebServer.cs` 実装
  - [x] ポート自動選択機能
  - [x] 非同期処理対応

### 2-4. APIエンドポイント実装 ✅
- [x] `Server/ApiHandler.cs` 実装
  - [x] 全エンドポイント実装完了

### 2-5. WebSocket実装 ✅
- [x] `Server/WebSocketServer.cs` 実装
  - [x] 初期化メッセージ送信
  - [x] カウンター更新イベント送信

### 2-6. グローバルホットキー実装 ✅
- [x] `Core/HotkeyManager.cs` 実装
  - [x] 動的ホットキー登録対応
  - [x] カウンターID毎の管理

### 2-7. 設定管理実装 ✅
- [x] `Core/ConfigManager.cs` 実装
  - [x] JSON読み込み・保存

### 2-8. タスクトレイ実装 ✅
- [x] `UI/TrayIcon.cs` 実装
  - [x] コンテキストメニュー作成
  - [x] 通知機能削除

### 2-9. アプリケーション統合 ✅
- [x] `App.xaml.cs` 修正
  - [x] サーバー自動起動削除
  - [x] 終了時に設定自動保存

---

## Phase 3: GUI実装（WPF） 【95%】🔄

### 3-1. MainWindow モダンデザイン ✅
- [x] `UI/MainWindow.xaml` 作成
  - [x] サイドバーナビゲーション
  - [x] グラデーションボタン
  - [x] ダークテーマ (#0f0f0f)
  - [x] ドロップシャドウエフェクト
  - [x] サーバー状態インジケーター

- [x] `UI/MainWindow.xaml.cs` 作成
  - [x] ビュー切り替え機能
  - [x] サーバー起動/停止制御
  - [x] ホットキー管理統合
  - [ ] 名前空間の曖昧参照エラー修正（残存）

### 3-2. カウンター管理ビュー ✅
- [x] `UI/Views/CounterManagementView.xaml` 作成
  - [x] カード型UI
  - [x] ホットキー表示機能
  - [x] カウンター操作ボタン

- [x] `UI/Views/CounterManagementView.xaml.cs` 作成
  - [x] カウンター一覧表示
  - [x] 追加・編集・削除機能
  - [ ] 名前空間の曖昧参照エラー修正（残存）

### 3-3. サーバー設定ビュー ✅
- [x] `UI/Views/ServerSettingsView.xaml` 作成
  - [x] サーバー起動/停止ボタン
  - [x] ポート設定UI
  - [x] サーバー情報表示

- [x] `UI/Views/ServerSettingsView.xaml.cs` 作成
  - [x] イベント駆動設計
  - [x] ServerStartRequested イベント
  - [x] ServerStopRequested イベント

### 3-4. 接続情報ビュー ✅
- [x] `UI/Views/ConnectionInfoView.xaml` 作成
  - [x] OBS URL表示
  - [x] ブラウザで開くボタン
  - [x] URLをコピーボタン

- [x] `UI/Views/ConnectionInfoView.xaml.cs` 作成
  - [x] URL表示機能
  - [x] コピー機能
  - [x] ブラウザ起動機能

### 3-5. CounterEditDialog ✅
- [x] `UI/CounterEditDialog.xaml` 作成
  - [x] シンプルデザイン
  - [x] 色選択（5色プリセット）

- [x] `UI/CounterEditDialog.xaml.cs` 実装
  - [x] 3つのコンストラクタ
  - [x] CounterName / CounterColor プロパティ

### 3-6. 名前空間の整理 🔄
- [x] WPF vs WinForms の衝突解決（一部）
- [ ] Color / ColorConverter の曖昧参照解決（残存）
- [ ] 全ファイルでエイリアス統一

---

## Phase 4: Web UI実装 【100%】✅

### 4-1. OBS表示画面（obs.html） ✅
- [x] `wwwroot/obs.html` 作成
- [x] `wwwroot/css/obs.css` 作成
  - [x] 縦並びレイアウト
  - [x] フラッシュアニメーション
- [x] `wwwroot/js/obs.js` 作成
  - [x] 複数カウンター対応
  - [x] WebSocket接続
  - [x] リアルタイム更新

### 4-2. 管理画面（使用されていない） ✅
- [x] `wwwroot/index.html` 作成（削除予定）
- [x] `wwwroot/css/manager.css` 作成（削除予定）
- [x] `wwwroot/js/manager.js` 作成（削除予定）

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

### 優先度: 緊急 🔥
- ⚠️ **名前空間の曖昧参照エラーが複数残存** - ビルドに影響
  - `MainWindow.xaml.cs` の一部
  - `CounterManagementView.xaml.cs` の一部
  - エイリアスの追加漏れ

### 優先度: 高
- ⚠️ **ホットキー設定UIが未実装** - 現在はconfig.json手動編集が必要
- ⚠️ **アイコンが仮アイコン** - 独自アイコンの作成が必要
- ⚠️ **サーバー起動/停止の動作確認** - 実機テストが必要

### 優先度: 中
- ⚠️ **カウンター並び替え機能なし** - ドラッグ&ドロップ未実装
- ⚠️ **アニメーション未実装** - スライドイン、パーティクル

### 優先度: 低
- WebSocketSharpの警告（pragma directiveで抑制済み）
- 使用されていないファイル（manager.html/css/js）

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

## 📝 メモ・TODO

### 実装時の注意点
- [x] グローバルホットキーは管理者権限不要で動作
- [x] WebSocketの自動再接続機能を実装済み
- [x] ポート番号の衝突対策を実装済み
- [x] OBS表示画面の背景透過を確認済み
- [x] WPFウィンドウを完全に非表示にする（タスクバーに出さない）
- [x] 設定の自動保存（終了時）
- [ ] 名前空間の曖昧参照エラーの完全修正（最優先）

### C#特有の注意点
- [x] `HttpListener` は管理者権限が必要な場合あり（localhost例外設定）
- [x] WPF + NotifyIcon の組み合わせ（System.Windows.Forms参照必要）
- [x] `System.Drawing` と `System.Windows.Media` の名前空間衝突に注意
- [ ] 単一ファイル発行時の埋め込みリソースパス問題
- [x] `RegisterHotKey` の HWnd 取得方法（隠しウィンドウ使用）

### リファクタリング完了事項 ✅
- [x] WebServer.cs を分割（300行→220行）
- [x] HTML/CSS/JSの外部ファイル化
- [x] 名前空間の整理（Core/Server/UI/Models）
- [x] 名前空間の衝突解決（WPF vs WinForms）- 一部
- [x] 単一カウンター → 複数カウンター対応
- [x] タブUI → サイドバーナビゲーション
- [x] サーバー自動起動 → 手動起動

### 将来的な拡張候補
- カウンター並び替え（ドラッグ&ドロップ）
- カスタムテーマ機能
- 効果音連動
- プラグインシステム
- Twitch / YouTube Chat連携

---

## 🎯 現在の作業

**現在のフェーズ**: Phase 3 - GUI実装（95%完了）  
**次のタスク**: 名前空間の曖昧参照エラーの完全修正（最優先）

**最近の成果**: 
- ✅ モダンUI設計の全面実装完了！
- ✅ サイドバーナビゲーション実装完了！
- ✅ カウンター管理ビュー実装完了（ホットキー表示機能付き）！
- ✅ サーバー設定ビュー実装完了（手動起動対応）！
- ✅ 接続情報ビュー実装完了（URL操作ボタン付き）！
- ✅ App.xaml.cs簡略化完了！
- ✅ TrayIcon.cs簡略化完了（通知削除）！
- ⚠️ 名前空間の曖昧参照エラーが複数残存（要修正）

---

## 📅 マイルストーン

| マイルストーン | 目標日 | 状態 |
|---------------|--------|------|
| プロトタイプ完成（コア機能のみ） | - | ✅ 完了 |
| GUI完成（WPF設定画面） | - | 🔄 95%完了 |
| 複数カウンター対応 | - | ✅ 完了 |
| グローバルホットキー実装 | - | ✅ 完了 |
| 設定永続化実装 | - | ✅ 完了 |
| モダンUI実装 | - | ✅ 完了 |
| 名前空間の曖昧参照エラー修正 | 次回 | ⏳ 最優先 |
| ホットキー設定UI実装 | 未定 | ⏳ 未着手 |
| アニメーション実装完了 | 未定 | ⏳ 未着手 |
| 初回リリース (v0.1.0) | 未定 | ⏳ 未着手 |

---

## 🔧 技術メモ

### エイリアス定義テンプレート
```csharp
// WPF関連
using WpfButton = System.Windows.Controls.Button;
using WpfMessageBox = System.Windows.MessageBox;
using WpfUserControl = System.Windows.Controls.UserControl;
using WpfClipboard = System.Windows.Clipboard;

// 色関連（最重要！）
using WpfColor = System.Windows.Media.Color;
using WpfColorConverter = System.Windows.Media.ColorConverter;
using WpfBrush = System.Windows.Media.Brush;
using WpfSolidColorBrush = System.Windows.Media.SolidColorBrush;

// WinForms関連
using WinForms = System.Windows.Forms;
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
- 全体進捗85%達成

### セッション4 (2026-01-05)
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

**更新履歴**
- 2026-01-04: C# 版としてタスク管理表作成（セッション1）
- 2026-01-04: リファクタリング完了、WPF実装完了（セッション2）
- 2026-01-04: 複数カウンター対応完了、85%達成（セッション3）
- 2026-01-05: モダンUI実装完了、90%達成（セッション4）
  - 次回最優先: 名前空間の曖昧参照エラーの完全修正