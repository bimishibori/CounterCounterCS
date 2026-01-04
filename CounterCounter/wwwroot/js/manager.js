// CounterCounter/wwwroot/js/manager.js
let ws = null;
let counters = [];

function init() {
    loadCounters();
    connectWebSocket();
}

function connectWebSocket() {
    const wsPort = parseInt(document.body.dataset.wsPort);
    ws = new WebSocket(`ws://localhost:${wsPort}/ws`);

    ws.onopen = () => {
        console.log('WebSocket connected');
        updateConnectionStatus(true);
    };

    ws.onmessage = (event) => {
        const data = JSON.parse(event.data);

        if (data.type === 'init') {
            counters = data.counters;
            renderCounters();
        } else if (data.type === 'counter_update') {
            const index = counters.findIndex(c => c.Id === data.counterId);
            if (index !== -1) {
                counters[index] = data.counter;
                renderCounters();
            }
        }
    };

    ws.onclose = () => {
        console.log('WebSocket disconnected');
        updateConnectionStatus(false);
        setTimeout(connectWebSocket, 5000);
    };

    ws.onerror = (error) => {
        console.error('WebSocket error:', error);
    };
}

function updateConnectionStatus(connected) {
    const statusEl = document.getElementById('connection-status');
    if (statusEl) {
        statusEl.textContent = connected ? '接続中' : '切断';
        statusEl.className = connected ? 'connected' : 'disconnected';
    }
}

async function loadCounters() {
    try {
        const response = await fetch('/api/counters');
        const data = await response.json();
        counters = data.counters;
        renderCounters();
    } catch (error) {
        console.error('Failed to load counters:', error);
    }
}

function renderCounters() {
    const container = document.getElementById('counters-container');
    if (!container) return;

    container.innerHTML = '';

    counters.forEach(counter => {
        const card = document.createElement('div');
        card.className = 'counter-card';
        card.innerHTML = `
            <div class="counter-header">
                <h3>${escapeHtml(counter.Name)}</h3>
                <div class="counter-color" style="background-color: ${counter.Color}"></div>
            </div>
            <div class="counter-value">${counter.Value}</div>
            <div class="counter-controls">
                <button onclick="incrementCounter('${counter.Id}')">+</button>
                <button onclick="decrementCounter('${counter.Id}')">-</button>
                <button onclick="resetCounter('${counter.Id}')">リセット</button>
            </div>
        `;
        container.appendChild(card);
    });
}

async function incrementCounter(id) {
    try {
        await fetch(`/api/counter/${id}/increment`, { method: 'POST' });
    } catch (error) {
        console.error('Failed to increment:', error);
    }
}

async function decrementCounter(id) {
    try {
        await fetch(`/api/counter/${id}/decrement`, { method: 'POST' });
    } catch (error) {
        console.error('Failed to decrement:', error);
    }
}

async function resetCounter(id) {
    try {
        await fetch(`/api/counter/${id}/reset`, { method: 'POST' });
    } catch (error) {
        console.error('Failed to reset:', error);
    }
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

window.addEventListener('DOMContentLoaded', init);