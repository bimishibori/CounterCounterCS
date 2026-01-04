// CounterCounter/wwwroot/js/obs.js

let ws;
const counterEl = document.getElementById('counter');

function connectWebSocket() {
    const wsPort = location.port ? parseInt(location.port) + 1 : 8766;
    ws = new WebSocket(`ws://localhost:${wsPort}/ws`);

    ws.onopen = () => {
        console.log('WebSocket接続成功');
    };

    ws.onmessage = (event) => {
        const data = JSON.parse(event.data);
        if (data.type === 'counter_update') {
            counterEl.textContent = data.value;

            counterEl.classList.add('flash');
            setTimeout(() => counterEl.classList.remove('flash'), 300);
        }
    };

    ws.onclose = () => {
        console.log('WebSocket切断、再接続します...');
        setTimeout(connectWebSocket, 3000);
    };

    ws.onerror = (error) => {
        console.error('WebSocketエラー:', error);
    };
}

connectWebSocket();