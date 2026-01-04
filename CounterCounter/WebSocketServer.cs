using System;
using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CounterCounter
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
            _wsPort = httpPort + 1; // HTTPポート + 1 (例: 8765 → 8766)
        }

        public void Start()
        {
            _server = new WebSocketSharp.Server.WebSocketServer(_wsPort);
            _server.AddWebSocketService<CounterWebSocketService>("/ws", () =>
                new CounterWebSocketService(_counterState));
            _server.Start();

            // カウンター変更時に全クライアントに通知
            _counterState.ValueChanged += OnCounterChanged;
        }

        private void OnCounterChanged(object? sender, CounterChangedEventArgs e)
        {
            // 全接続中のクライアントにブロードキャスト
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

    // WebSocketサービスクラス
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

            // 接続時に現在値を送信
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