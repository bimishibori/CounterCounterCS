# カウンター・カウンター 要求仕様書（C#版）

## 1. 概要

本仕様書は、OBS配信者向けに提供する  
**ブラウザ表示型・演出付きカウンターアプリ「カウンター・カウンター（Counter Counter）」**の要求仕様を定義するものである。

本アプリは、OBSのブラウザソースと連携し、  
グローバルホットキーで直感的にカウンター操作を可能とすることを目的とする。

---

## 2. 目的

- 配信中のカウンター操作を簡略化する
- グローバルホットキーで画面切り替え不要に
- 視覚的に魅力的な演出を提供する
- 配信者が導入しやすい形で配布する

想定用途：
- デスカウンター
- 勝敗カウンター
- 任意イベントカウンター

---

## 3. 基本方針

- **C# + .NET 6/8**で実装
- OBS公式機能（Browser Source）のみを利用する
- Windows専用
- 配布形態は EXE（単一ファイル or インストーラ）
- GUIは WPF
- 常駐型だが Windowsサービスにはしない
- **起動時はサーバー停止状態**でユーザーが手動起動

---

## 4. システム構成

### 4.1 全体アーキテクチャ

```
CounterCounter.exe (C#製)
├ タスクトレイ常駐（NotifyIcon）
├ ローカルHTTPサーバー（HttpListener）- 手動起動
├ WebSocket通信（WebSocketSharp）- 手動起動
├ グローバルホットキー登録（Win32 API）
├ カウンター状態管理
└ 設定GUI（WPF）- モダンデザイン

OBS
└ Browser Source
    └ 表示用Webページ（obs.html）
```

- 通信は localhost のみ
- 外部ネットワーク通信は行わない
- **ブラウザ管理画面は不要**（WPF設定画面のみ）

---

## 5. 技術スタック

### 5.1 使用技術

| 項目 | 技術 | 目的 |
|------|------|------|
| 言語 | C# 10+ | アプリケーション本体 |
| フレームワーク | .NET 6 or .NET 8 | ランタイム |
| GUI | WPF | 設定画面UI（モダンデザイン） |
| HTTPサーバー | HttpListener | ローカルWebサーバー |
| WebSocket | WebSocketSharp | リアルタイム通信 |
| ホットキー | Win32 API (RegisterHotKey) | グローバルショートカット |
| タスクトレイ | NotifyIcon | トレイアイコン・メニュー |
| 設定保存 | JSON (System.Text.Json) | 設定ファイル管理 |
| フロントエンド | HTML/CSS/JS | OBS表示画面 |

### 5.2 ディレクトリ構成

```
CounterCounter/
├─ CounterCounter.sln              # Visual Studioソリューション
├─ CounterCounter/
│  ├─ Program.cs               # エントリーポイント
│  ├─ App.xaml                 # WPFアプリケーション定義
│  ├─ App.xaml.cs
│  ├─ Core/
│  │  ├─ CounterManager.cs     # カウンター状態管理
│  │  ├─ HotkeyManager.cs      # ホットキー管理
│  │  └─ ConfigManager.cs      # 設定管理
│  ├─ Server/
│  │  ├─ WebServer.cs          # HTTPサーバー
│  │  ├─ WebSocketServer.cs    # WebSocket通信
│  │  ├─ ApiHandler.cs         # APIエンドポイント
│  │  ├─ HtmlContentProvider.cs # HTML生成
│  │  └─ StaticFileProvider.cs # 静的ファイル配信
│  ├─ UI/
│  │  ├─ MainWindow.xaml       # 設定GUI（モダンデザイン）
│  │  ├─ MainWindow.xaml.cs
│  │  ├─ TrayIcon.cs           # タスクトレイ管理
│  │  └─ CounterEditDialog.xaml # カウンター編集ダイアログ
│  ├─ Models/
│  │  ├─ Counter.cs
│  │  ├─ HotkeySettings.cs
│  │  └─ CounterSettings.cs
│  ├─ wwwroot/                 # Webファイル
│  │  ├─ obs.html              # OBS表示画面
│  │  ├─ css/
│  │  │  └─ obs.css
│  │  └─ js/
│  │     └─ obs.js
│  ├─ Resources/
│  │  └─ icon.ico              # アプリアイコン
│  └─ config.json              # 設定ファイル
├─ README.md
└─ LICENSE.txt
```

---

## 6. 実行形態

### 6.1 アプリケーション形態

