// CounterCounter/Server/WebServer.cs
using System;
using System.Net;
using System.Threading.Tasks;
using CounterCounter.Core;

namespace CounterCounter.Server
{
    public class WebServer : IDisposable
    {
        private HttpListener? _listener;
        private readonly CounterManager _counterManager;
        private readonly ApiHandler _apiHandler;
        private readonly HtmlContentProvider _htmlProvider;
        private readonly StaticFileProvider _staticFileProvider;

        public int Port { get; private set; }

        public WebServer(CounterManager counterManager)
        {
            _counterManager = counterManager;
            _apiHandler = new ApiHandler(_counterManager);
            _htmlProvider = new HtmlContentProvider();
            _staticFileProvider = new StaticFileProvider();
        }

        public async Task StartAsync(int preferredPort = 8765)
        {
            Port = preferredPort;

            for (int attempt = 0; attempt < 10; attempt++)
            {
                try
                {
                    _listener = new HttpListener();
                    _listener.Prefixes.Add($"http://localhost:{Port}/");
                    _listener.Start();
                    Console.WriteLine($"HTTP Server started on port {Port}");
                    _ = Task.Run(HandleRequestsAsync);
                    return;
                }
                catch (HttpListenerException)
                {
                    _listener?.Close();
                    _listener = null;
                    Port++;
                }
            }

            throw new InvalidOperationException("Could not start HTTP server on any port.");
        }

        private async Task HandleRequestsAsync()
        {
            while (_listener != null && _listener.IsListening)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Run(() => ProcessRequest(context));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling request: {ex.Message}");
                }
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                if (context.Request.HttpMethod == "OPTIONS")
                {
                    context.Response.StatusCode = 200;
                    context.Response.Close();
                    return;
                }

                string path = context.Request.Url?.AbsolutePath ?? "/";

                if (path.StartsWith("/api/"))
                {
                    _apiHandler.HandleRequest(context);
                }
                else if (path == "/" || path == "/index.html")
                {
                    ServeHtmlContent(context, _htmlProvider.GetManagerHtml(Port));
                }
                else if (path == "/obs.html")
                {
                    ServeHtmlContent(context, _htmlProvider.GetObsHtml(Port));
                }
                else if (path.StartsWith("/css/") || path.StartsWith("/js/"))
                {
                    _staticFileProvider.ServeFile(context, path);
                }
                else
                {
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
        }

        private void ServeHtmlContent(HttpListenerContext context, string html)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(html);
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }

        public void Dispose()
        {
            _listener?.Stop();
            _listener?.Close();
        }
    }
}