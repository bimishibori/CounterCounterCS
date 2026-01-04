// CounterCounter/Server/WebServer.cs
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CounterCounter.Core;

namespace CounterCounter.Server
{
    public class WebServer : IDisposable
    {
        private HttpListener? _listener;
        private CancellationTokenSource? _cts;
        private readonly CounterState _counterState;
        private readonly ApiHandler _apiHandler;
        private readonly HtmlContentProvider _htmlProvider;
        private readonly StaticFileProvider _staticFileProvider;
        private int _port = 8765;
        private int _wsPort = 8766;

        public int Port => _port;
        public int WebSocketPort
        {
            get => _wsPort;
            set => _wsPort = value;
        }

        public bool IsRunning { get; private set; }

        public WebServer(CounterState counterState)
        {
            _counterState = counterState;
            _apiHandler = new ApiHandler(counterState);
            _htmlProvider = new HtmlContentProvider();
            _staticFileProvider = new StaticFileProvider();
        }

        public async Task StartAsync()
        {
            for (int port = 8765; port < 8775; port++)
            {
                try
                {
                    _port = port;
                    _wsPort = port + 1;
                    _listener = new HttpListener();
                    _listener.Prefixes.Add($"http://localhost:{_port}/");
                    _listener.Start();
                    IsRunning = true;
                    break;
                }
                catch (HttpListenerException)
                {
                    _listener?.Close();
                    _listener = null;
                    continue;
                }
            }

            if (_listener == null)
            {
                throw new Exception("利用可能なポートが見つかりませんでした");
            }

            _cts = new CancellationTokenSource();
            _ = Task.Run(async () => await ProcessRequestsAsync(_cts.Token));
        }

        private async Task ProcessRequestsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _listener != null)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequest(context), cancellationToken);
                }
                catch (Exception ex) when (ex is HttpListenerException || ex is ObjectDisposedException)
                {
                    break;
                }
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                SetCorsHeaders(response);

                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = 200;
                    response.Close();
                    return;
                }

                string path = request.Url?.AbsolutePath ?? "/";
                RouteRequest(path, request.HttpMethod, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling request: {ex.Message}");
                try
                {
                    context.Response.StatusCode = 500;
                    context.Response.Close();
                }
                catch { }
            }
        }

        private void SetCorsHeaders(HttpListenerResponse response)
        {
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
        }

        private void RouteRequest(string path, string method, HttpListenerResponse response)
        {
            if (path.StartsWith("/api/"))
            {
                HandleApiRequest(path, method, response);
                return;
            }

            if (path.StartsWith("/css/") || path.StartsWith("/js/"))
            {
                ServeStaticFile(response, path);
                return;
            }

            switch (path)
            {
                case "/":
                    ServeHtml(response, _htmlProvider.GetIndexHtml());
                    break;
                case "/obs.html":
                    ServeHtml(response, _htmlProvider.GetObsHtml());
                    break;
                default:
                    response.StatusCode = 404;
                    SendJson(response, new { error = "Not Found" });
                    break;
            }
        }

        private void HandleApiRequest(string path, string method, HttpListenerResponse response)
        {
            switch (path)
            {
                case "/api/counter" when method == "GET":
                    _apiHandler.HandleGetCounter(response);
                    break;
                case "/api/counter/increment" when method == "POST":
                    _apiHandler.HandleIncrement(response);
                    break;
                case "/api/counter/decrement" when method == "POST":
                    _apiHandler.HandleDecrement(response);
                    break;
                case "/api/counter/reset" when method == "POST":
                    _apiHandler.HandleReset(response);
                    break;
                default:
                    response.StatusCode = 404;
                    SendJson(response, new { error = "API Not Found" });
                    break;
            }
        }

        private void ServeStaticFile(HttpListenerResponse response, string path)
        {
            string content = _staticFileProvider.ReadFile(path);

            if (string.IsNullOrEmpty(content))
            {
                response.StatusCode = 404;
                SendJson(response, new { error = "File Not Found" });
                return;
            }

            string contentType = _staticFileProvider.GetContentType(path);
            response.ContentType = $"{contentType}; charset=utf-8";
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }

        private void ServeHtml(HttpListenerResponse response, string content)
        {
            response.ContentType = "text/html; charset=utf-8";
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }

        private void SendJson(HttpListenerResponse response, object data)
        {
            response.ContentType = "application/json";
            string json = System.Text.Json.JsonSerializer.Serialize(data);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }

        public void Stop()
        {
            IsRunning = false;
            _cts?.Cancel();
            _listener?.Stop();
            _listener?.Close();
        }

        public void Dispose()
        {
            Stop();
            _cts?.Dispose();
            _listener = null;
        }
    }
}