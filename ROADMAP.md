# Counter Counter - 将来の展望ドキュメント

**最終更新**: 2026-01-07  
**バージョン**: α1.0.0 完成記念

---

## 🎉 α版完成おめでとうございます！

べ、別にアンタのためにドキュメント作ってあげたわけじゃないんだからね！  
た、ただ...プロジェクトが完成したから記録しておくだけよ！

---

## 📊 現在の状態

### 完成機能
- ✅ 複数カウンター管理
- ✅ グローバルホットキー（複数セット対応）
- ✅ OBS連携（スライドイン表示）
- ✅ ローテーション表示
- ✅ スタイリッシュなWPF GUI
- ✅ WebSocket リアルタイム更新
- ✅ 設定ファイル永続化
- ✅ 単一EXE化

### 進捗率
**100%** - α版完成 🎊

---

## 🚀 Phase 6: 次期バージョンの機能拡張計画

### 優先度: 高

#### 1. 設定プロファイル機能【重要度: ★★★★★】

**概要**  
配信ゲームやシチュエーションごとに設定を切り替えられるようにする。

**実装内容**
- 複数の設定ファイルを保存・読み込み可能に
  - `profiles/apex_legends.json`
  - `profiles/valorant.json`
  - `profiles/minecraft.json`
- メニューバーを追加
- 「名前を付けて保存」「プロファイルを開く」機能
- プロファイル切り替え時のホットキー自動再登録
- ショートカットキーでクイック切り替え

**技術的検討事項**
- `ConfigManager` の拡張
  - `LoadProfile(string profileName)` メソッド追加
  - `SaveProfile(string profileName, CounterSettings settings)` メソッド追加
  - `ListProfiles()` メソッドでプロファイル一覧取得
- プロファイル管理用の新しいビュー `ProfileManagementView.xaml` 作成
- 設定ファイル保存先: `profiles/` フォルダ

**データ構造例**
```json
{
  "ProfileName": "Apex Legends",
  "Counters": [...],
  "Hotkeys": [...],
  "ServerPort": 9000,
  "CustomCss": "profiles/apex_legends/style.css",
  "CustomJs": "profiles/apex_legends/script.js"
}
```

**UI/UX改善**
- 現在のプロファイル名をタイトルバーに表示
- クイック切り替え用のホットキー設定（例: Ctrl+Shift+1~9）

---

#### 2. カウンター見た目カスタマイズ機能【重要度: ★★★★☆】

**概要**  
エンドユーザーがカウンターの見た目を自由にカスタマイズできるようにする。

**実装内容**
- カスタムCSS・JavaScript読み込み機能
- プリセットテーマの提供（5種類程度）
  - デフォルト（現行）
  - ネオン風
  - レトロゲーム風
  - ミニマル
  - 3D風
- テーマプレビュー機能
- カスタムフォント対応

**技術的検討事項**
- `wwwroot/themes/` フォルダ構造
  ```
  wwwroot/
  ├─ themes/
  │  ├─ default/
  │  │  ├─ obs.css
  │  │  └─ obs.js
  │  ├─ neon/
  │  │  ├─ obs.css
  │  │  └─ obs.js
  │  └─ retro/
  │     ├─ obs.css
  │     └─ obs.js
  ```
- `HtmlContentProvider` の拡張
  - テーマパラメータ追加
  - カスタムCSS/JS パス指定
- テーマ選択UI (`ThemeSettingsView.xaml`)

**カスタマイズ可能項目**
- フォント（種類・サイズ・ウェイト）
- 色（テキスト・背景・アクセント）
- アニメーション（速度・エフェクト）
- レイアウト（横並び・縦並び・中央揃え）
- 影・グロー効果
- 背景画像対応

---

#### 3. プロファイル連動カスタマイズ【重要度: ★★★★☆】

**概要**  
プロファイルごとに異なるカウンターデザインを保存・適用。

**実装内容**
- プロファイル保存時にテーマ設定も含める
- プロファイル切り替え時にテーマも自動切り替え
- 「このプロファイル用のカスタムテーマを作成」ボタン

**データ構造拡張**
```json
{
  "ProfileName": "Apex Legends",
  "Counters": [...],
  "Hotkeys": [...],
  "Theme": {
    "Name": "Apex Custom",
    "CssPath": "profiles/apex_legends/custom.css",
    "JsPath": "profiles/apex_legends/custom.js",
    "FontFamily": "Arial Black",
    "PrimaryColor": "#ff0000",
    "SecondaryColor": "#000000"
  }
}
```

---

### 優先度: 中

#### 4. アナログメーター機能【重要度: ★★★☆☆】

**概要**  
数値だけでなく、スライダーやゲージのようなアナログ表示を追加。

**実装内容**
- カウンタータイプの追加
  - デジタル（既存）
  - プログレスバー
  - 円形ゲージ
  - スライダー
  - パーセンテージ表示
