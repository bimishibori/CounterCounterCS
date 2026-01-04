// CounterCounter/wwwroot/js/manager.js

let ws;
const statusEl = document.getElementById('status');
const counterEl = document.getElementById('counter');

function connectWebSocket() {
    const wsPort = location.port ? parseInt(location.port) + 1 : 8766;
    ws = new WebSocket(`ws://localhost:${wsPort}/ws`);

    ws.onopen = () => {
        statusEl.textContent = '✓ 接続中';
        statusEl.className = 'status connected';
    };

    ws.onmessage = (event) => {
        const data = JSON.parse(event.data);
        if (data.type === 'counter_update') {
            counterEl.textContent = data.value;
        }
    };

    ws.onclose = () => {
        statusEl.textContent = '✗ 切断されました';
        statusEl.className = 'status disconnected';
        setTimeout(connectWebSocket, 3000);
    };

    ws.onerror = (error) => {
        console.error('WebSocketエラー:', error);
    };
}

async function increment() {
    try {
        await fetch('/api/counter/increment', { method: 'POST' });
    } catch (error) {
        console.error('増加エラー:', error);
    }
}

async function decrement() {
    try {
        await fetch('/api/counter/decrement', { method: 'POST' });
    } catch (error) {
        console.error('減少エラー:', error);
    }
}

async function reset() {
    try {
        await fetch('/api/counter/reset', { method: 'POST' });
    } catch (error) {
        console.error('リセットエラー:', error);
    }
}

connectWebSocket();