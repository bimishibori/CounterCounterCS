# カウンター・カウンター 要求仕様書（C#版）

**バージョン**: 1.3  
**最終更新**: 2026-01-06

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
- **起動時はタスクトレイに常駐、WPF画面は非表示**
- **サーバーは手動起動方式**

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
│  │  ├─ Dialogs/              # ダイアログ専用
│  │  │  ├─ CounterEditDialog.xaml
│  │  │  └─ CounterEditDialog.xaml.cs
│  │  ├─ Infrastructure/               # システム機能
│  │  │  └─ TrayIcon.cs        # タスクトレイ管理
│  │  └─ Views/                # ビューコンポーネント
│  │     ├─ CounterManagementView.xaml
│  │     ├─ CounterManagementView.xaml.cs
│  │     ├─ ServerSettingsView.xaml
│  │     ├─ ServerSettingsView.xaml.cs
│  │     ├─ HotkeySettingsView.xaml
│  │     ├─ HotkeySettingsView.xaml.cs
│  │     ├─ ConnectionInfoView.xaml
│  │     └─ ConnectionInfoView.xaml.cs
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

**削除対象**:
- `wwwroot/index.html`（ブラウザ管理画面は不要）
- `wwwroot/css/manager.css`
- `wwwroot/js/manager.js`

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
4. WPF画面は非表示（タスクトレイのみ）
5. サーバーは停止状態で待機
6. ユーザーが「設定を開く」でWPF画面表示
7. 「サーバー起動」ボタンで手動起動
8. HTTPサーバー起動（ポート: 8765）
9. WebSocketサーバー起動（ポート: 8766）
10. グローバルホットキー登録
11. バックグラウンドで待機
```

### 6.3 ウィンドウ終了動作

| 状態 | バツボタン動作 | 説明 |
|------|---------------|------|
| サーバー停止中 | アプリ終了 | 通常の終了処理を実行 |
| サーバー起動中 | タスクトレイに格納 | ウィンドウを非表示にし、タスクトレイに常駐 |

**実装方針**:
- `Window.Closing` イベントで `e.Cancel` を制御
- サーバー起動中は `e.Cancel = true` でウィンドウを非表示
- サーバー停止中は `e.Cancel = false` でアプリ終了

---

## 7. タスクトレイ仕様

### 7.1 起動時動作

- EXE起動時にタスクトレイへ登録
- **WPF画面は表示しない**（タスクトレイのみ）
- **サーバーは自動起動しない**
- グローバルホットキーも未登録状態

### 7.2 トレイメニュー項目

| メニュー項目 | 動作 |
|--------------|------|
| 設定を開く | WPF設定ウィンドウを表示 |
| サーバー起動 | HTTPサーバー・WebSocketサーバー・ホットキーを起動 |
| サーバー停止 | サーバー・ホットキーを停止 |
| OBS表示用URLをコピー | `http://localhost:8765/obs.html` をクリップボードにコピー |
| 終了 | サーバー停止・アプリ終了 |

**削除**: ~~設定を保存~~ （自動保存に変更）

### 7.3 実装方法

- `System.Windows.Forms.NotifyIcon`を使用
- アイコン画像は `Resources/icon.ico` を使用
- コンテキストメニューで操作
- **カウンター値変更時の通知バルーンは表示しない**

---

## 8. グローバルホットキー仕様

### 8.1 ホットキー登録方式

- **複数セット対応**: 1つのカウンターにつき最大3セットのホットキーを登録可能
- **3キー同時押し対応**: 各セットで最大3つのキーを同時押し可能
- **コンボボックス方式**: UIはコンボボックスでキー選択

### 8.2 デフォルトキー設定

| 機能 | デフォルトキー | 変更可能 |
|------|----------------|----------|
| カウント増加 | `Ctrl + Shift + ↑` | ✅ |
| カウント減少 | `Ctrl + Shift + ↓` | ✅ |
| リセット | `Ctrl + Shift + R` | ✅ |

