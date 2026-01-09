// ================================================================================
// Counter Counter - OBS表示用JavaScript (スライドイン表示)
// ================================================================================
//
// 【このファイルについて】
// このJavaScriptファイルは、サーバーからカウンターの値をリアルタイムで受信し、
// OBS画面に表示するためのものです。
//
// 【カスタマイズ方法】
// 1. wwwroot/themes/フォルダに新しいテーマフォルダを作成
//    例: wwwroot/themes/my_theme/
//
// 2. その中にjs/obs.jsを配置
//    例: wwwroot/themes/my_theme/js/obs.js
//
// 3. Counter Counterの「OBS設定」画面でテーマを選択
//
// 【カスタマイズのポイント】
// - アニメーション効果の変更（renderCounters, updateCounter）
// - 表示レイアウトの変更（HTML構造）
// - カウンター値表示のフォーマット変更
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
let counters = [];

// 現在表示中のカウンターID
let currentVisibleCounterId = null;

// スライドイン表示のタイマー
let slideInInterval = null;

// カウンター表示間隔（ミリ秒）
// HTML側のdata-slidein-interval属性から読み取ります
let intervalDuration = 5000;

// ================================================================================
// 初期化処理
// ================================================================================

/**
 * 初期化関数
 * ページ読み込み完了時に実行されます
 */
function init() {
    console.log('OBS Display: Initializing...');

    // HTML body要素からスライドイン表示間隔を取得
    // 例: <body data-slidein-interval="5000">
    intervalDuration = parseInt(document.body.dataset.slideinInterval) || 5000;

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
    console.log('OBS Display: Connecting to WebSocket on port', wsPort);

    // WebSocket接続を確立
    ws = new WebSocket(`ws://localhost:${wsPort}/ws`);

    // ===== 接続成功時 =====
    ws.onopen = () => {
        console.log('OBS Display: WebSocket connected');
    };

    // ===== メッセージ受信時 =====
    ws.onmessage = (event) => {
        console.log('OBS Display: Received message', event.data);

        // JSON形式のデータをパース
        const data = JSON.parse(event.data);

        // メッセージタイプによって処理を分岐
        if (data.type === 'init') {
            // 初期化メッセージ: 全カウンターデータを受信
            console.log('OBS Display: Init message received with counters:', data.counters);
            counters = data.counters;
            renderCounters(); // カウンターを画面に描画

        } else if (data.type === 'counter_update') {
            // カウンター更新メッセージ: 特定のカウンター値が変更された
            console.log('OBS Display: Counter update received', data);
            updateCounter(data); // カウンター値を更新
        }
    };

    // ===== 接続切断時 =====
    ws.onclose = () => {
        console.log('OBS Display: WebSocket disconnected, reconnecting in 5 seconds...');
        // 5秒後に再接続を試行
        setTimeout(connectWebSocket, 5000);
    };

    // ===== エラー発生時 =====
    ws.onerror = (error) => {
        console.error('OBS Display: WebSocket error:', error);
    };
}

// ================================================================================
// カウンター描画
// ================================================================================

/**
 * カウンターをHTML要素として描画
 * サーバーから受信した全カウンターを画面に配置します
 */
function renderCounters() {
    console.log('OBS Display: Rendering counters', counters);

    // カウンター表示エリアの要素を取得
    const container = document.getElementById('counter-display');
    if (!container) {
        console.error('OBS Display: counter-display element not found!');
        return;
    }

    // 既存の内容をクリア
    container.innerHTML = '';

    // カウンターが1つもない場合
    if (counters.length === 0) {
        console.warn('OBS Display: No counters to display');
        container.innerHTML = '<div style="color: white; text-align: center; padding: 20px;">カウンターがありません</div>';
        return;
    }

    // 各カウンターをループで描画
    counters.forEach(counter => {
        console.log('OBS Display: Creating counter element for', counter.Name);

        // カウンター用のdiv要素を作成
        const div = document.createElement('div');
        div.className = 'counter-item'; // CSSで指定したクラス名
        div.id = `counter-${counter.Id}`; // カウンターごとにユニークなID

        // カウンターの内容を設定
        // カスタマイズポイント: ここのHTMLを変更すると表示レイアウトを変更できます
        div.innerHTML = `
            <div class="counter-name" style="color: ${counter.Color}">${escapeHtml(counter.Name)}</div>
            <div class="counter-value" style="color: ${counter.Color}">${counter.Value}</div>
        `;

        // コンテナに追加
        container.appendChild(div);
    });

    console.log('OBS Display: Rendering complete');
}

// ================================================================================
// カウンター表示制御
// ================================================================================

/**
 * 現在表示中のカウンターを非表示にする
 */
function hideCurrentCounter() {
    if (currentVisibleCounterId) {
        const currentEl = document.getElementById(`counter-${currentVisibleCounterId}`);
        if (currentEl) {
            currentEl.classList.remove('visible'); // visibleクラスを削除 → opacity: 0
            console.log('OBS Display: Hiding current counter', currentVisibleCounterId);
        }
        currentVisibleCounterId = null;
    }
}

/**
 * カウンター値が更新された時の処理
 * 
 * @param {Object} data - サーバーから受信したカウンター更新データ
 * @param {string} data.counterId - 更新されたカウンターのID
 * @param {number} data.value - 新しい値
 * @param {Object} data.counter - カウンターの完全なデータ
 */
function updateCounter(data) {
    console.log('OBS Display: Updating counter', data.counterId, 'to value', data.value);

    // counters配列内の対象カウンターを検索
    const index = counters.findIndex(c => c.Id === data.counterId);
    if (index !== -1) {
        // 古い値を保存（アニメーション判定用）
        const oldValue = counters[index].Value;

        // カウンターデータを更新
        counters[index] = data.counter;

        // 既存の表示タイマーをクリア
        if (slideInInterval) {
            clearTimeout(slideInInterval);
            slideInInterval = null;
        }

        // 現在表示中のカウンターを非表示
        hideCurrentCounter();

        // 更新されたカウンターの要素を取得
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

            // カウンターを表示
            el.classList.add('visible');
            currentVisibleCounterId = data.counterId;
            console.log('OBS Display: Showing counter', data.counterId, 'for', intervalDuration, 'ms');

            // 一定時間後に自動的に非表示
            slideInInterval = setTimeout(() => {
                el.classList.remove('visible');
                currentVisibleCounterId = null;
                console.log('OBS Display: Counter hidden after interval');
            }, intervalDuration);
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
    console.log('OBS Display: DOM loaded');
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
   --- 例2: アニメーションを変更 ---
   
   // 値が増加した時に回転アニメーションを追加（CSSに@keyframes rotateを追加必要）
   if (data.counter.Value > oldValue) {
       valueEl.classList.add('rotate');
   }
*/

/*
   --- 例3: 効果音を追加 ---
   
   // updateCounter関数内で効果音を再生
   const audio = new Audio('/sounds/beep.mp3');
   audio.play();
*/

/*
   --- 例4: 特定の値で特別な表示 ---
   
   if (data.counter.Value === 100) {
       valueEl.textContent = '達成！';
       valueEl.style.fontSize = '10em';
   }
*/