- 種別：デスクトップアプリケーション（EXE）
- 起動：ユーザーが明示的に起動
- 常駐：タスクトレイに常駐
- 終了：タスクトレイメニューから終了

### 6.2 起動フロー

```
1. CounterCounter.exe 起動
2. 設定ファイル読み込み（config.json）
3. タスクトレイにアイコン表示
4. サーバーは停止状態で待機
5. ユーザーが「サーバー起動」ボタンを押す
6. HTTPサーバー起動（ポート: 8765）
7. WebSocketサーバー起動（ポート: 8766）
8. グローバルホットキー登録
9. バックグラウンドで待機
```

---

## 7. タスクトレイ仕様

### 7.1 起動時動作

- EXE起動時にタスクトレイへ登録
- **サーバーは自動起動しない**
- グローバルホットキーも未登録状態

### 7.2 トレイメニュー項目

| メニュー項目 | 動作 |
|--------------|------|
| 設定を開く | WPF設定ウィンドウを表示 |
| OBS表示用URLをコピー | `http://localhost:8765/obs.html` をクリップボードにコピー |
| 設定を保存 | config.jsonに保存 |
| 終了 | サーバー停止・アプリ終了 |

### 7.3 実装方法

- `System.Windows.Forms.NotifyIcon`を使用
- アイコン画像は `Resources/icon.ico` を使用
- コンテキストメニューで操作
- **カウンター値変更時の通知バルーンは表示しない**

---

## 8. グローバルホットキー仕様

### 8.1 デフォルトキー設定

| 機能 | デフォルトキー | 変更可能 |
|------|----------------|----------|
| カウント増加 | `Ctrl + Shift + ↑` | ✅ |
| カウント減少 | `Ctrl + Shift + ↓` | ✅ |
| リセット | `Ctrl + Shift + R` | ✅ |

### 8.2 実装方法

- Win32 API `RegisterHotKey` / `UnregisterHotKey` 使用
- `WndProc` でメッセージ処理
- キー競合時はエラーダイアログ表示
- **サーバー起動時に自動登録、停止時に自動解除**

### 8.3 設定UI

- WPFダイアログで視覚的に設定
- 現在のキーを表示
- 「記録」ボタンで新しいキーを待機
- キー競合チェック機能

---

## 9. OBS連携仕様

### 9.1 表示方式

- OBSの「ブラウザソース」を使用
- 表示URL：`http://localhost:8765/obs.html`

### 9.2 OBS設定推奨値

| 項目 | 推奨値 |
|------|--------|
| 幅 | 800px |
| 高さ | 600px |
| FPS | 30 |
| カスタムCSS | なし |
| シャットダウン時にソースを非表示 | OFF |

### 9.3 表示内容

- 現在のカウンター値（大きく中央表示）
- 数値変化時のアニメーション
- パーティクル演出（設定でON/OFF可能）

---

## 10. サーバー仕様

### 10.1 HTTPエンドポイント

| エンドポイント | メソッド | 説明 |
|----------------|----------|------|
| `/obs.html` | GET | OBS表示画面 |
| `/api/counters` | GET | 全カウンター取得 |
| `/api/counter/:id/increment` | POST | カウンター+1 |
| `/api/counter/:id/decrement` | POST | カウンター-1 |
| `/api/counter/:id/reset` | POST | カウンターリセット |
| `/api/settings` | GET/POST | 設定の取得・更新 |

**注意**: ブラウザ管理画面（`/`）は不要。WPF設定画面のみ使用。

### 10.2 WebSocketイベント

| イベント名 | 方向 | データ | 説明 |
|-----------|------|--------|------|
| `connect` | Client→Server | - | 接続確立 |
| `counter_update` | Server→Client | `{counterId, value}` | カウンター値更新通知 |

### 10.3 ポート設定

- デフォルトポート：8765（HTTP）、8766（WebSocket）
- ポート衝突時は自動で別ポートを選択（8767, 8768...）
- 使用ポートはトレイアイコンツールチップに表示

### 10.4 起動制御

- **アプリ起動時はサーバー停止状態**
- WPF設定画面の「サーバー起動」ボタンで手動起動
- 「サーバー停止」ボタンで手動停止
- 起動/停止時にホットキーも同時に登録/解除

---

## 11. カウンター機能仕様

### 11.1 操作機能

