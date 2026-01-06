// CounterCounter/Server/WebSocketHandler.cs
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CounterCounter.Core;

namespace CounterCounter.Server
{
    public class WebSocketHandler : IDisposable
    {
        private readonly CounterManager _counterManager;
        private readonly ConcurrentBag<WebSocket> _sockets;

        public WebSocketHandler(CounterManager counterManager)
        {
            _counterManager = counterManager;
            _sockets = new ConcurrentBag<WebSocket>();
            _counterManager.CounterChanged += OnCounterChanged;
        }

        public async Task HandleConnectionAsync(WebSocket webSocket)
        {
            _sockets.Add(webSocket);
            Console.WriteLine("WebSocket client connected");

            var counters = _counterManager.GetAllCounters();
            var initMessage = JsonSerializer.Serialize(new
            {
                type = "init",
                counters
            });
            await SendMessageAsync(webSocket, initMessage);

            var buffer = new byte[1024 * 4];
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Closed by client",
                            CancellationToken.None);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("WebSocket client disconnected");
            }
        }

        private void OnCounterChanged(object? sender, CounterChangedEventArgs e)
        {
            var counter = _counterManager.GetCounter(e.CounterId);
            if (counter == null) return;

            var message = JsonSerializer.Serialize(new
            {
                type = "counter_update",
                counterId = e.CounterId,
                value = e.NewValue,
                oldValue = e.OldValue,
                changeType = e.ChangeType.ToLower(),
                counter
            });

            BroadcastMessage(message);
        }

        public void BroadcastNextRotation()
        {
            var message = JsonSerializer.Serialize(new
            {
                type = "next_rotation"
            });

            BroadcastMessage(message);
        }

        public void BroadcastForceDisplay(string counterId)
        {
            var counter = _counterManager.GetCounter(counterId);
            if (counter == null) return;

            var message = JsonSerializer.Serialize(new
            {
                type = "force_display",
                counterId,
                counter
            });

            BroadcastMessage(message);
        }

        private void BroadcastMessage(string message)
        {
            var socketsToRemove = new List<WebSocket>();

            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    _ = Task.Run(async () => await SendMessageAsync(socket, message));
                }
                else
                {
                    socketsToRemove.Add(socket);
                }
            }

            foreach (var socket in socketsToRemove)
            {
                socket.Dispose();
            }
        }

        private async Task SendMessageAsync(WebSocket socket, string message)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(
                    new ArraySegment<byte>(buffer),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send WebSocket message: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _counterManager.CounterChanged -= OnCounterChanged;

            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    _ = socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Server shutdown",
                        CancellationToken.None);
                }
                socket.Dispose();
            }
        }
    }
}