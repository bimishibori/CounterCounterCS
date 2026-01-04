# カウンター・カウンター 開発引き継ぎ資料

**作成日**: 2026-01-04  
**プロジェクト名**: Counter Counter（カウンター・カウンター）  
**技術スタック**: C# + .NET 8 + WPF

---

## 📊 現在の進捗状況

### 全体進捗: **30%完了**

| フェーズ | 進捗 | 状態 |
|---------|------|------|
| Phase 1: 環境構築 | 100% | ✅ 完了 |
| Phase 2: コア機能実装 | 50% | 🔄 進行中 |
| Phase 3: GUI実装 | 0% | ⏳ 未着手 |
| Phase 4: アニメーション | 10% | 🔄 一部実装 |
| Phase 5: EXE化・配布 | 0% | ⏳ 未着手 |

---

## ✅ 完了した機能

### 1. プロジェクト基盤
- [x] Visual Studio 2022 プロジェクト作成
- [x] .NET 8 環境構築
- [x] NuGetパッケージ導入
  - WebSocketSharp-netstandard
  - System.Windows.Forms

### 2. タスクトレイ機能
- [x] `TrayIcon.cs` 実装完了
  - NotifyIconによる常駐
  - コンテキストメニュー
  - 「設定を開く」（仮実装）
  - 「管理ページを開く」
  - 「OBS URLをコピー」
  - 「再起動」（仮実装）
  - 「終了」
  - ツールチップ（ポート番号表示）

### 3. カウンター状態管理
- [x] `CounterState.cs` 実装完了
  - スレッドセーフな値管理
  - Increment() / Decrement() / Reset()
  - ValueChangedイベント
  - 変更タイプ検知（増加/減少/リセット）

### 4. HTTPサーバー
- [x] `WebServer.cs` 実装完了
  - HttpListenerによるローカルサーバー
  - ポート自動選択（8765から順に試行）
  - 静的ファイル配信
  - APIエンドポイント
    - GET `/api/counter` - カウンター値取得
    - POST `/api/counter/increment` - カウンター増加
    - POST `/api/counter/decrement` - カウンター減少
    - POST `/api/counter/reset` - リセット
  - 管理画面HTML（埋め込み）
  - OBS表示画面HTML（埋め込み）

### 5. WebSocket通信
- [x] `WebSocketServer.cs` 実装完了
  - WebSocketSharpによるリアルタイム通信
  - HTTPポート+1で自動起動（例: 8766）
  - カウンター変更時の即時ブロードキャスト
  - 接続時に現在値を送信
  - 自動再接続対応（クライアント側）

### 6. Webインターフェース
- [x] 管理画面（index.html）
  - ダークテーマUI
  - カウンター操作ボタン（+/-/リセット）
  - WebSocket接続状態表示
  - リアルタイム更新
- [x] OBS表示画面（obs.html）
  - 背景透過対応
  - 大きなカウンター表示
  - フラッシュエフェクト（基本実装）
  - WebSocketリアルタイム更新

---

## 🔧 現在のシステム構成

```
CounterCounter.exe
├─ タスクトレイ常駐 ✅
├─ HTTPサーバー (Port: 8765) ✅
├─ WebSocketサーバー (Port: 8766) ✅
├─ カウンター状態管理 ✅
└─ グローバルホットキー ❌ 未実装

管理画面 (http://localhost:8765/)
└─ WebSocket接続でリアルタイム更新 ✅

OBS表示 (http://localhost:8765/obs.html)
└─ WebSocket接続でリアルタイム更新 ✅
```

---

## 📁 実装済みファイル一覧

| ファイル名 | 実装状況 | 説明 |
|-----------|---------|------|
| `App.xaml` | ✅ 完了 | アプリケーション定義 |
| `App.xaml.cs` | ✅ 完了 | 起動処理・サーバー初期化 |
| `TrayIcon.cs` | ✅ 完了 | タスクトレイアイコン管理 |
| `CounterState.cs` | ✅ 完了 | カウンター状態管理 |
| `WebServer.cs` | ✅ 完了 | HTTPサーバー |
| `WebSocketServer.cs` | ✅ 完了 | WebSocketサーバー |
| `MainWindow.xaml` | ⏳ 未実装 | 設定画面GUI |
| `HotkeyManager.cs` | ⏳ 未実装 | グローバルホットキー |
| `ConfigManager.cs` | ⏳ 未実装 | 設定ファイル管理 |

---

## 🎯 次に実装すべき機能

### 優先度：高

#### 1. グローバルホットキー実装 🔥
- `HotkeyManager.cs` 作成
- Win32 API `RegisterHotKey` / `UnregisterHotKey` 使用
- デフォルトキー設定
  - カウンター増加: `Ctrl+Shift+↑`
  - カウンター減少: `Ctrl+Shift+↓`
  - リセット: `Ctrl+Shift+R`
- キー競合チェック

#### 2. WPF設定画面
- `MainWindow.xaml` 実装
- タブ構成
  - カウンター操作タブ
  - 表示設定タブ（フォント、色、サイズ）
  - 演出設定タブ（アニメーション速度）
  - ホットキー設定タブ
  - 接続情報タブ
- MVVMパターン適用

#### 3. 設定の永続化
- `ConfigManager.cs` 実装
- JSON設定ファイル（config.json）
- 起動時の設定読み込み
- 設定変更時の自動保存

