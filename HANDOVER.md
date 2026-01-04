# カウンター・カウンター 開発引き継ぎ資料

**作成日**: 2026-01-04  
**最終更新**: 2026-01-04 (セッション2)  
**プロジェクト名**: Counter Counter（カウンター・カウンター）  
**技術スタック**: C# + .NET 8 + WPF

---

## 📊 現在の進捗状況

### 全体進捗: **60%完了**

| フェーズ | 進捗 | 状態 |
|---------|------|------|
| Phase 1: 環境構築 | 100% | ✅ 完了 |
| Phase 2: コア機能実装 | 80% | 🔄 進行中 |
| Phase 3: GUI実装 | 60% | 🔄 進行中 |
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
- [x] **名前空間整理完了** (Core/Server/UI)
- [x] フォルダ構造の整理

### 2. コア機能 (Core/)
- [x] `CounterState.cs` 実装完了
  - スレッドセーフな値管理
  - Increment() / Decrement() / Reset()
  - ValueChangedイベント
  - 変更タイプ検知（増加/減少/リセット）

### 3. サーバー機能 (Server/)
- [x] `WebServer.cs` 実装完了
  - HttpListenerによるローカルサーバー
  - ポート自動選択（8765から順に試行）
  - 静的ファイル配信（wwwroot/）
  - ルーティング処理の分離
  - CORS対応
- [x] `ApiHandler.cs` 実装完了
  - GET `/api/counter` - カウンター値取得
  - POST `/api/counter/increment` - カウンター増加
  - POST `/api/counter/decrement` - カウンター減少
  - POST `/api/counter/reset` - リセット
- [x] `WebSocketServer.cs` 実装完了
  - WebSocketSharpによるリアルタイム通信
  - HTTPポート+1で自動起動（例: 8766）
  - カウンター変更時の即時ブロードキャスト
  - 接続時に現在値を送信
- [x] `HtmlContentProvider.cs` 実装完了
  - HTML生成管理
  - フォールバック機能
- [x] `StaticFileProvider.cs` 実装完了
  - wwwrootからのファイル読み込み
  - Content-Type自動判定
  - ファイル存在確認

### 4. UI機能 (UI/)
- [x] `TrayIcon.cs` 実装完了
  - NotifyIconによる常駐
  - コンテキストメニュー
  - 「設定を開く」→ WPFウィンドウ表示
  - 「管理ページを開く」→ ブラウザ起動
  - 「OBS URLをコピー」→ クリップボードコピー
  - 「終了」→ アプリケーション終了
  - ツールチップ（ポート番号表示）
  - カウンター値の変更通知
- [x] `MainWindow.xaml` 実装完了
  - ダークテーマWPF設定画面
  - タブ構成（3タブ）
    1. カウンター操作タブ
    2. 接続情報タブ
    3. 設定タブ（無効化・今後実装）
  - リアルタイムカウンター表示
  - +/-/リセットボタン
  - OBS URL表示＆コピー機能
  - ポート情報表示
  - 閉じても非表示化（終了しない）

### 5. Webインターフェース (wwwroot/)
- [x] `index.html` - 管理画面（ブラウザ版）
- [x] `obs.html` - OBS表示画面
- [x] `css/manager.css` - 管理画面スタイル
- [x] `css/obs.css` - OBS表示スタイル
- [x] `js/manager.js` - 管理画面ロジック
- [x] `js/obs.js` - OBS表示ロジック
- [x] WebSocket自動再接続機能
- [x] フラッシュエフェクト（基本実装）

### 6. ビルド設定
- [x] .csprojの設定完了
  - wwwrootフォルダの自動コピー設定

---

## 🔧 現在のシステム構成

```
CounterCounter.exe
├─ タスクトレイ常駐 ✅
├─ HTTPサーバー (Port: 8765) ✅
├─ WebSocketサーバー (Port: 8766) ✅
├─ カウンター状態管理 ✅
├─ WPF設定画面 ✅
└─ グローバルホットキー ❌ 未実装

管理画面
├─ WPF版 (設定画面) ✅
└─ ブラウザ版 (http://localhost:8765/) ✅

OBS表示 (http://localhost:8765/obs.html)
└─ WebSocket接続でリアルタイム更新 ✅
```

---

## 📁 プロジェクト構造

