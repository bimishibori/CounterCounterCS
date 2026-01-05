# カウンター・カウンター タスク管理表

**プロジェクト名**: Counter Counter（カウンター・カウンター）  
**技術スタック**: C# + .NET 8 + WPF  
**最終更新日**: 2026-01-05 (セッション5 - UIフォルダ整理 & 追加仕様確定)

---

## 📊 全体進捗

| フェーズ | 進捗 | 状態 |
|---------|------|------|
| Phase 1: 環境構築 | 100% | ✅ 完了 |
| Phase 2: コア機能実装 | 100% | ✅ 完了 |
| Phase 3: GUI実装 | 90% | 🔄 ほぼ完了 |
| Phase 4: アニメーション | 10% | 🔄 進行中 |
| Phase 5: EXE化・配布 | 0% | ⏳ 未着手 |

**全体進捗: 92%完了**

---

## 🎯 重大な設計変更

### セッション3: 複数カウンター対応への全面リニューアル
**変更前**: 単一の `CounterState` で1つのカウンターのみ管理  
**変更後**: `CounterManager` で複数カウンターを管理

### セッション4: モダンUI + サーバー手動起動
**変更前**: タブUI、サーバー自動起動、通知あり、ブラウザ管理画面あり  
**変更後**: サイドバーUI、サーバー手動起動、通知なし、WPF設定画面のみ

### セッション5: UIフォルダ整理 + 追加仕様確定
**フォルダ整理**:
- `UI/Dialogs/` 新設（CounterEditDialog移動）
- `UI/Infrastructure/` 新設（TrayIcon移動）
- 不要ファイル削除予定（manager.css/js, index.html）

**追加仕様**:
1. CounterEditDialog: カラーピッカー & ホットキー設定機能
2. ServerSettingsView: トグルボタン化、起動中はポート変更不可
3. App起動: 画面非表示でタスクトレイのみ
4. TrayIcon: メニュー変更（保存削除、サーバー起動/停止追加）
5. 設定自動保存

---

## 🔴 最優先タスク（次回セッション）

### 1. CounterEditDialog の拡張実装【最優先】
- [ ] **カラーピッカーの実装**
  - [ ] `System.Windows.Forms.ColorDialog` の統合
  - [ ] カラーコード（#RRGGBB）の取得・保存
  - [ ] プリセットボタンの削除
  - [ ] XAMLの更新
  - [ ] コードビハインドの更新
  
- [ ] **ホットキー設定機能の追加**
  - [ ] 増加キー設定UIの追加
  - [ ] 減少キー設定UIの追加
  - [ ] 「記録」ボタンの実装
  - [ ] キー入力待機処理
  - [ ] キー競合チェック機能
  - [ ] HotkeySettings への保存処理

### 2. ServerSettingsView の変更【最優先】
- [ ] **トグルボタンの実装**
  - [ ] 起動/停止ボタンを1つに統合
  - [ ] ボタンラベルの動的変更
  - [ ] 状態に応じたスタイル変更
  
- [ ] **ポート番号制御**
  - [ ] サーバー起動中はTextBox無効化
  - [ ] 停止中のみ編集可能

### 3. App.xaml.cs の修正【最優先】
- [ ] **起動時の画面非表示**
  - [ ] MainWindow の初期表示を非表示に
  - [ ] タスクトレイから「設定を開く」で表示

### 4. TrayIcon (Infrastructure/TrayIcon.cs) の修正【最優先】
- [ ] **メニュー項目の変更**
  - [ ] 「設定を保存」メニュー削除
  - [ ] 「サーバー起動」メニュー追加
  - [ ] 「サーバー停止」メニュー追加
  - [ ] サーバー状態の管理・イベント連携

### 5. 不要ファイルの削除【最優先】
- [ ] `wwwroot/index.html` 削除
- [ ] `wwwroot/css/manager.css` 削除
- [ ] `wwwroot/js/manager.js` 削除
- [ ] `UI/ViewModels/CounterViewModel.cs` 削除（使ってない場合）
- [ ] `UI/Components/CounterListItem.xaml` + `.cs` 削除（使ってない場合）

### 6. 設定の自動保存実装【高】
- [ ] カウンター追加・編集・削除時に自動保存
- [ ] サーバー設定変更時に自動保存
- [ ] ホットキー変更時に自動保存
- [ ] MainWindowの「設定を保存」ボタン削除（必要な場合）

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
- [x] UIフォルダの機能別整理（Dialogs/Infrastructure/Views）
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

### 2-8. タスクトレイ実装 🔄
- [x] `UI/Infrastructure/TrayIcon.cs` 実装（フォルダ移動完了）
  - [x] コンテキストメニュー作成
  - [x] 通知機能削除
  - [ ] メニュー項目の変更（保存削除、サーバー起動/停止追加）

### 2-9. アプリケーション統合 🔄
- [x] `App.xaml.cs` 修正
  - [x] サーバー自動起動削除
  - [x] 終了時に設定自動保存
  - [ ] 起動時の画面非表示

---

## Phase 3: GUI実装（WPF） 【90%】🔄

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
  - [x] 名前空間の曖昧参照エラー修正

### 3-2. カウンター管理ビュー ✅
- [x] `UI/Views/CounterManagementView.xaml` 作成
  - [x] カード型UI
  - [x] ホットキー表示機能
  - [x] カウンター操作ボタン

- [x] `UI/Views/CounterManagementView.xaml.cs` 作成
  - [x] カウンター一覧表示
  - [x] 追加・編集・削除機能
  - [x] 名前空間の曖昧参照エラー修正

