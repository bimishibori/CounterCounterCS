// CounterCounter/Server/WebSocketServer.cs
using System;
using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using CounterCounter.Core;

namespace CounterCounter.Server
{
    public class CounterWebSocketService : WebSocketBehavior
    {
        private readonly CounterManager _counterManager;

        public CounterWebSocketService(CounterManager counterManager)
        {
            _counterManager = counterManager;
        }

        protected override void OnOpen()
        {
            var counters = _counterManager.GetAllCounters();
            var message = JsonSerializer.Serialize(new
            {
                type = "init",
                counters
            });
            Send(message);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine($"Received: {e.Data}");
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("WebSocket connection closed.");
        }
    }

    public class WebSocketServer : IDisposable
    {
        private readonly WebSocketSharp.Server.WebSocketServer _server;
        private readonly CounterManager _counterManager;

        public int Port { get; }

        public WebSocketServer(CounterManager counterManager, int httpPort)
        {
            _counterManager = counterManager;
            Port = httpPort + 1;

            _server = new WebSocketSharp.Server.WebSocketServer($"ws://localhost:{Port}");

#pragma warning disable CS0618
            _server.AddWebSocketService<CounterWebSocketService>("/ws", () => new CounterWebSocketService(_counterManager));
#pragma warning restore CS0618

            _counterManager.CounterChanged += OnCounterChanged;
        }

        public void Start()
        {
            _server.Start();
            Console.WriteLine($"WebSocket Server started on port {Port}");
        }

        private void OnCounterChanged(object? sender, CounterChangeEventArgs e)
        {
            var counter = _counterManager.GetCounter(e.CounterId);
            if (counter == null) return;

            var message = JsonSerializer.Serialize(new
            {
                type = "counter_update",
                counterId = e.CounterId,
                value = e.NewValue,
                oldValue = e.OldValue,
                changeType = e.ChangeType.ToString().ToLower(),
                counter
            });

            _server.WebSocketServices["/ws"].Sessions.Broadcast(message);
        }

        public void Dispose()
        {
            _counterManager.CounterChanged -= OnCounterChanged;
            _server?.Stop();
        }
    }
}