```
CounterCounter/
├── Core/                    # コア機能
│   └── CounterState.cs     # カウンター状態管理
├── Server/                  # サーバー機能
│   ├── ApiHandler.cs       # APIエンドポイント処理
│   ├── HtmlContentProvider.cs  # HTML生成
│   ├── StaticFileProvider.cs   # 静的ファイル読み込み
│   ├── WebServer.cs        # HTTPサーバー
│   └── WebSocketServer.cs  # WebSocketサーバー
├── UI/                      # UI機能
│   ├── MainWindow.xaml     # WPF設定画面
│   ├── MainWindow.xaml.cs  # 設定画面ロジック
│   └── TrayIcon.cs         # タスクトレイ管理
├── Models/                  # データモデル（今後使用）
├── wwwroot/                 # Webファイル
│   ├── index.html          # 管理画面（ブラウザ版）
│   ├── obs.html            # OBS表示画面
│   ├── css/
│   │   ├── manager.css     # 管理画面スタイル
│   │   └── obs.css         # OBS表示スタイル
│   └── js/
│       ├── manager.js      # 管理画面ロジック
│       └── obs.js          # OBS表示ロジック
├── Resources/               # リソースファイル（今後使用）
├── App.xaml                # WPFアプリケーション定義
├── App.xaml.cs             # アプリケーション起動処理
└── CounterCounter.csproj   # プロジェクト設定
```

---

## 🎯 次に実装すべき機能

### 優先度：高 🔥

#### 1. グローバルホットキー実装
- [ ] `Core/HotkeyManager.cs` 作成
- [ ] Win32 API `RegisterHotKey` / `UnregisterHotKey` 使用
- [ ] デフォルトキー設定
  - カウンター増加: `Ctrl+Shift+↑`
  - カウンター減少: `Ctrl+Shift+↓`
  - リセット: `Ctrl+Shift+R`
- [ ] キー競合チェック
- [ ] App.xaml.csに統合

#### 2. 設定の永続化
- [ ] `Core/ConfigManager.cs` 実装
- [ ] JSON設定ファイル（config.json）
  - カウンター設定（初期値、前回値復元）
  - ホットキー設定
  - 表示設定（今後拡張）
- [ ] 起動時の設定読み込み
- [ ] 設定変更時の自動保存

#### 3. WPF設定画面の拡張
- [ ] ホットキー設定タブの実装
  - キー入力待機機能
  - キー競合表示
  - 設定保存機能
- [ ] カウンター設定タブの追加
  - 初期値設定
  - 前回値復元ON/OFF
- [ ] 「設定」タブの有効化

### 優先度：中

#### 4. アニメーション強化
- [ ] スライドイン演出（上下方向）
- [ ] パーティクルエフェクト（Canvas）
- [ ] アニメーション速度設定対応
- [ ] obs.cssの拡張

#### 5. エラーハンドリング強化
- [ ] ポート使用中エラーの詳細表示
- [ ] WebSocket切断時の再接続ロジック改善
- [ ] ログ出力機能
- [ ] エラーダイアログの統一

### 優先度：低

#### 6. アイコンの作成
- [ ] `Resources/icon.ico` 作成
- [ ] 複数サイズのアイコン含む
- [ ] TrayIconに適用

#### 7. 単体テストの追加
- [ ] CounterStateのテスト
- [ ] APIHandlerのテスト

---

## 🐛 既知の問題・制限事項

### 解決済み
- ✅ ポート競合エラー → HTTPとWebSocketでポート分離（8765, 8766）
- ✅ Application型のあいまいな参照 → エイリアス使用
- ✅ HTML/CSS/JSが埋め込みコード → 外部ファイル化完了
- ✅ 名前空間の衝突 → Core/Server/UIに分離

### 未解決
- ⚠️ 設定の永続化未対応（再起動で初期化される）
- ⚠️ グローバルホットキー未実装（現在はGUI/ブラウザ経由のみ）
- ⚠️ WebSocketSharpの警告（AddWebSocketService deprecatedメッセージ）
- ⚠️ アイコンが仮アイコン（SystemIcons.Application）

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