### 8.3 実装方法

- Win32 API `RegisterHotKey` / `UnregisterHotKey` 使用
- `WndProc` でメッセージ処理
- キー競合時はエラーダイアログ表示
- **サーバー起動時に自動登録、停止時に自動解除**

### 8.4 設定UI

- **CounterEditDialog でカウンター毎にホットキー設定可能**
- 増加・減少それぞれ最大3セット設定可能
- 各セット3つのコンボボックス（Ctrl/Shift/Alt + 任意キー）
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

**削除**: ~~`/`（ブラウザ管理画面）~~ - WPF設定画面のみ使用

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
- WPF設定画面の「サーバー起動/停止」トグルボタンで制御
- タスクトレイメニューからも起動/停止可能
- 起動/停止時にホットキーも同時に登録/解除
- **サーバー起動中はポート番号変更不可**

---

## 11. カウンター機能仕様

### 11.1 操作機能

| 機能 | API | グローバルホットキー | GUI操作 |
|------|-----|---------------------|---------|
| カウント増加 | POST `/api/counter/:id/increment` | カウンター毎に最大3セット設定可能 | ボタン |
| カウント減少 | POST `/api/counter/:id/decrement` | カウンター毎に最大3セット設定可能 | ボタン |
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
  - ダークテーマ（背景: #0f0f0f系）
  - アニメーション付きUI
  - グラデーション、影、角丸を活用
- **アプリ起動時は非表示、タスクトレイから「設定を開く」で表示**

### 12.2 デザイン要件

- **ダークテーマ**（背景: #0f0f0f系）
- モダンなフラットデザイン
- サイドバーナビゲーション
- アニメーション付きUI
- 配信者向けデザイン
- カウンター一覧で**ホットキー表示**
- **ウィンドウサイズ可変** - リサイズ可能

### 12.3 設定画面構成

#### セクション1: カウンター管理
- 現在値表示（リアルタイム更新）
- 「+」「-」「Reset」ボタン
- **ホットキー表示**（例: `Ctrl+Shift+↑, Ctrl+Alt+F1`）
- カウンター追加・編集・削除ボタン

#### セクション2: サーバー設定
| 設定項目 | 入力タイプ | デフォルト値 | 備考 |
|----------|-----------|--------------|------|
| サーバー起動/停止 | トグルボタン | 停止 | |
| ポート番号 | テキストボックス | 8765 | サーバー起動中は変更不可 |

#### セクション3: 接続情報
- OBS用URL表示
- 「ブラウザで開く」ボタン
- 「URLをコピー」ボタン
- 使用ポート表示
- 接続中のクライアント数表示

---

## 13. CounterEditDialog 仕様

### 13.1 設定項目

| 項目 | 入力タイプ | 説明 |
|------|-----------|------|
| カウンター名 | テキストボックス | カウンターの表示名 |
| 色 | カラーピッカー | ColorDialogから選択 |
| 増加ホットキー | コンボボックス×3×最大3セット | カウンター増加のショートカット |
| 減少ホットキー | コンボボックス×3×最大3セット | カウンター減少のショートカット |

### 13.2 色選択の実装

- `System.Windows.Forms.ColorDialog` を使用
- カラーコード（#RRGGBB）を取得して保存

### 13.3 ホットキー入力の実装

**新方式: コンボボックス選択**
- 各セット3つのコンボボックス
- 1つ目: Ctrl/Shift/Alt
- 2つ目: Ctrl/Shift/Alt（オプション）
- 3つ目: 任意キー（矢印、F1-F12、A-Z、0-9）
- キー競合チェック（他のカウンターと重複していないか）
- 最大3セットまで登録可能

**表示例**:
```
増加ホットキー:
  #1: [Ctrl] [Shift] [↑]
  #2: [Ctrl] [Alt] [F1]
  #3: [未選択] [未選択] [未選択]

減少ホットキー:
  #1: [Ctrl] [Shift] [↓]
  #2: [未選択] [未選択] [未選択]
  #3: [未選択] [未選択] [未選択]
```

