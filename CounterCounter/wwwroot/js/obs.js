// CounterCounter/wwwroot/js/obs.js
let ws = null;
let counters = [];

function init() {
    connectWebSocket();
}

function connectWebSocket() {
    const wsPort = parseInt(document.body.dataset.wsPort);
    ws = new WebSocket(`ws://localhost:${wsPort}/ws`);

    ws.onopen = () => {
        console.log('WebSocket connected');
    };

    ws.onmessage = (event) => {
        const data = JSON.parse(event.data);

        if (data.type === 'init') {
            counters = data.counters;
            renderCounters();
        } else if (data.type === 'counter_update') {
            updateCounter(data);
        }
    };

    ws.onclose = () => {
        console.log('WebSocket disconnected');
        setTimeout(connectWebSocket, 5000);
    };

    ws.onerror = (error) => {
        console.error('WebSocket error:', error);
    };
}

function renderCounters() {
    const container = document.getElementById('counter-display');
    if (!container) return;

    container.innerHTML = '';

    counters.forEach(counter => {
        const div = document.createElement('div');
        div.className = 'counter-item';
        div.id = `counter-${counter.Id}`;
        div.innerHTML = `
            <div class="counter-name" style="color: ${counter.Color}">${escapeHtml(counter.Name)}</div>
            <div class="counter-value" style="color: ${counter.Color}">${counter.Value}</div>
        `;
        container.appendChild(div);
    });
}

function updateCounter(data) {
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

window.addEventListener('DOMContentLoaded', init);