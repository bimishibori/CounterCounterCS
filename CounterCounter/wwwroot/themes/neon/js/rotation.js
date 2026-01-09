// ================================================================================
// Counter Counter - OBS表示用JavaScript (ローテーション表示)
// ================================================================================
//
// 【このファイルについて】
// このJavaScriptファイルは、サーバーからカウンターの値をリアルタイムで受信し、
// OBS画面にローテーション表示するためのものです。
//
// 【スライドイン表示との違い】
// - スライドイン (obs.js): カウンター値が変わった時だけ表示
// - ローテーション (rotation.js): 常にカウンターを表示し、定期的に切り替え
//
// 【カスタマイズ方法】
// 1. wwwroot/themes/フォルダに新しいテーマフォルダを作成
//    例: wwwroot/themes/my_theme/
//
// 2. その中にjs/rotation.jsを配置
//    例: wwwroot/themes/my_theme/js/rotation.js
//
// 3. Counter Counterの「OBS設定」画面でテーマを選択
//
// 【カスタマイズのポイント】
// - アニメーション効果の変更（showCounter, rotateToNext）
// - 表示レイアウトの変更（HTML構造 in renderCounters）
// - カウンター値表示のフォーマット変更
// - ローテーション順序の変更（rotateToNext関数）
//
// 【注意事項】
// - WebSocket接続処理は変更しないでください（connectWebSocket関数）
// - HTMLのclass名（counter-item, counter-name, counter-value）は維持してください
// - CSSと連携しているため、クラス名変更すると表示が崩れます
//
// ================================================================================

// ================================================================================
// グローバル変数
// ================================================================================

// WebSocket接続オブジェクト
let ws = null;

// カウンターデータの配列（サーバーから受信）
// ShowInRotation = true のカウンターのみ格納
let counters = [];

// 現在表示中のカウンターのインデックス（counters配列内）
let currentIndex = 0;

// ローテーション用タイマー
let rotationInterval = null;

// カウンター切り替え間隔（ミリ秒）
// HTML側のdata-rotation-interval属性から読み取ります
let intervalDuration = 5000;

// ================================================================================
// 初期化処理
// ================================================================================

/**
 * 初期化関数
 * ページ読み込み完了時に実行されます
 */
function init() {
    console.log('Rotation Display: Initializing...');

    // HTML body要素からローテーション間隔を取得
    // 例: <body data-rotation-interval="5000">
    intervalDuration = parseInt(document.body.dataset.rotationInterval) || 5000;

    // WebSocket接続開始
    connectWebSocket();
}

// ================================================================================
// WebSocket接続
// ================================================================================

/**
 * WebSocketサーバーへ接続
 * カウンター値の更新をリアルタイムで受信します
 */
function connectWebSocket() {
    // HTML body要素からWebSocketポート番号を取得
    const wsPort = parseInt(document.body.dataset.wsPort);
    console.log('Rotation Display: Connecting to WebSocket on port', wsPort);

    // WebSocket接続を確立
    ws = new WebSocket(`ws://localhost:${wsPort}/ws`);

    // ===== 接続成功時 =====
    ws.onopen = () => {
        console.log('Rotation Display: WebSocket connected');
    };

    // ===== メッセージ受信時 =====
    ws.onmessage = (event) => {
        console.log('Rotation Display: Received message', event.data);

        // JSON形式のデータをパース
        const data = JSON.parse(event.data);

        // メッセージタイプによって処理を分岐
        if (data.type === 'init') {
            // 初期化メッセージ: 全カウンターデータを受信
            console.log('Rotation Display: Init message received with counters:', data.counters);
            // ShowInRotation = true のカウンターのみフィルタリング
            counters = data.counters.filter(c => c.ShowInRotation);
            renderCounters(); // カウンターを画面に描画
            startRotation(); // ローテーション開始

        } else if (data.type === 'counter_update') {
            // カウンター更新メッセージ: 特定のカウンター値が変更された
            console.log('Rotation Display: Counter update received', data);
            updateCounter(data); // カウンター値を更新

        } else if (data.type === 'next_rotation') {
            // 次のカウンターへ強制切り替えメッセージ
            console.log('Rotation Display: Next rotation triggered');
            rotateToNext(); // 次のカウンターへ

        } else if (data.type === 'force_display') {
            // 特定のカウンターを強制表示メッセージ
            console.log('Rotation Display: Force display requested for', data.counterId);
            forceDisplay(data.counterId); // 指定されたカウンターを表示
        }
    };

    // ===== 接続切断時 =====
    ws.onclose = () => {
        console.log('Rotation Display: WebSocket disconnected, reconnecting in 5 seconds...');
        stopRotation(); // ローテーション停止
        // 5秒後に再接続を試行
        setTimeout(connectWebSocket, 5000);
    };

    // ===== エラー発生時 =====
    ws.onerror = (error) => {
        console.error('Rotation Display: WebSocket error:', error);
    };
}