### 優先度：中

#### 4. アニメーション強化
- スライドイン演出（上下方向）
- パーティクルエフェクト（Canvas）
- アニメーション速度設定対応

#### 5. 外部ファイル化
- HTML/CSS/JSを外部ファイルに分離
- `wwwroot/` フォルダから読み込み
- EXE化時の埋め込み対応

### 優先度：低

#### 6. エラーハンドリング強化
- ポート使用中エラーの詳細表示
- WebSocket切断時の再接続ロジック改善
- ログ出力機能

---

## 🐛 既知の問題・制限事項

### 解決済み
- ✅ ポート競合エラー → HTTPとWebSocketでポート分離（8765, 8766）
- ✅ Application型のあいまいな参照 → エイリアス使用

### 未解決
- ⚠️ HTML/CSS/JSが埋め込みコード（外部ファイル化が必要）
- ⚠️ 設定の永続化未対応（再起動で初期化される）
- ⚠️ グローバルホットキー未実装（現在はブラウザ経由のみ）

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
```

---

## 🎮 動作確認方法

### 1. アプリケーション起動
```
1. Visual StudioでF5キー（デバッグ実行）
2. タスクトレイにアイコンが表示される
3. ツールチップに "HTTP:8765 WS:8766" と表示される
```

### 2. 管理画面テスト
```
1. タスクトレイアイコンを右クリック
2. 「管理ページを開く」をクリック
3. ブラウザで http://localhost:8765/ が開く
4. 「✓ 接続中」と表示される
5. +/-/リセットボタンが動作する
6. カウンターがリアルタイム更新される
```

### 3. OBS表示テスト
```
1. ブラウザで http://localhost:8765/obs.html を開く
2. カウンター値が表示される
3. 管理画面でボタンを押すと即座に更新される
4. 数値変化時にフラッシュエフェクトが発生
```

---

## 📝 重要な設計決定

### 1. ポート設定
- **HTTPサーバー**: 8765（自動選択、競合時は8766, 8767...）
- **WebSocketサーバー**: HTTPポート+1（例: 8766）
- 理由: 同一ポートでHTTPとWebSocketの競合を回避

### 2. スレッドセーフ設計
- `CounterState` は `lock` でスレッドセーフを保証
- 複数クライアントからの同時アクセスに対応

### 3. イベント駆動アーキテクチャ
- カウンター変更時にイベント発火
- WebSocketが自動的に全クライアントにブロードキャスト
- 疎結合で拡張しやすい設計

### 4. HTML埋め込み（暫定）
- 現在はC#コード内にHTML文字列を埋め込み
- 理由: 早期プロトタイピング
- 今後: 外部ファイル化してメンテナンス性向上

---

## 🔄 次のセッションで実装すべきこと

### 最優先タスク
1. **グローバルホットキー実装** (`HotkeyManager.cs`)
   - これがないと配信中に使いにくい
   - Win32 API の実装例は要求仕様書に記載あり

2. **WPF設定画面** (`MainWindow.xaml`)
   - タスクトレイから開けるGUI
   - 最低限: カウンター操作とホットキー設定

3. **設定の永続化** (`ConfigManager.cs`)
   - config.json で設定保存
   - ホットキー設定を保存できるようにする

### 実装の優先順位
```
グローバルホットキー → 設定画面 → 設定永続化 → アニメーション強化 → EXE化
```

---

## 📚 参考情報

### コードの場所
- **GitHub**: （次のセッションで共有予定）
- **要求仕様書**: `要求仕様書（C#版）`
- **タスク管理表**: `タスク管理表`

### 外部リンク
- [WebSocketSharp GitHub](https://github.com/sta/websocket-sharp)
- [RegisterHotKey API](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey)
- [WPF チュートリアル](https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/)

---

## 💡 開発のヒント

### グローバルホットキー実装時の注意
- `HwndSource` を使ってWPFでWin32メッセージを受け取る
- 管理者権限は不要（localhostのみなので）
- キー競合時のエラーハンドリングを忘れずに

### 設定画面実装時の注意
- WPFウィンドウは `ShowInTaskbar = false` で非表示に
- タスクトレイから開くときは `Show()` / `Activate()`
- 閉じるボタンで終了せず、非表示にする（`Hide()`）

### EXE化時の注意
- `PublishSingleFile=true` で単一EXE化
- wwwrootフォルダは埋め込みリソースとして含める
- `IncludeNativeLibrariesForSelfExtract=true` が必要

---

## 🎉 完成イメージ

最終的に以下ができるアプリ：
1. 起動すると自動でタスクトレイに常駐
2. グローバルホットキーでゲーム中でもカウンター操作
3. OBSブラウザソースでリアルタイム表示
4. スタイリッシュなアニメーション
5. 簡単な設定GUI
6. 単一EXEで配布可能

---

## 📞 引き継ぎ時の確認事項

次のセッションで開発を続ける場合、以下を確認：
- [ ] GitHubリポジトリのURL
- [ ] 開発環境（Visual Studio 2022, .NET 8）
- [ ] 現在のブランチ・コミット
- [ ] 未解決の問題やエラー

---

**作成者**: Claude (Anthropic)  
**最終更新**: 2026-01-04  
**バージョン**: v0.1-alpha