- 最大値・最小値設定
- 目標値設定（目標達成時のエフェクト）

**技術的検討事項**
- `Counter.cs` にタイプフィールド追加
  ```csharp
  public enum CounterType
  {
      Digital,
      ProgressBar,
      CircularGauge,
      Slider,
      Percentage
  }
  
  public class Counter
  {
      public CounterType Type { get; set; }
      public int MinValue { get; set; }
      public int MaxValue { get; set; }
      public int TargetValue { get; set; }
  }
  ```
- Canvas / SVG でのゲージ描画実装
- アニメーション対応（ゲージが滑らかに変化）

**UI/UX改善**
- `CounterEditDialog` でタイプ選択
- タイプごとの設定項目表示切り替え
- プレビュー機能

---

### 優先度: 低（将来的に検討）

#### 5. コメント連動機能【重要度: ★★☆☆☆】

**概要**  
配信コメントを拾ってワードのカウントを行う。

**実装内容**
- Twitch / YouTube Chat API連携
- キーワード検出機能
- コメント数カウンター
- 特定ワード出現回数カウント
- NGワードフィルタ

**技術的検討事項**
- 外部API連携（Twitch IRC / YouTube Data API）
- WebSocket での配信サービス接続
- コメント解析エンジン実装
- キーワードマッチングロジック

**データ構造例**
```json
{
  "CommentTracking": {
    "Enabled": true,
    "Platform": "Twitch",
    "Channel": "your_channel",
    "Keywords": [
      {
        "Keyword": "草",
        "CounterId": "counter-1",
        "Action": "Increment"
      }
    ]
  }
}
```

**注意事項**
- 配信サービスのAPI利用規約確認必須
- レート制限対応
- OAuth認証実装
- プライバシー配慮

---

## 📁 推奨ディレクトリ構造（Phase 6対応版）

```
CounterCounter/
├── CounterCounter.sln
├── CounterCounter/
│   ├── Core/
│   │   ├── CounterManager.cs
│   │   ├── HotkeyManager.cs
│   │   ├── ConfigManager.cs
│   │   └── ProfileManager.cs
│   ├── Server/
│   │   ├── WebServer.cs
│   │   ├── WebSocketServer.cs
│   │   ├── ApiHandler.cs
│   │   ├── HtmlContentProvider.cs
│   │   └── ThemeProvider.cs
│   ├── UI/
│   │   ├── MainWindow.xaml(.cs)
│   │   ├── Dialogs/
│   │   │   ├── CounterEditDialog.xaml(.cs)
│   │   │   └── ThemeEditorDialog.xaml(.cs)
│   │   ├── Views/
│   │   │   ├── CounterManagementView.xaml(.cs)
│   │   │   ├── ProfileManagementView.xaml(.cs)
│   │   │   ├── ThemeSettingsView.xaml(.cs)
│   │   │   ├── ServerSettingsView.xaml(.cs)
│   │   │   ├── HotkeySettingsView.xaml(.cs)
│   │   │   └── ConnectionInfoView.xaml(.cs)
│   │   └── Components/
│   │       └── CounterCard.xaml(.cs)
│   ├── Models/
│   │   ├── Counter.cs
│   │   ├── Profile.cs
│   │   ├── Theme.cs
│   │   ├── HotkeySettings.cs
│   │   └── CounterSettings.cs
│   ├── wwwroot/
│   │   ├── themes/
│   │   │   ├── default/
│   │   │   ├── neon/
│   │   │   ├── retro/
│   │   │   └── minimal/
│   │   ├── obs.html
│   │   ├── rotation.html
│   │   ├── css/
│   │   └── js/
│   ├── profiles/
│   │   ├── default.json
│   │   ├── apex_legends.json
│   │   └── valorant.json
│   └── Resources/
│       └── icon.ico
├── README.md
├── REQUIREMENTS.md
├── HANDOVER.md
├── TASKS.md
└── ROADMAP.md
```

---

## 🛠️ 実装優先順位

### フェーズ6.1: 基盤整備（2週間）
1. プロファイル機能の実装
2. `ProfileManager.cs` の作成
3. GUI にプロファイル管理機能追加

### フェーズ6.2: カスタマイズ機能（3週間）
1. テーマシステムの実装
2. プリセットテーマ5種類作成
3. テーマエディタUI実装

### フェーズ6.3: 連動機能（2週間）
1. プロファイル⇔テーマ連動
2. テーマプレビュー機能
3. カスタムテーマ保存機能

### フェーズ6.4: 拡張機能（検討中）
1. アナログメーター実装
2. コメント連動（将来的に）

---

## 📝 技術的課題と解決策

### 課題1: プロファイル切り替え時のパフォーマンス

**問題**  
プロファイル切り替え時にホットキー再登録、WebSocket再接続が必要。

**解決策**
- ホットキーの差分更新
- WebSocket接続は維持し、設定のみ更新
- 非同期処理で UI をブロックしない

