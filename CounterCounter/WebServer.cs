using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CounterCounter
{
    public class WebServer : IDisposable
    {
        private HttpListener? _listener;
        private CancellationTokenSource? _cts;
        private readonly CounterState _counterState;
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
        }

        public async Task StartAsync()
        {
            // ポートを探す（8765から順に試す）
            for (int port = 8765; port < 8775; port++)
            {
                try
                {
                    _port = port;
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

            // リクエスト処理ループ
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

                // CORS対応
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = 200;
                    response.Close();
                    return;
                }

                string path = request.Url?.AbsolutePath ?? "/";

                // ルーティング
                if (path == "/")
                {
                    ServeHtmlFile(response, "index.html", _wsPort);
                }
                else if (path == "/obs.html")
                {
                    ServeHtmlFile(response, "obs.html", _wsPort);
                }
                else if (path.StartsWith("/css/"))
                {
                    ServeStaticFile(response, path, "text/css");
                }
                else if (path.StartsWith("/js/"))
                {
                    ServeStaticFile(response, path, "application/javascript");
                }
                else if (path == "/api/counter" && request.HttpMethod == "GET")
                {
                    HandleGetCounter(response);
                }
                else if (path == "/api/counter/increment" && request.HttpMethod == "POST")
                {
                    HandleIncrement(response);
                }
                else if (path == "/api/counter/decrement" && request.HttpMethod == "POST")
                {
                    HandleDecrement(response);
                }
                else if (path == "/api/counter/reset" && request.HttpMethod == "POST")
                {
                    HandleReset(response);
                }
                else
                {
                    response.StatusCode = 404;
                    SendJson(response, new { error = "Not Found" });
                }
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

        private void HandleGetCounter(HttpListenerResponse response)
        {
            SendJson(response, new { value = _counterState.GetValue() });
        }

        private void HandleIncrement(HttpListenerResponse response)
        {
            _counterState.Increment();
            SendJson(response, new { value = _counterState.GetValue() });
        }

        private void HandleDecrement(HttpListenerResponse response)
        {
            _counterState.Decrement();
            SendJson(response, new { value = _counterState.GetValue() });
        }

        private void HandleReset(HttpListenerResponse response)
        {
            _counterState.Reset();
            SendJson(response, new { value = _counterState.GetValue() });
        }

        private void SendJson(HttpListenerResponse response, object data)
        {
            response.ContentType = "application/json";
            string json = JsonSerializer.Serialize(data);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }

        private void ServeHtmlFile(HttpListenerResponse response, string fileName, int wsPort)
        {
            string content = GetEmbeddedHtml(fileName, wsPort);
            response.ContentType = "text/html; charset=utf-8";
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }

        private void ServeStaticFile(HttpListenerResponse response, string path, string contentType)
        {
            string content = GetEmbeddedFile(path);
            response.ContentType = $"{contentType}; charset=utf-8";
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }

        private string GetEmbeddedHtml(string fileName, int wsPort)
        {
            // 仮実装：簡単なHTMLを返す（後で外部ファイル化）
            if (fileName == "index.html")
            {
                return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>カウンター・カウンター 管理画面</title>
    <style>
        body {{ 
            background: #1a1a1a; 
            color: #fff; 
            font-family: Arial, sans-serif; 
            text-align: center; 
            padding: 50px;
        }}
        .counter {{ font-size: 72px; margin: 30px; }}
        button {{ 
            font-size: 24px; 
            padding: 15px 30px; 
            margin: 10px; 
            cursor: pointer;
            background: #4CAF50;
            color: white;
            border: none;
            border-radius: 5px;
        }}
        button:hover {{ background: #45a049; }}
        .reset {{ background: #f44336; }}
        .reset:hover {{ background: #da190b; }}
        .status {{ 
            margin: 20px; 
            padding: 10px; 
            background: #333; 
            border-radius: 5px; 
            font-size: 14px;
        }}
        .connected {{ color: #4CAF50; }}
        .disconnected {{ color: #f44336; }}
    </style>
</head>
<body>
    <h1>カウンター・カウンター</h1>
    <div class='status' id='status'>接続中...</div>
    <div class='counter' id='counter'>0</div>
    <button onclick='increment()'>＋</button>
    <button onclick='decrement()'>－</button>
    <button class='reset' onclick='reset()'>リセット</button>
    
    <script>
        let ws;
        const statusEl = document.getElementById('status');
        const counterEl = document.getElementById('counter');
        
        function connectWebSocket() {{
            ws = new WebSocket('ws://localhost:{wsPort}/ws');
            
            ws.onopen = () => {{
                statusEl.textContent = '✓ 接続中';
                statusEl.className = 'status connected';
            }};
            
            ws.onmessage = (event) => {{
                const data = JSON.parse(event.data);
                if (data.type === 'counter_update') {{
                    counterEl.textContent = data.value;
                }}
            }};
            
            ws.onclose = () => {{
                statusEl.textContent = '✗ 切断されました';
                statusEl.className = 'status disconnected';
                setTimeout(connectWebSocket, 3000);
            }};
            
            ws.onerror = (error) => {{
                console.error('WebSocketエラー:', error);
            }};
        }}
        
        async function increment() {{
            await fetch('/api/counter/increment', {{ method: 'POST' }});
        }}
        
        async function decrement() {{
            await fetch('/api/counter/decrement', {{ method: 'POST' }});
        }}
        
        async function reset() {{
            await fetch('/api/counter/reset', {{ method: 'POST' }});
        }}
        
        connectWebSocket();
    </script>
</body>
</html>";
            }
            else if (fileName == "obs.html")
            {
                return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>カウンター表示</title>
    <style>
        body {{ 
            margin: 0;
            background: transparent;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }}
        .counter {{ 
            font-size: 120px; 
            font-weight: bold;
            color: #fff;
            text-shadow: 3px 3px 6px rgba(0,0,0,0.8);
            transition: transform 0.3s ease;
        }}
        .counter.flash {{
            transform: scale(1.2);
        }}
    </style>
</head>
<body>
    <div class='counter' id='counter'>0</div>
    
    <script>
        let ws;
        const counterEl = document.getElementById('counter');
        
        function connectWebSocket() {{
            ws = new WebSocket('ws://localhost:{wsPort}/ws');
            
            ws.onopen = () => {{
                console.log('WebSocket接続成功');
            }};
            
            ws.onmessage = (event) => {{
                const data = JSON.parse(event.data);
                if (data.type === 'counter_update') {{
                    counterEl.textContent = data.value;
                    
                    // フラッシュエフェクト
                    counterEl.classList.add('flash');
                    setTimeout(() => counterEl.classList.remove('flash'), 300);
                }}
            }};
            
            ws.onclose = () => {{
                console.log('WebSocket切断、再接続します...');
                setTimeout(connectWebSocket, 3000);
            }};
            
            ws.onerror = (error) => {{
                console.error('WebSocketエラー:', error);
            }};
        }}
        
        connectWebSocket();
    </script>
</body>
</html>";
            }
            return "Not Found";
        }

        private string GetEmbeddedFile(string path)
        {
            // 仮実装：空文字列を返す（後で実装）
            return "";
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