| 機能 | API | グローバルホットキー | GUI操作 |
|------|-----|---------------------|---------|
| カウント増加 | POST `/api/counter/:id/increment` | `Ctrl+Shift+↑` | ボタン |
| カウント減少 | POST `/api/counter/:id/decrement` | `Ctrl+Shift+↓` | ボタン |
| リセット | POST `/api/counter/:id/reset` | `Ctrl+Shift+R` | ボタン |
| 初期値設定 | POST `/api/settings` | - | 設定画面 |

### 11.2 状態管理

- カウンター値はメモリ上で管理（`CounterManager.cs`）
- 設定は `config.json` に永続化
- アプリ再起動時は前回値を復元可能（設定で選択）
- 負の値も許可
- 最大値・最小値制限なし

---

## 12. WPF設定画面仕様

### 12.1 実装方式

- WPFでネイティブGUI
- MVVM パターン推奨
- **モダン・スタイリッシュなデザイン**
  - フラットデザイン
  - ダークテーマ（背景: #1e1e1e系）
  - アニメーション付きUI
  - グラデーション、影、角丸を活用

### 12.2 デザイン要件

- **ダークテーマ**（背景: #1e1e1e系）
- モダンなフラットデザイン
- タブではなくサイドバーナビゲーション推奨
- アニメーション付きUI
- 配信者向けデザイン
- カウンター一覧で**ホットキー表示**

### 12.3 設定画面構成

#### セクション1: カウンター管理
- 現在値表示（リアルタイム更新）
- 「+」「-」「Reset」ボタン
- **ホットキー表示**（例: `Ctrl+Shift+↑`）
- カウンター追加・編集・削除ボタン

#### セクション2: サーバー設定
| 設定項目 | 入力タイプ | デフォルト値 |
|----------|-----------|--------------|
| サーバー起動/停止 | ボタン | 停止 |
| ポート番号 | テキストボックス | 8765 |

#### セクション3: 接続情報
- OBS用URL表示
- 「ブラウザで開く」ボタン
- 「URLをコピー」ボタン
- 使用ポート表示
- 接続中のクライアント数表示

---

## 13. アニメーション仕様

### 13.1 スライドイン演出

- 数値が変化するたびに実行
- **増加時**：下から上へスライドイン + フェードイン
- **減少時**：上から下へスライドイン + フェードイン
- 演出時間：300ms（設定で変更可能）
- CSS Transitionで実装

### 13.2 パーティクル演出

- 数値変化時のみ発生（常時表示なし）
- Canvas 2Dで軽量描画
- パーティクル数：20個（設定可能）
- 色：カウンター文字色に連動
- ON / OFF 切替可能（デフォルト: ON）

### 13.3 実装方針

- CSSアニメーション優先（軽量・簡潔）
- パーティクルのみJavaScript Canvas使用
- requestAnimationFrame で60fps維持

---

## 14. 設定管理仕様

### 14.1 設定保存方法

- JSONファイル保存（`config.json`）
- `System.Text.Json` を使用
- アプリ起動時に自動読み込み
- 設定変更時に自動保存

### 14.2 設定項目JSON構造

```json
{
  "counter": {
    "initial_value": 0,
    "current_value": 0,
    "restore_on_startup": false
  },
  "display": {
    "font_family": "Arial",
    "font_size": 120,
    "color": "#ffffff",
    "background_color": "transparent"
  },
  "animation": {
    "slide_enabled": true,
    "particle_enabled": true,
    "duration_ms": 300
  },
  "hotkeys": {
    "increment": "Ctrl+Shift+Up",
    "decrement": "Ctrl+Shift+Down",
    "reset": "Ctrl+Shift+R"
  },
  "server": {
    "port": 8765,
    "auto_start": false
  }
}
```

---

## 15. EXE化・配布仕様

### 15.1 ビルド設定

#### 単一ファイルEXE
```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

#### インストーラ形式（オプション）
- Inno Setup または WiX Toolset使用
- スタートアップ登録オプション
- アンインストール機能

### 15.2 ビルド成果物

- `CounterCounter.exe`（単一ファイル）
- サイズ目安：15-30MB（.NET含む）
- または インストーラ `CounterCounter_Setup.exe`

### 15.3 配布形態

**オプション1: ZIP配布**
- CounterCounter.exe
- README.txt（使い方説明）
- LICENSE.txt

**オプション2: インストーラ配布**
- CounterCounter_Setup.exe
- 自動でスタートアップ登録可能

---

## 16. 配布・利用方法

### 16.1 利用手順（配信者向け）

1. `CounterCounter.exe` を起動（初回は Windows Defender警告が出る場合あり）
2. タスクトレイにアイコンが表示される
3. トレイアイコンを右クリック →「設定を開く」
4. 「サーバー起動」ボタンをクリック
5. ホットキーを確認・変更
6. OBSで「ソース追加」→「ブラウザ」
7. URL欄に `http://localhost:8765/obs.html` を入力
8. 幅800、高さ600に設定
9. グローバルホットキーでカウンター操作開始