### 3-3. サーバー設定ビュー 🔄
- [x] `UI/Views/ServerSettingsView.xaml` 作成
  - [x] サーバー起動/停止ボタン
  - [ ] トグルボタン化
  - [x] ポート設定UI
  - [ ] 起動中のポート変更不可
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

### 3-5. CounterEditDialog 🔄
- [x] `UI/Dialogs/CounterEditDialog.xaml` 作成（フォルダ移動完了）
  - [x] シンプルデザイン
  - [x] 色選択（5色プリセット）
  - [ ] カラーピッカー化

- [x] `UI/Dialogs/CounterEditDialog.xaml.cs` 実装（フォルダ移動完了）
  - [x] 3つのコンストラクタ
  - [x] CounterName / CounterColor プロパティ
  - [ ] カラーピッカー統合
  - [ ] ホットキー設定機能

### 3-6. 名前空間の整理 ✅
- [x] WPF vs WinForms の衝突解決
- [x] Color / ColorConverter の曖昧参照解決
- [x] 全ファイルでエイリアス統一

### 3-7. フォルダ整理 ✅
- [x] `UI/Dialogs/` フォルダ新設
- [x] `UI/Infrastructure/` フォルダ新設
- [x] CounterEditDialog 移動
- [x] TrayIcon 移動
- [x] 名前空間の更新
- [x] 参照元ファイルの修正

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

### 4-2. 管理画面（削除予定） ⏳
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

### 優先度: 最高 🔥🔥🔥
- ⚠️ **CounterEditDialog 拡張が未実装**
  - カラーピッカー未実装
  - ホットキー設定機能未実装
- ⚠️ **ServerSettingsView のトグルボタン化が未実装**
- ⚠️ **アプリ起動時の画面非表示が未実装**
- ⚠️ **TrayIcon のメニュー変更が未実装**
- ⚠️ **不要ファイルの削除が未実施**

### 優先度: 高
- ⚠️ **設定の自動保存が未実装**
- ⚠️ **サーバー起動中のポート番号変更不可が未実装**
- ⚠️ **アイコンが仮アイコン**
- ⚠️ **動作確認が不十分**

### 優先度: 中
- ⚠️ **カウンター並び替え機能なし**
- ⚠️ **アニメーション未実装**

### 優先度: 低
- WebSocketSharpの警告（pragma directiveで抑制済み）

### 解決済み ✅
- ✅ 単一カウンターのみ対応 → 複数カウンター対応完了
- ✅ 設定の永続化未対応 → ConfigManager実装完了
- ✅ グローバルホットキー未実装 → HotkeyManager実装完了
- ✅ StaticFileProvider.ServeFileメソッド欠落 → 実装完了
- ✅ WPF/WinForms名前空間衝突 → エイリアスで解決
- ✅ サーバー自動起動 → 手動起動に変更
- ✅ カウンター値変更通知 → 削除
- ✅ ブラウザ管理画面 → 削除
- ✅ UIフォルダの構造が散らかっていた → 機能別に整理完了
- ✅ 名前空間の曖昧参照エラー → 完全修正完了

---

## 📝 実装時の注意点

- [x] グローバルホットキーは管理者権限不要で動作
- [x] WebSocketの自動再接続機能を実装済み
- [x] ポート番号の衝突対策を実装済み
- [x] OBS表示画面の背景透過を確認済み
- [x] WPFウィンドウを完全に非表示にする（タスクバーに出さない）
- [x] 設定の自動保存（終了時）
- [ ] カラーピッカーの統合（ColorDialog使用）
- [ ] ホットキー設定UIの実装（キー入力待機処理）
- [ ] トグルボタンの実装（サーバー起動/停止）
- [ ] 起動時の画面非表示処理
- [ ] タスクトレイメニューの変更

---

## 🎯 現在の作業

**現在のフェーズ**: Phase 3 - GUI実装（90%完了）  
**次のタスク**: CounterEditDialog 拡張実装（最優先）

**最近の成果**: 
- ✅ 名前空間の曖昧参照エラー完全修正！
- ✅ UIフォルダの機能別整理完了！
- ✅ 追加仕様の確定・要求仕様書更新完了！

**次回セッションの最優先タスク**:
1. CounterEditDialog: カラーピッカー & ホットキー設定実装
2. ServerSettingsView: トグルボタン化
3. App.xaml.cs: 起動時画面非表示
4. TrayIcon: メニュー変更
5. 不要ファイル削除
6. 設定自動保存実装

---

## 📅 マイルストーン

| マイルストーン | 目標日 | 状態 |
|---------------|--------|------|
| プロトタイプ完成 | - | ✅ 完了 |
| GUI完成（WPF設定画面） | - | 🔄 90%完了 |
| 複数カウンター対応 | - | ✅ 完了 |
| グローバルホットキー実装 | - | ✅ 完了 |
| 設定永続化実装 | - | ✅ 完了 |
| モダンUI実装 | - | ✅ 完了 |
| UIフォルダ整理 | - | ✅ 完了 |
| 名前空間エラー修正 | - | ✅ 完了 |
| CounterEditDialog 拡張 | 次回 | ⏳ 最優先 |
| サーバー設定トグルボタン化 | 次回 | ⏳ 最優先 |
| アプリ起動時画面非表示 | 次回 | ⏳ 最優先 |
| TrayIconメニュー変更 | 次回 | ⏳ 最優先 |
| 不要ファイル削除 | 次回 | ⏳ 最優先 |
| 設定自動保存実装 | 次回 | ⏳ 高優先度 |
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

### ColorDialog 使用例
```csharp
using WinForms = System.Windows.Forms;

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

### 単一ファイル発行コマンド
```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

---

**最終更新日**: 2026-01-05  
**次回の最優先タスク**: CounterEditDialog 拡張実装