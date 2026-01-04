// CounterCounter/Server/ApiHandler.cs
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CounterCounter.Core;
using CounterCounter.Models;

namespace CounterCounter.Server
{
    public class ApiHandler
    {
        private readonly CounterManager _counterManager;

        public ApiHandler(CounterManager counterManager)
        {
            _counterManager = counterManager;
        }

        public async Task HandleApiRequestAsync(HttpListenerContext context, string path, string method)
        {
            try
            {
                if (method == "GET" && path == "/api/counters")
                {
                    await HandleGetAllCountersAsync(context);
                }
                else if (method == "GET" && path.StartsWith("/api/counter/"))
                {
                    string counterId = path.Substring("/api/counter/".Length);
                    await HandleGetCounterAsync(context, counterId);
                }
                else if (method == "POST" && path == "/api/counters")
                {
                    await HandleCreateCounterAsync(context);
                }
                else if (method == "PUT" && path.StartsWith("/api/counter/"))
                {
                    string counterId = path.Substring("/api/counter/".Length);
                    await HandleUpdateCounterAsync(context, counterId);
                }
                else if (method == "DELETE" && path.StartsWith("/api/counter/"))
                {
                    string counterId = path.Substring("/api/counter/".Length);
                    await HandleDeleteCounterAsync(context, counterId);
                }
                else if (method == "POST" && path.EndsWith("/increment"))
                {
                    string counterId = ExtractCounterId(path, "/increment");
                    await HandleIncrementAsync(context, counterId);
                }
                else if (method == "POST" && path.EndsWith("/decrement"))
                {
                    string counterId = ExtractCounterId(path, "/decrement");
                    await HandleDecrementAsync(context, counterId);
                }
                else if (method == "POST" && path.EndsWith("/reset"))
                {
                    string counterId = ExtractCounterId(path, "/reset");
                    await HandleResetAsync(context, counterId);
                }
                else
                {
                    await SendJsonResponseAsync(context, 404, new { error = "Not Found" });
                }
            }
            catch (Exception ex)
            {
                await SendJsonResponseAsync(context, 500, new { error = ex.Message });
            }
        }

        private string ExtractCounterId(string path, string suffix)
        {
            int start = "/api/counter/".Length;
            int end = path.Length - suffix.Length;
            return path.Substring(start, end - start);
        }

        private async Task HandleGetAllCountersAsync(HttpListenerContext context)
        {
            var counters = _counterManager.GetAllCounters();
            await SendJsonResponseAsync(context, 200, new { counters });
        }

        private async Task HandleGetCounterAsync(HttpListenerContext context, string counterId)
        {
            var counter = _counterManager.GetCounter(counterId);
            if (counter == null)
            {
                await SendJsonResponseAsync(context, 404, new { error = "Counter not found" });
                return;
            }
            await SendJsonResponseAsync(context, 200, counter);
        }

        private async Task HandleCreateCounterAsync(HttpListenerContext context)
        {
            string json = await ReadRequestBodyAsync(context);
            var data = JsonSerializer.Deserialize<Counter>(json);

            if (data == null)
            {
                await SendJsonResponseAsync(context, 400, new { error = "Invalid data" });
                return;
            }

            _counterManager.AddCounter(data);
            await SendJsonResponseAsync(context, 201, new { success = true, counter = data });
        }

        private async Task HandleUpdateCounterAsync(HttpListenerContext context, string counterId)
        {
            string json = await ReadRequestBodyAsync(context);
            var data = JsonSerializer.Deserialize<Counter>(json);

            if (data == null)
            {
                await SendJsonResponseAsync(context, 400, new { error = "Invalid data" });
                return;
            }

            _counterManager.UpdateCounter(counterId, data.Name, data.Color);
            await SendJsonResponseAsync(context, 200, new { success = true });
        }

        private async Task HandleDeleteCounterAsync(HttpListenerContext context, string counterId)
        {
            _counterManager.RemoveCounter(counterId);
            await SendJsonResponseAsync(context, 200, new { success = true });
        }

        private async Task HandleIncrementAsync(HttpListenerContext context, string counterId)
        {
            _counterManager.Increment(counterId);
            var counter = _counterManager.GetCounter(counterId);
            await SendJsonResponseAsync(context, 200, new { success = true, value = counter?.Value ?? 0 });
        }

        private async Task HandleDecrementAsync(HttpListenerContext context, string counterId)
        {
            _counterManager.Decrement(counterId);
            var counter = _counterManager.GetCounter(counterId);
            await SendJsonResponseAsync(context, 200, new { success = true, value = counter?.Value ?? 0 });
        }

        private async Task HandleResetAsync(HttpListenerContext context, string counterId)
        {
            _counterManager.Reset(counterId);
            var counter = _counterManager.GetCounter(counterId);
            await SendJsonResponseAsync(context, 200, new { success = true, value = counter?.Value ?? 0 });
        }

        private async Task<string> ReadRequestBodyAsync(HttpListenerContext context)
        {
            using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
            return await reader.ReadToEndAsync();
        }

        private async Task SendJsonResponseAsync(HttpListenerContext context, int statusCode, object data)
        {
            string json = JsonSerializer.Serialize(data);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.Close();
        }
    }
}