### 課題2: カスタムテーマのセキュリティ

**問題**  
ユーザーが任意のJS/CSSを読み込むとセキュリティリスク。

**解決策**
- サンドボックス環境でテーマを実行
- 危険なAPIへのアクセスを制限
- テーマレビューシステム（将来的に）

### 課題3: プロファイル管理の UX

**問題**  
プロファイルが増えると選択が大変。

**解決策**
- お気に入り機能
- タグ付け・分類機能
- 最近使用したプロファイル表示
- ショートカットキーでクイック切り替え（Ctrl+Shift+1~9）

---

## 🎨 デザインガイドライン（Phase 6）

### テーマ作成時の注意事項

1. **パフォーマンス**
   - 重いアニメーションは避ける
   - 60fps 維持を目標
   - Canvas よりCSS優先

2. **互換性**
   - すべてのカウンタータイプに対応
   - ローテーション表示でも正常動作
   - 複数カウンター同時表示対応

3. **アクセシビリティ**
   - コントラスト比 4.5:1 以上確保
   - 読みやすいフォントサイズ（最小16px推奨）
   - 色覚異常対応（色だけに頼らない）

4. **コーディング規約**
   - BEM記法でCSS命名
   - カスタムプロパティ（CSS変数）活用
   - モバイル対応は不要（OBS専用）

### プリセットテーマ仕様（案）

#### テーマ1: デフォルト（現行）
- シンプル・視認性重視
- ダークな背景に明るい文字
- フラッシュ・スライドアニメーション

#### テーマ2: ネオン
- サイバーパンク風
- 蛍光色・グロー効果
- 電光掲示板風フォント

#### テーマ3: レトロゲーム
- ドット絵風フォント
- 8bit風カラーパレット
- ピクセルアート背景

#### テーマ4: ミニマル
- 極限まで装飾を削減
- モノクロ配色
- アニメーションなし（軽量）

#### テーマ5: 3D風
- 立体的なデザイン
- シャドウ・ハイライト多用
- 回転・遠近効果

---

## 📚 参考資料

### プロファイル機能の参考
- Visual Studio Code の設定プロファイル
- OBS の シーンコレクション
- Adobe Creative Cloud のワークスペース

### テーマシステムの参考
- VSCode のカラーテーマ
- Discord のテーマ機能
- Streamlabs OBS のテーマ
- Firefox / Chrome の拡張テーマ

### ゲージ実装の参考
- Chart.js のゲージチャート
- D3.js のゲージサンプル
- CSS3 アニメーション（transform, transition）
- Canvas API によるリアルタイム描画

---

## 🤝 コントリビューション（将来的に）

将来的にはコミュニティテーマの投稿・共有プラットフォームも検討。

**構想**
- GitHub でテーマリポジトリ管理
- テーママーケットプレイス
- ユーザー投票・レビューシステム
- テーマ作成ガイド・テンプレート提供

**テーマ投稿の条件（案）**
- MIT ライセンスまたは CC0
- パフォーマンステスト合格
- 全カウンタータイプ対応
- アクセシビリティ基準クリア

---

## ⚠️ 注意事項

- Phase 6 の機能は**構想段階**
- 実装時に仕様変更の可能性あり
- ユーザーフィードバックを重視
- パフォーマンスを犠牲にしない
- α版との互換性維持

---

## 🗓️ 開発スケジュール（暫定）

### 2026年 Q1（1-3月）
- α版リリース ✅
- ユーザーフィードバック収集
- バグ修正・マイナーアップデート

### 2026年 Q2（4-6月）
- Phase 6.1: プロファイル機能実装
- β版リリース

### 2026年 Q3（7-9月）
- Phase 6.2: テーマシステム実装
- Phase 6.3: プロファイル連動

### 2026年 Q4（10-12月）
- Phase 6.4: 拡張機能検討
- v2.0 正式リリース

---

## 📈 成功指標（KPI）

### Phase 6 完了時の目標
- プロファイル機能利用率: 70%以上
- テーマカスタマイズ率: 50%以上
- 平均起動時間: 1秒以内維持
- メモリ使用量: 100MB以下維持
- ユーザー満足度: 4.5/5以上

---

## 🎓 学習リソース

### Phase 6 開発に必要な知識
- WPF 高度な UI/UX設計
- ファイルシステム操作（プロファイル管理）
- CSS/JavaScript 動的読み込み
- Canvas API（ゲージ描画）
- 外部API連携（コメント機能）

### 推奨教材
- Microsoft Learn（WPF コース）
- MDN Web Docs（CSS/Canvas）
- Twitch / YouTube API ドキュメント

---

<div align="center">

**α版完成、本当にお疲れ様でした！**

べ、別に感動してるわけじゃないんだからね！  
次のバージョンも...頑張りなさいよね！

Made with ❤️ for Streamers

</div>