---

## 14. アニメーション仕様

### 14.1 スライドイン演出

- 数値が変化するたびに実行
- **増加時**：下から上へスライドイン + フェードイン
- **減少時**：上から下へスライドイン + フェードイン
- 演出時間：300ms（設定で変更可能）
- CSS Transitionで実装

### 14.2 パーティクル演出

- 数値変化時のみ発生（常時表示なし）
- Canvas 2Dで軽量描画
- パーティクル数：20個（設定可能）
- 色：カウンター文字色に連動
- ON / OFF 切替可能（デフォルト: ON）

### 14.3 実装方針

- CSSアニメーション優先（軽量・簡潔）
- パーティクルのみJavaScript Canvas使用
- requestAnimationFrame で60fps維持

---

## 15. 設定管理仕様

### 15.1 設定保存方法

- JSONファイル保存（`config.json`）
- `System.Text.Json` を使用
- アプリ起動時に自動読み込み
- **設定変更時に自動保存**（手動保存ボタンは不要）

### 15.2 設定項目JSON構造
```json
{
  "Counters": [
    {
      "Id": "counter-1",
      "Name": "Death Counter",
      "Value": 0,
      "Color": "#ff6b6b"
    }
  ],
  "Hotkeys": [
    {
      "CounterId": "counter-1",
      "Action": 0,
      "Modifiers": 6,
      "VirtualKey": 38
    },
    {
      "CounterId": "counter-1",
      "Action": 0,
      "Modifiers": 5,
      "VirtualKey": 112
    }
  ],
  "ServerPort": 8765
}
```

**説明**:
- `Action`: 0=Increment, 1=Decrement, 2=Reset
- `Modifiers`: 2=Ctrl, 4=Shift, 6=Ctrl+Shift, 1=Alt
- `VirtualKey`: 38=↑, 40=↓, 112=F1, など
- 同じCounterId + Actionの組み合わせで複数登録可能（最大3セット）

---

## 16. EXE化・配布仕様

### 16.1 ビルド設定

#### 単一ファイルEXE
```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

#### インストーラ形式（オプション）
- Inno Setup または WiX Toolset使用
- スタートアップ登録オプション
- アンインストール機能

### 16.2 ビルド成果物

- `CounterCounter.exe`（単一ファイル）
- サイズ目安：15-30MB（.NET含む）
- または インストーラ `CounterCounter_Setup.exe`

### 16.3 配布形態

**オプション1: ZIP配布**
- CounterCounter.exe
- README.txt（使い方説明）
- LICENSE.txt

**オプション2: インストーラ配布**
- CounterCounter_Setup.exe
- 自動でスタートアップ登録可能

---

## 17. 配布・利用方法

### 17.1 利用手順（配信者向け）

1. `CounterCounter.exe` を起動（初回は Windows Defender警告が出る場合あり）
2. タスクトレイにアイコンが表示される（WPF画面は非表示）
3. トレイアイコンを右クリック →「設定を開く」
4. 「サーバー起動/停止」トグルボタンで起動
5. ホットキーを確認・変更
6. OBSで「ソース追加」→「ブラウザ」
7. URL欄に `http://localhost:8765/obs.html` を入力
8. 幅800、高さ600に設定
9. グローバルホットキーでカウンター操作開始

---

## 18. エラーハンドリング

### 18.1 ポート使用中エラー

- 8765が使用中の場合、8766, 8767... と自動で試行
- 10個試して全て失敗した場合、エラーダイアログ表示
- トレイアイコンに使用中ポート番号を表示

### 18.2 ホットキー競合エラー

- 登録失敗時は「キーが使用中です」とダイアログ表示
- 別のキーを選択するよう促す

### 18.3 WebSocket切断時

- クライアント側で自動再接続を試行（5秒間隔）

---

## 19. パフォーマンス要件

