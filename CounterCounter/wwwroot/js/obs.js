// CounterCounter/wwwroot/js/obs.js
let ws = null;
let counters = [];

function init() {
    console.log('OBS Display: Initializing...');
    connectWebSocket();
}

function connectWebSocket() {
    const wsPort = parseInt(document.body.dataset.wsPort);
    console.log('OBS Display: Connecting to WebSocket on port', wsPort);

    ws = new WebSocket(`ws://localhost:${wsPort}/ws`);

    ws.onopen = () => {
        console.log('OBS Display: WebSocket connected');
    };

    ws.onmessage = (event) => {
        console.log('OBS Display: Received message', event.data);
        const data = JSON.parse(event.data);

        if (data.type === 'init') {
            console.log('OBS Display: Init message received with counters:', data.counters);
            counters = data.counters;
            renderCounters();
        } else if (data.type === 'counter_update') {
            console.log('OBS Display: Counter update received', data);
            updateCounter(data);
        }
    };

    ws.onclose = () => {
        console.log('OBS Display: WebSocket disconnected, reconnecting in 5 seconds...');
        setTimeout(connectWebSocket, 5000);
    };

    ws.onerror = (error) => {
        console.error('OBS Display: WebSocket error:', error);
    };
}

function renderCounters() {
    console.log('OBS Display: Rendering counters', counters);
    const container = document.getElementById('counter-display');
    if (!container) {
        console.error('OBS Display: counter-display element not found!');
        return;
    }

    container.innerHTML = '';

    if (counters.length === 0) {
        console.warn('OBS Display: No counters to display');
        container.innerHTML = '<div style="color: white; text-align: center; padding: 20px;">カウンターがありません</div>';
        return;
    }

    counters.forEach(counter => {
        console.log('OBS Display: Creating counter element for', counter.Name);
        const div = document.createElement('div');
        div.className = 'counter-item';
        div.id = `counter-${counter.Id}`;
        div.innerHTML = `
            <div class="counter-name" style="color: ${counter.Color}">${escapeHtml(counter.Name)}</div>
            <div class="counter-value" style="color: ${counter.Color}">${counter.Value}</div>
        `;
        container.appendChild(div);
    });

    console.log('OBS Display: Rendering complete');
}

function updateCounter(data) {
    console.log('OBS Display: Updating counter', data.counterId, 'to value', data.value);
    const index = counters.findIndex(c => c.Id === data.counterId);
    if (index !== -1) {
        counters[index] = data.counter;

        const el = document.getElementById(`counter-${data.counterId}`);
        if (el) {
            const valueEl = el.querySelector('.counter-value');
            if (valueEl) {
                valueEl.textContent = data.counter.Value;
                valueEl.classList.add('flash');
                setTimeout(() => valueEl.classList.remove('flash'), 300);
            }
        }
    }
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

window.addEventListener('DOMContentLoaded', () => {
    console.log('OBS Display: DOM loaded');
    init();
});