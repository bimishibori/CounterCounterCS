// CounterCounter/Server/WebServer.cs
using System.Net;
using System.Net.WebSockets;
using System.Text;
using CounterCounter.Core;

namespace CounterCounter.Server
{
    public class WebServer : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly CounterManager _counterManager;
        private readonly ApiHandler _apiHandler;
        private readonly HtmlContentProvider _htmlProvider;
        private readonly StaticFileProvider _staticFileProvider;
        private readonly WebSocketHandler _wsHandler;
        private readonly int _rotationIntervalMs;
        private readonly int _slideInIntervalMs;
        private bool _isRunning;

        public int Port { get; private set; }

        public WebServer(CounterManager counterManager, int rotationIntervalMs = 5000, int slideInIntervalMs = 5000)
        {
            _listener = new HttpListener();
            _counterManager = counterManager;
            _apiHandler = new ApiHandler(counterManager);
            _htmlProvider = new HtmlContentProvider();
            _staticFileProvider = new StaticFileProvider();
            _wsHandler = new WebSocketHandler(counterManager);
            _rotationIntervalMs = rotationIntervalMs;
            _slideInIntervalMs = slideInIntervalMs;
        }

        public async Task StartAsync(int startPort = 9000)
        {
            int port = startPort;
            int maxAttempts = 10;

            for (int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    _listener.Prefixes.Clear();
                    _listener.Prefixes.Add($"http://localhost:{port}/");
                    _listener.Start();
                    Port = port;
                    _isRunning = true;
                    Console.WriteLine($"HTTP Server started on port {port}");
                    _ = Task.Run(ProcessRequestsAsync);
                    return;
                }
                catch (HttpListenerException)
                {
                    port++;
                }
            }

            throw new Exception($"Failed to start HTTP server after {maxAttempts} attempts");
        }

        private async Task ProcessRequestsAsync()
        {
            while (_isRunning)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequestAsync(context));
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing request: {ex.Message}");
                }
            }
        }

        private async Task HandleRequestAsync(HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url?.AbsolutePath ?? "/";
                string method = context.Request.HttpMethod;

                context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

                if (method == "OPTIONS")
                {
                    context.Response.StatusCode = 204;
                    context.Response.Close();
                    return;
                }

                if (path == "/ws" && context.Request.IsWebSocketRequest)
                {
                    await HandleWebSocketAsync(context);
                    return;
                }

                if (path.StartsWith("/api/"))
                {
                    await _apiHandler.HandleApiRequestAsync(context, path, method);
                    return;
                }

                if (path == "/obs.html")
                {
                    string html = _htmlProvider.GenerateObsHtml(Port, _slideInIntervalMs);
                    await SendHtmlResponseAsync(context, html);
                    return;
                }

                if (path == "/rotation.html")
                {
                    string html = _htmlProvider.GenerateRotationHtml(Port, _rotationIntervalMs);
                    await SendHtmlResponseAsync(context, html);
                    return;
                }

                if (path.StartsWith("/css/") || path.StartsWith("/js/"))
                {
                    _staticFileProvider.ServeFile(context, path);
                    return;
                }

                context.Response.StatusCode = 404;
                byte[] buffer = Encoding.UTF8.GetBytes("Not Found");
                await context.Response.OutputStream.WriteAsync(buffer);
                context.Response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling request: {ex.Message}");
                try
                {
                    context.Response.StatusCode = 500;
                    byte[] buffer = Encoding.UTF8.GetBytes($"Internal Server Error: {ex.Message}");
                    await context.Response.OutputStream.WriteAsync(buffer);
                    context.Response.Close();
                }
                catch
                {
                }
            }
        }

        private async Task HandleWebSocketAsync(HttpListenerContext context)
        {
            HttpListenerWebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
            await _wsHandler.HandleConnectionAsync(wsContext.WebSocket);
        }

        private async Task SendHtmlResponseAsync(HttpListenerContext context, string html)
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            byte[] buffer = Encoding.UTF8.GetBytes(html);
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer);
            context.Response.Close();
        }

        public void BroadcastNextRotation()
        {
            _wsHandler.BroadcastNextRotation();
        }

        public void BroadcastForceDisplay(string counterId)
        {
            _wsHandler.BroadcastForceDisplay(counterId);
        }

        public void Dispose()
        {
            _isRunning = false;
            _wsHandler?.Dispose();
            _listener?.Stop();
            _listener?.Close();
        }
    }
}