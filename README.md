# カウンター・カウンター (Counter Counter)

<div align="center">

![Status](https://img.shields.io/badge/Status-In%20Development-yellow)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![License](https://img.shields.io/badge/License-MIT-green)

**OBS配信者向け リアルタイムカウンターアプリケーション**

デスカウンター・勝敗カウンター・任意イベントカウンターに最適！

[特徴](#-特徴) • [スクリーンショット](#-スクリーンショット) • [インストール](#-インストール) • [使い方](#-使い方) • [開発](#-開発)

</div>

---

## 📝 概要

**カウンター・カウンター**は、OBS配信者向けに設計されたスタイリッシュなカウンターアプリケーションです。

グローバルホットキーでゲーム中でもカウンター操作が可能で、OBSのブラウザソースでリアルタイム表示できます。

---

## ✨ 特徴

### 🎯 主要機能
- ✅ **タスクトレイ常駐** - 邪魔にならず、いつでも使える
- ✅ **リアルタイム更新** - WebSocketで遅延なし
- ✅ **グローバルホットキー** - ゲーム中でも操作可能（実装予定）
- ✅ **OBS完全対応** - ブラウザソースで簡単表示
- ✅ **スタイリッシュUI** - ダークテーマの洗練されたデザイン
- ✅ **軽量動作** - メモリ使用量 < 100MB

### 🎨 演出機能
- スライドインアニメーション（実装予定）
- パーティクルエフェクト（実装予定）
- フラッシュエフェクト（実装済み）
- カスタマイズ可能な表示設定（実装予定）

### 🔧 技術的特徴
- C# + .NET 8 製
- HTTPサーバー（ローカルのみ）
- WebSocketによるリアルタイム通信
- スレッドセーフな状態管理
- 外部依存なし（単一EXE配布可能）

---

## 🖼️ スクリーンショット

### 管理画面
```
┌─────────────────────────────────┐
│  カウンター・カウンター          │
│  ✓ 接続中                        │
│                                  │
│         【 5 】                  │
│                                  │
│    [ ＋ ]  [ － ]  [ リセット ]  │
└─────────────────────────────────┘
```

### OBS表示画面
```
┌─────────────────────────────────┐
│                                  │
│                                  │
│           【 5 】                │
│                                  │
│                                  │
└─────────────────────────────────┘
```

---

## 📦 インストール

### システム要件
- **OS**: Windows 10/11 (64bit)
- **.NET**: .NET 8 Runtime（含まれる予定）
- **メモリ**: 100MB以上
- **OBS**: OBS Studio 最新版推奨

### ダウンロード
> ⚠️ 現在開発中のため、リリース版はまだありません

開発版を試す場合：
1. このリポジトリをクローン
2. Visual Studio 2022で開く
3. F5キーでビルド＆実行

---

## 🚀 使い方

### 1. 起動
`CounterCounter.exe` を実行すると、タスクトレイに常駐します。

### 2. OBSに追加
1. OBSで「ソース」→「ブラウザ」を追加
2. URLに以下を入力：
   ```
   http://localhost:8765/obs.html
   ```
3. 幅: `800px`、高さ: `600px` を推奨
4. 完了！

### 3. カウンター操作

#### ブラウザから
1. タスクトレイアイコンを右クリック
2. 「管理ページを開く」を選択
3. ブラウザでボタン操作

#### グローバルホットキー（実装予定）
- `Ctrl + Shift + ↑` : カウンター増加
- `Ctrl + Shift + ↓` : カウンター減少
- `Ctrl + Shift + R` : リセット

### 4. OBS URLのコピー
タスクトレイアイコンを右クリック → 「OBS URLをコピー」

---

## 🎮 想定用途

### ゲーム配信
- デスカウンター
- 勝利数カウンター
- コンボカウンター
- ミスカウンター

### その他
- チャレンジ企画のカウント
- 目標達成カウンター
- イベントトラッカー

---

## 🛠️ 開発

### 開発環境セットアップ

#### 必要なツール
- [Visual Studio 2022](https://visualstudio.microsoft.com/)
  - ワークロード: .NET デスクトップ開発
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

#### クローン＆ビルド
```bash
git clone https://github.com/yourusername/CounterCounter.git
cd CounterCounter
```

Visual Studio 2022で `CounterCounter.sln` を開き、F5キーで実行。

#### NuGetパッケージ
```xml
<PackageReference Include="WebSocketSharp-netstandard" Version="1.0.1" />
```

---

## 📂 プロジェクト構成

```
CounterCounter/
├── CounterCounter.sln         # ソリューションファイル
├── CounterCounter/
│   ├── App.xaml              # WPFアプリケーション定義
│   ├── App.xaml.cs           # 起動処理
│   ├── TrayIcon.cs           # タスクトレイ管理
│   ├── CounterState.cs       # カウンター状態管理
│   ├── WebServer.cs          # HTTPサーバー
│   ├── WebSocketServer.cs   # WebSocketサーバー
│   ├── HotkeyManager.cs      # グローバルホットキー（未実装）
│   ├── ConfigManager.cs      # 設定管理（未実装）
│   ├── MainWindow.xaml       # 設定GUI（未実装）
│   ├── Models/               # データモデル
│   ├── wwwroot/              # Webファイル（予定）
│   └── Resources/            # リソースファイル
├── HANDOVER.md               # 開発引き継ぎ資料
├── REQUIREMENTS.md           # 要求仕様書
└── TASKS.md                  # タスク管理表
```

---

## 📊 開発状況

### Phase 1: 環境構築 ✅ 100%
- [x] プロジェクト作成
- [x] 基本構造構築

### Phase 2: コア機能実装 🔄 50%
- [x] タスクトレイ常駐
- [x] カウンター状態管理
- [x] HTTPサーバー
- [x] WebSocketサーバー
- [ ] グローバルホットキー
- [ ] 設定管理

### Phase 3: GUI実装 ⏳ 0%
- [ ] WPF設定画面
- [ ] ホットキー設定UI
- [ ] 表示設定UI

### Phase 4: アニメーション 🔄 10%
- [x] フラッシュエフェクト（基本）
- [ ] スライドインアニメーション
- [ ] パーティクル演出

### Phase 5: EXE化・配布 ⏳ 0%
- [ ] 単一EXEビルド
- [ ] インストーラ作成

詳細は [TASKS.md](TASKS.md) を参照

---

## 🤝 コントリビューション

現在は個人開発中ですが、将来的にコントリビューションを受け付ける予定です。

---

## 📄 ライセンス

MIT License（予定）

---

## 🔗 リンク

- **要求仕様書**: [REQUIREMENTS.md](REQUIREMENTS.md)
- **開発引き継ぎ**: [HANDOVER.md](HANDOVER.md)
- **タスク管理**: [TASKS.md](TASKS.md)

---

## 📞 サポート

### 動作確認済み環境
- Windows 10 (64bit)
- Windows 11 (64bit)
- OBS Studio 30.0+

### トラブルシューティング

#### ポート8765が使用中と表示される
→ 自動で8766, 8767...と試行します。再起動してください。

#### WebSocketに接続できない
→ ファイアウォール設定を確認してください（localhostなので通常は不要）。

#### OBSで表示されない
→ ブラウザソースのURLが正しいか確認してください。

---

## 🎉 Special Thanks

- WebSocketSharp（MIT License）
- OBS Studio Community

---

<div align="center">

**開発中のため、機能や仕様は予告なく変更される可能性があります**

Made with ❤️ for Streamers

</div>