---

## 17. エラーハンドリング

### 17.1 ポート使用中エラー

- 8765が使用中の場合、8766, 8767... と自動で試行
- 10個試して全て失敗した場合、エラーダイアログ表示
- トレイアイコンに使用中ポート番号を表示

### 17.2 ホットキー競合エラー

- 登録失敗時は「キーが使用中です」とダイアログ表示
- 別のキーを選択するよう促す

### 17.3 WebSocket切断時

- クライアント側で自動再接続を試行（5秒間隔）

---

## 18. パフォーマンス要件

- アプリ起動時間：1秒以内
- カウンター操作レスポンス：50ms以内
- WebSocket遅延：30ms以内
- メモリ使用量：50MB以下（アイドル時）
- CPU使用率：1%以下（アイドル時）

---

## 19. セキュリティ考慮事項

- **localhostのみ**でリッスン（外部アクセス不可）
- 認証機能なし（ローカル利用のため不要）
- CORS制限なし（同一ホストのため不要）

---

## 20. 非要件（対象外）

- OBS公式C++プラグインの開発
- Windowsサービスとしての常駐
- 外部サーバー通信
- macOS / Linux 対応
- データベース使用
- ユーザー認証機能
- クラウド同期
- ブラウザ管理画面（WPF設定画面のみ）

---

## 21. 将来拡張（検討項目）

### Phase 2
- 複数カウンター対応
- プリセット管理機能
- カウンター履歴機能

### Phase 3
- 効果音連動
- Twitch / YouTube Chat連携
- カスタムテーマ機能
- プラグインシステム

---

## 22. 開発環境

### 22.1 推奨環境

- IDE：Visual Studio 2022（Community Edition可）
- .NET：.NET 6 SDK 以上（.NET 8推奨）
- OS：Windows 10/11

### 22.2 必要なワークロード

Visual Studioインストール時に選択：
- .NET デスクトップ開発
- ASP.NET と Web 開発（オプション）

### 22.3 NuGetパッケージ

```xml
<PackageReference Include="WebSocketSharp-netstandard" Version="1.0.1" />
<PackageReference Include="System.Text.Json" Version="8.0.0" />
```

---

## 23. テスト方針

### 23.1 テスト項目

- アプリ起動・停止
- サーバー手動起動・停止
- グローバルホットキー動作
- カウンター増減・リセット
- WebSocket通信
- 設定変更の反映
- 設定ファイル保存・読み込み
- アニメーション動作
- OBSでの表示確認

### 23.2 テスト環境

- OBS Studio 最新版
- Windows 10/11
- ブラウザ：Chrome / Edge

---

## 24. 進行状況整理

- ✅ 完了：技術選定（C# + .NET）
- ✅ 完了：要求仕様整理
- 🔄 進行中：詳細設計
- ⏳ 未着手：実装、テスト、配布準備

---

## 付録A：使用ライブラリ詳細

| ライブラリ | 用途 | 必須 |
|-----------|------|------|
| System.Net.HttpListener | HTTPサーバー | ✅ 標準 |
| System.Windows.Forms | NotifyIcon（トレイアイコン） | ✅ 標準 |
| System.Text.Json | JSON処理 | ✅ 標準 |
| WebSocketSharp-netstandard | WebSocket通信 | ✅ NuGet |

---

## 付録B：参考リンク

- .NET公式: https://dotnet.microsoft.com/
- WPF チュートリアル: https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/
- HttpListener: https://docs.microsoft.com/ja-jp/dotnet/api/system.net.httplistener
- RegisterHotKey API: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey

---

## 変更履歴

| 日付 | 版 | 変更内容 |
|------|----|---------| 
| 2026-01-04 | 1.0 | Python版から C# 版として作成 |
| 2026-01-05 | 1.1 | UI設計変更、サーバー起動制御追加、通知削除、ブラウザ管理画面削除 |