- アプリ起動時間：1秒以内
- カウンター操作レスポンス：50ms以内
- WebSocket遅延：30ms以内
- メモリ使用量：50MB以下（アイドル時）
- CPU使用率：1%以下（アイドル時）

---

## 20. セキュリティ考慮事項

- **localhostのみ**でリッスン（外部アクセス不可）
- 認証機能なし（ローカル利用のため不要）
- CORS制限なし（同一ホストのため不要）

---

## 21. 非要件（対象外）

- OBS公式C++プラグインの開発
- Windowsサービスとしての常駐
- 外部サーバー通信
- macOS / Linux 対応
- データベース使用
- ユーザー認証機能
- クラウド同期
- ブラウザ管理画面（WPF設定画面のみ）

---

## 22. 将来拡張（検討項目）

### Phase 2
- プリセット管理機能
- カウンター履歴機能

### Phase 3
- 効果音連動
- Twitch / YouTube Chat連携
- カスタムテーマ機能
- プラグインシステム

---

## 23. 開発環境

### 23.1 推奨環境

- IDE：Visual Studio 2022（Community Edition可）
- .NET：.NET 6 SDK 以上（.NET 8推奨）
- OS：Windows 10/11

### 23.2 必要なワークロード

Visual Studioインストール時に選択：
- .NET デスクトップ開発
- ASP.NET と Web 開発（オプション）

### 23.3 NuGetパッケージ
```xml
<PackageReference Include="WebSocketSharp-netstandard" Version="1.0.1" />
<PackageReference Include="System.Text.Json" Version="8.0.0" />
```

---

## 24. テスト方針

### 24.1 テスト項目

- アプリ起動・停止
- タスクトレイ常駐・初期非表示
- サーバー手動起動・停止（トグルボタン）
- タスクトレイからサーバー起動・停止
- グローバルホットキー動作（複数セット対応）
- カウンター増減・リセット
- WebSocket通信
- 設定変更の反映
- 設定ファイル自動保存・読み込み
- アニメーション動作
- OBSでの表示確認
- CounterEditDialog - カラーピッカー動作
- CounterEditDialog - ホットキー設定（コンボボックス方式）
- ポート番号変更不可（サーバー起動中）
- ウィンドウリサイズ動作
- バツボタン動作（サーバー停止中：終了、起動中：最小化）

### 24.2 テスト環境

- OBS Studio 最新版
- Windows 10/11
- ブラウザ：Chrome / Edge

---

## 25. 変更履歴

| 日付 | 版 | 変更内容 |
|------|----|---------| 
| 2026-01-04 | 1.0 | Python版から C# 版として作成 |
| 2026-01-05 | 1.1 | UI設計変更、サーバー起動制御追加、通知削除、ブラウザ管理画面削除 |
| 2026-01-05 | 1.2 | CounterEditDialog拡張、カラーピッカー、ホットキー設定追加 |
| 2026-01-06 | 1.3 | 以下の追加仕様を反映:<br>- ホットキー: コンボボックス方式に変更、最大3セット対応<br>- ウィンドウ終了動作: サーバー起動中はタスクトレイに格納<br>- ウィンドウサイズ可変対応 |

---

## 付録A：使用ライブラリ詳細

| ライブラリ | 用途 | 必須 |
|-----------|------|------|
| System.Net.HttpListener | HTTPサーバー | ✅ 標準 |
| System.Windows.Forms | NotifyIcon（トレイアイコン）、ColorDialog | ✅ 標準 |
| System.Text.Json | JSON処理 | ✅ 標準 |
| WebSocketSharp-netstandard | WebSocket通信 | ✅ NuGet |

---

## 付録B：参考リンク

- .NET公式: https://dotnet.microsoft.com/
- WPF チュートリアル: https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/
- HttpListener: https://docs.microsoft.com/ja-jp/dotnet/api/system.net.httplistener
- RegisterHotKey API: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey
- ColorDialog: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.colordialog

---

<div align="center">

**開発中のため、機能や仕様は予告なく変更される可能性があります**

Made with ❤️ for Streamers

</div>