// ================================================================================
// カウンター描画
// ================================================================================

/**
 * カウンターをHTML要素として描画
 * サーバーから受信したカウンター（ShowInRotation = true）を画面に配置します
 */
function renderCounters() {
    console.log('Rotation Display: Rendering counters', counters);

    // カウンター表示エリアの要素を取得
    const container = document.getElementById('counter-display');
    if (!container) {
        console.error('Rotation Display: counter-display element not found!');
        return;
    }

    // 既存の内容をクリア
    container.innerHTML = '';

    // カウンターが1つもない場合
    if (counters.length === 0) {
        console.warn('Rotation Display: No counters to display');
        container.innerHTML = '<div style="color: white; text-align: center; padding: 20px;">カウンターがありません</div>';
        return;
    }

    // 各カウンターをループで描画
    counters.forEach((counter, index) => {
        console.log('Rotation Display: Creating counter element for', counter.Name);

        // カウンター用のdiv要素を作成
        const div = document.createElement('div');
        div.className = 'counter-item'; // CSSで指定したクラス名
        div.id = `counter-${counter.Id}`; // カウンターごとにユニークなID

        // カウンターの内容を設定
        // カスタマイズポイント: ここのHTMLを変更すると表示レイアウトを変更できます
        // 例: 縦並びにしたい場合は .counter-display-line を削除し、divを分ける
        div.innerHTML = `
            <div class="counter-display-line">
                <div class="counter-name" style="color: ${counter.Color}">${escapeHtml(counter.Name)}</div>
                <div class="counter-value" style="color: ${counter.Color}">${counter.Value}</div>
            </div>
        `;

        // コンテナに追加
        container.appendChild(div);
    });

    // 最初のカウンターを表示
    showCounter(currentIndex);
    console.log('Rotation Display: Rendering complete');
}

// ================================================================================
// カウンター表示制御
// ================================================================================

/**
 * 指定されたインデックスのカウンターを表示
 * 他のカウンターは非表示にします
 * 
 * @param {number} index - 表示するカウンターのインデックス
 */
function showCounter(index) {
    const container = document.getElementById('counter-display');
    if (!container) return;

    // すべてのカウンター要素を取得
    const items = container.querySelectorAll('.counter-item');

    // 各カウンターをループ
    items.forEach((item, i) => {
        if (i === index) {
            // 指定されたインデックスのカウンター: activeクラスを追加 → 表示
            item.classList.add('active');
        } else {
            // それ以外: activeクラスを削除 → 非表示
            item.classList.remove('active');
        }
    });
}

// ================================================================================
// ローテーション制御
// ================================================================================

/**
 * ローテーションを開始
 * 指定された間隔で自動的にカウンターを切り替えます
 */
function startRotation() {
    // カウンターが1つ以下の場合はローテーション不要
    if (counters.length <= 1) {
        console.log('Rotation Display: Only one or no counters, rotation disabled');
        return;
    }

    // 既存のタイマーを停止
    stopRotation();

    // 定期実行タイマーを設定
    rotationInterval = setInterval(() => {
        rotateToNext(); // 次のカウンターへ
    }, intervalDuration);

    console.log('Rotation Display: Rotation started with interval', intervalDuration, 'ms');
}

/**
 * 次のカウンターへ切り替え
 * カスタマイズポイント: ここでローテーション順序を変更できます
 */
function rotateToNext() {
    // 次のインデックスへ（配列の最後に達したら最初に戻る）
    currentIndex = (currentIndex + 1) % counters.length;

    // カスタマイズ例: ランダムに切り替え
    // currentIndex = Math.floor(Math.random() * counters.length);

    // カスタマイズ例: 逆順に切り替え
    // currentIndex = (currentIndex - 1 + counters.length) % counters.length;

    // カウンターを表示
    showCounter(currentIndex);
    console.log('Rotation Display: Switched to counter index', currentIndex);
}

/**
 * ローテーションを停止
 */
function stopRotation() {
    if (rotationInterval) {
        clearInterval(rotationInterval);
        rotationInterval = null;
        console.log('Rotation Display: Rotation stopped');
    }
}

/**
 * 特定のカウンターを強制表示
 * ローテーションを一旦停止し、指定されたカウンターを表示後、再開します
 * 
 * @param {string} counterId - 表示するカウンターのID
 */
