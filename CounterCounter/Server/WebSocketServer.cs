// CounterCounter/Server/WebSocketServer.cs
using System;
using System.Text.Json;
using CounterCounter.Core;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CounterCounter.Server
{
    public class WebSocketServer : IDisposable
    {
        private WebSocketSharp.Server.WebSocketServer? _server;
        private readonly CounterState _counterState;
        private int _wsPort;

        public int Port => _wsPort;

        public WebSocketServer(CounterState counterState, int httpPort)
        {
            _counterState = counterState;
            _wsPort = httpPort + 1;
        }

        public void Start()
        {
            _server = new WebSocketSharp.Server.WebSocketServer(_wsPort);
            _server.AddWebSocketService("/ws", () =>
                new CounterWebSocketService(_counterState));
            _server.Start();

            _counterState.ValueChanged += OnCounterChanged;
        }

        private void OnCounterChanged(object? sender, CounterChangedEventArgs e)
        {
            _server?.WebSocketServices["/ws"]?.Sessions?.Broadcast(
                JsonSerializer.Serialize(new
                {
                    type = "counter_update",
                    value = e.NewValue,
                    changeType = e.ChangeType.ToString().ToLower()
                })
            );
        }

        public void Stop()
        {
            _counterState.ValueChanged -= OnCounterChanged;
            _server?.Stop();
        }

        public void Dispose()
        {
            Stop();
            _server = null;
        }
    }

    public class CounterWebSocketService : WebSocketBehavior
    {
        private readonly CounterState _counterState;

        public CounterWebSocketService(CounterState counterState)
        {
            _counterState = counterState;
        }

        protected override void OnOpen()
        {
            Console.WriteLine("WebSocket接続: " + ID);

            Send(JsonSerializer.Serialize(new
            {
                type = "counter_update",
                value = _counterState.GetValue(),
                changeType = "init"
            }));
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("WebSocket切断: " + ID);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("WebSocketメッセージ受信: " + e.Data);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine("WebSocketエラー: " + e.Message);
        }
    }
}