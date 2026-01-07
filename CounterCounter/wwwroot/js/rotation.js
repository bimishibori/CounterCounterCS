// CounterCounter/wwwroot/js/rotation.js
let ws = null;
let counters = [];
let currentIndex = 0;
let rotationInterval = null;
let intervalDuration = 5000;

function init() {
    console.log('Rotation Display: Initializing...');

    intervalDuration = parseInt(document.body.dataset.rotationInterval) || 5000;

    connectWebSocket();
}

function connectWebSocket() {
    const wsPort = parseInt(document.body.dataset.wsPort);
    console.log('Rotation Display: Connecting to WebSocket on port', wsPort);

    ws = new WebSocket(`ws://localhost:${wsPort}/ws`);

    ws.onopen = () => {
        console.log('Rotation Display: WebSocket connected');
    };

    ws.onmessage = (event) => {
        console.log('Rotation Display: Received message', event.data);
        const data = JSON.parse(event.data);

        if (data.type === 'init') {
            console.log('Rotation Display: Init message received with counters:', data.counters);
            counters = data.counters.filter(c => c.ShowInRotation);
            renderCounters();
            startRotation();
        } else if (data.type === 'counter_update') {
            console.log('Rotation Display: Counter update received', data);
            updateCounter(data);
        } else if (data.type === 'next_rotation') {
            console.log('Rotation Display: Next rotation triggered');
            rotateToNext();
        } else if (data.type === 'force_display') {
            console.log('Rotation Display: Force display requested for', data.counterId);
            forceDisplay(data.counterId);
        }
    };

    ws.onclose = () => {
        console.log('Rotation Display: WebSocket disconnected, reconnecting in 5 seconds...');
        stopRotation();
        setTimeout(connectWebSocket, 5000);
    };

    ws.onerror = (error) => {
        console.error('Rotation Display: WebSocket error:', error);
    };
}

function renderCounters() {
    console.log('Rotation Display: Rendering counters', counters);
    const container = document.getElementById('counter-display');
    if (!container) {
        console.error('Rotation Display: counter-display element not found!');
        return;
    }

    container.innerHTML = '';

    if (counters.length === 0) {
        console.warn('Rotation Display: No counters to display');
        container.innerHTML = '<div style="color: white; text-align: center; padding: 20px;">カウンターがありません</div>';
        return;
    }

    counters.forEach((counter, index) => {
        console.log('Rotation Display: Creating counter element for', counter.Name);
        const div = document.createElement('div');
        div.className = 'counter-item';
        div.id = `counter-${counter.Id}`;
        div.innerHTML = `
            <div class="counter-display-line">
                <div class="counter-name" style="color: ${counter.Color}">${escapeHtml(counter.Name)}</div>
                <div class="counter-value" style="color: ${counter.Color}">${counter.Value}</div>
            </div>
        `;
        container.appendChild(div);
    });

    showCounter(currentIndex);
    console.log('Rotation Display: Rendering complete');
}

function showCounter(index) {
    const container = document.getElementById('counter-display');
    if (!container) return;

    const items = container.querySelectorAll('.counter-item');
    items.forEach((item, i) => {
        if (i === index) {
            item.classList.add('active');
        } else {
            item.classList.remove('active');
        }
    });
}

function startRotation() {
    if (counters.length <= 1) {
        console.log('Rotation Display: Only one or no counters, rotation disabled');
        return;
    }

    stopRotation();

    rotationInterval = setInterval(() => {
        rotateToNext();
    }, intervalDuration);

    console.log('Rotation Display: Rotation started with interval', intervalDuration, 'ms');
}

function rotateToNext() {
    currentIndex = (currentIndex + 1) % counters.length;
    showCounter(currentIndex);
    console.log('Rotation Display: Switched to counter index', currentIndex);
}

function stopRotation() {
    if (rotationInterval) {
        clearInterval(rotationInterval);
        rotationInterval = null;
        console.log('Rotation Display: Rotation stopped');
    }
}

function updateCounter(data) {
    console.log('Rotation Display: Updating counter', data.counterId, 'to value', data.value);

    if (data.counter.ShowInRotation) {
        const index = counters.findIndex(c => c.Id === data.counterId);
        if (index === -1) {
            counters.push(data.counter);
            renderCounters();
            startRotation();
        } else {
            const oldValue = counters[index].Value;
            counters[index] = data.counter;

            const el = document.getElementById(`counter-${data.counterId}`);
            if (el) {
                const valueEl = el.querySelector('.counter-value');
                if (valueEl) {
                    valueEl.textContent = data.counter.Value;

                    valueEl.classList.remove('flash', 'slide-up', 'slide-down');

                    if (data.counter.Value > oldValue) {
                        valueEl.classList.add('slide-up');
                    } else if (data.counter.Value < oldValue) {
                        valueEl.classList.add('slide-down');
                    }

                    valueEl.classList.add('flash');

                    setTimeout(() => {
                        valueEl.classList.remove('flash', 'slide-up', 'slide-down');
                    }, 300);
                }
            }
        }
    } else {
        const index = counters.findIndex(c => c.Id === data.counterId);
        if (index !== -1) {
            counters.splice(index, 1);
            renderCounters();
            startRotation();
        }
    }
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

window.addEventListener('DOMContentLoaded', () => {
    console.log('Rotation Display: DOM loaded');
    init();
});