function forceDisplay(counterId) {
    // 対象カウンターのインデックスを検索
    const index = counters.findIndex(c => c.Id === counterId);
    if (index === -1) {
        console.warn('Rotation Display: Counter not found for force display', counterId);
        return;
    }

    // ローテーションを一旦停止
    stopRotation();

    // 指定されたカウンターを表示
    currentIndex = index;
    showCounter(currentIndex);
    console.log('Rotation Display: Force displayed counter', counterId, 'at index', index);

    // ローテーションを再開
    startRotation();
}

// ================================================================================
// カウンター更新
// ================================================================================

/**
 * カウンター値が更新された時の処理
 * 
 * @param {Object} data - サーバーから受信したカウンター更新データ
 * @param {string} data.counterId - 更新されたカウンターのID
 * @param {number} data.value - 新しい値
 * @param {Object} data.counter - カウンターの完全なデータ
 */
function updateCounter(data) {
    console.log('Rotation Display: Updating counter', data.counterId, 'to value', data.value);

    // ShowInRotation が有効な場合
    if (data.counter.ShowInRotation) {
        // counters配列内の対象カウンターを検索
        const index = counters.findIndex(c => c.Id === data.counterId);

        if (index === -1) {
            // 新規カウンター: 配列に追加し、再描画
            counters.push(data.counter);
            renderCounters();
            startRotation(); // ローテーション再起動
        } else {
            // 既存カウンター: 値を更新
            const oldValue = counters[index].Value; // 古い値を保存
            counters[index] = data.counter; // データ更新

            // 画面上の要素を取得
            const el = document.getElementById(`counter-${data.counterId}`);
            if (el) {
                // カウンター値の要素を取得
                const valueEl = el.querySelector('.counter-value');
                if (valueEl) {
                    // 値を更新
                    valueEl.textContent = data.counter.Value;

                    // 古いアニメーションクラスを削除
                    valueEl.classList.remove('flash', 'slide-up', 'slide-down');

                    // ===== アニメーション判定 =====
                    // 増加した場合: 下から上へスライド
                    if (data.counter.Value > oldValue) {
                        valueEl.classList.add('slide-up');
                    }
                    // 減少した場合: 上から下へスライド
                    else if (data.counter.Value < oldValue) {
                        valueEl.classList.add('slide-down');
                    }

                    // フラッシュエフェクトを追加
                    valueEl.classList.add('flash');

                    // アニメーション終了後、クラスを削除
                    setTimeout(() => {
                        valueEl.classList.remove('flash', 'slide-up', 'slide-down');
                    }, 300);
                }
            }
        }
    } else {
        // ShowInRotation が無効な場合: 配列から削除
        const index = counters.findIndex(c => c.Id === data.counterId);
        if (index !== -1) {
            counters.splice(index, 1); // 配列から削除
            renderCounters(); // 再描画
            startRotation(); // ローテーション再起動
        }
    }
}

// ================================================================================
// ユーティリティ関数
// ================================================================================

/**
 * HTMLエスケープ処理
 * XSS攻撃を防ぐため、テキストをHTMLとして安全に表示できる形式に変換
 * 
 * @param {string} text - エスケープ対象のテキスト
 * @returns {string} - エスケープ済みテキスト
 */
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text; // textContentは自動的にエスケープされる
    return div.innerHTML;
}

// ================================================================================
// イベントリスナー
// ================================================================================

/**
 * DOMContentLoadedイベント
 * HTML要素の読み込みが完了したら初期化処理を実行
 */
window.addEventListener('DOMContentLoaded', () => {
    console.log('Rotation Display: DOM loaded');
    init(); // 初期化開始
});

// ================================================================================
// カスタマイズ例（コメントアウトされています）
// ================================================================================

/*
   --- 例1: カウンター値の表示形式を変更 ---
   
   // updateCounter関数内のvalueEl.textContentを変更
   valueEl.textContent = `${data.counter.Value}回`; // 「回」を追加
*/

/*
   --- 例2: ランダムにローテーション ---
   
   function rotateToNext() {
       currentIndex = Math.floor(Math.random() * counters.length);
       showCounter(currentIndex);
   }
*/

/*
   --- 例3: 値の大きい順にソート ---
   
   // renderCounters関数内で配列をソート
   counters.sort((a, b) => b.Value - a.Value);
*/

/*
   --- 例4: 特定の条件でローテーション速度変更 ---
   
   function startRotation() {
       const dynamicInterval = counters.length > 5 ? 3000 : 5000;
       rotationInterval = setInterval(() => {
           rotateToNext();
       }, dynamicInterval);
   }
*/

/*
   --- 例5: 効果音を追加 ---
   
   // rotateToNext関数内で効果音を再生
   function rotateToNext() {
       currentIndex = (currentIndex + 1) % counters.length;
       showCounter(currentIndex);
       
       const audio = new Audio('/sounds/beep.mp3');
       audio.play();
   }
*/