<!-- wwwroot フォルダを出力ディレクトリにコピー -->
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
```

### 2. WPF設定画面テスト
```
1. タスクトレイアイコンをダブルクリック
2. WPF設定画面が表示される
3. 「カウンター操作」タブで+/-/リセットが動作する
4. 「接続情報」タブでURL表示＆コピーができる
5. ウィンドウを閉じても終了せず非表示化される
```

### 3. ブラウザ管理画面テスト
```
1. タスクトレイアイコンを右クリック
2. 「管理ページを開く」をクリック
3. ブラウザで http://localhost:8765/ が開く
4. 「✓ 接続中」と表示される
5. +/-/リセットボタンが動作する
6. カウンターがリアルタイム更新される
```

### 4. OBS表示テスト
```
1. ブラウザで http://localhost:8765/obs.html を開く
2. カウンター値が表示される
3. WPFまたはブラウザ管理画面でボタンを押すと即座に更新される
4. 数値変化時にフラッシュエフェクトが発生
```

---

## 📝 重要な設計決定

### 1. 名前空間設計
```csharp
CounterCounter              // ルート（App.xaml.cs のみ）
├── CounterCounter.Core     // コア機能（カウンター状態管理）
├── CounterCounter.Server   // サーバー機能（HTTP/WebSocket）
└── CounterCounter.UI       // UI機能（WPF/TrayIcon）
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
- `CounterState` は `lock` でスレッドセーフを保証
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
```

**理由**: WPFとWinFormsの名前空間衝突を回避

---

## 🔄 次のセッションで実装すべきこと

### 最優先タスク（順番に）

1. **グローバルホットキー実装** (`Core/HotkeyManager.cs`)
   - これがないと配信中に使いにくい
   - Win32 API の実装例は要求仕様書に記載あり
   - 約50-80行のクラス

2. **設定の永続化** (`Core/ConfigManager.cs`)
   - config.json で設定保存
   - ホットキー設定を保存できるようにする
   - 約80-120行のクラス

3. **WPF設定画面の拡張** (`UI/MainWindow.xaml`)
   - ホットキー設定タブの実装
   - カウンター設定タブの追加
   - 「設定」タブの有効化

### 実装の優先順位
```
グローバルホットキー → 設定永続化 → WPF拡張 → アニメーション強化 → EXE化
```

---

## 📚 参考情報

### コードの場所
- **GitHub**: （リポジトリURL）
- **要求仕様書**: `REQUIREMENTS.md`
- **タスク管理表**: `TASKS.md`

### 外部リンク
- [WebSocketSharp GitHub](https://github.com/sta/websocket-sharp)
- [RegisterHotKey API](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey)
- [WPF チュートリアル](https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/)

---

## 💡 開発のヒント

### グローバルホットキー実装時の注意
```csharp
// HwndSource を使ってWPFでWin32メッセージを受け取る
protected override void OnSourceInitialized(EventArgs e)
{
    base.OnSourceInitialized(e);
    var helper = new WindowInteropHelper(this);
    var source = HwndSource.FromHwnd(helper.Handle);
    source.AddHook(HwndHook);
}
```

### 設定ファイルのパス
```csharp
// 実行ファイルと同じディレクトリに保存
string configPath = Path.Combine(
    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
    "config.json"
);
```

### WebSocketSharpの警告について
```csharp
// 現在の実装
_server.AddWebSocketService<CounterWebSocketService>("/ws", 
    () => new CounterWebSocketService(_counterState));

// 推奨される新しい方法（要調査）
_server.AddWebSocketService("/ws", 
    () => new CounterWebSocketService(_counterState));
```

---

## 🎉 完成イメージ

最終的に以下ができるアプリ：
1. ✅ 起動すると自動でタスクトレイに常駐
2. ⏳ グローバルホットキーでゲーム中でもカウンター操作（未実装）
3. ✅ OBSブラウザソースでリアルタイム表示
4. ✅ スタイリッシュなWPF設定画面
5. ⏳ 簡単な設定管理（未実装）
6. ⏳ 単一EXEで配布可能（未実装）

---

## 📞 引き継ぎ時の確認事項

次のセッションで開発を続ける場合、以下を確認：
- [x] プロジェクト構造の理解（Core/Server/UI）
- [x] 名前空間の整理状況
- [x] wwwrootフォルダの配置
- [ ] 次の実装対象（グローバルホットキー）
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

---

**作成者**: Claude (Anthropic)  
**最終更新**: 2026-01-04 (セッション2)  
**バージョン**: v0.2-alpha