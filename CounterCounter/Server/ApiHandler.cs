// CounterCounter/Server/ApiHandler.cs
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
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

        public void HandleRequest(HttpListenerContext context)
        {
            string path = context.Request.Url?.AbsolutePath ?? "/";
            string method = context.Request.HttpMethod;

            try
            {
                if (method == "GET" && path == "/api/counters")
                {
                    HandleGetAllCounters(context);
                }
                else if (method == "GET" && path.StartsWith("/api/counter/"))
                {
                    string counterId = path.Substring("/api/counter/".Length);
                    HandleGetCounter(context, counterId);
                }
                else if (method == "POST" && path == "/api/counters")
                {
                    HandleCreateCounter(context);
                }
                else if (method == "PUT" && path.StartsWith("/api/counter/"))
                {
                    string counterId = path.Substring("/api/counter/".Length);
                    HandleUpdateCounter(context, counterId);
                }
                else if (method == "DELETE" && path.StartsWith("/api/counter/"))
                {
                    string counterId = path.Substring("/api/counter/".Length);
                    HandleDeleteCounter(context, counterId);
                }
                else if (method == "POST" && path.EndsWith("/increment"))
                {
                    string counterId = ExtractCounterId(path, "/increment");
                    HandleIncrement(context, counterId);
                }
                else if (method == "POST" && path.EndsWith("/decrement"))
                {
                    string counterId = ExtractCounterId(path, "/decrement");
                    HandleDecrement(context, counterId);
                }
                else if (method == "POST" && path.EndsWith("/reset"))
                {
                    string counterId = ExtractCounterId(path, "/reset");
                    HandleReset(context, counterId);
                }
                else
                {
                    SendJsonResponse(context, 404, new { error = "Not Found" });
                }
            }
            catch (Exception ex)
            {
                SendJsonResponse(context, 500, new { error = ex.Message });
            }
        }

        private string ExtractCounterId(string path, string suffix)
        {
            int start = "/api/counter/".Length;
            int end = path.Length - suffix.Length;
            return path.Substring(start, end - start);
        }

        private void HandleGetAllCounters(HttpListenerContext context)
        {
            var counters = _counterManager.GetAllCounters();
            SendJsonResponse(context, 200, new { counters });
        }

        private void HandleGetCounter(HttpListenerContext context, string counterId)
        {
            var counter = _counterManager.GetCounter(counterId);
            if (counter == null)
            {
                SendJsonResponse(context, 404, new { error = "Counter not found" });
                return;
            }
            SendJsonResponse(context, 200, counter);
        }

        private void HandleCreateCounter(HttpListenerContext context)
        {
            string json = ReadRequestBody(context);
            var data = JsonSerializer.Deserialize<Counter>(json);

            if (data == null)
            {
                SendJsonResponse(context, 400, new { error = "Invalid data" });
                return;
            }

            _counterManager.AddCounter(data);
            SendJsonResponse(context, 201, new { success = true, counter = data });
        }

        private void HandleUpdateCounter(HttpListenerContext context, string counterId)
        {
            string json = ReadRequestBody(context);
            var data = JsonSerializer.Deserialize<Counter>(json);

            if (data == null)
            {
                SendJsonResponse(context, 400, new { error = "Invalid data" });
                return;
            }

            _counterManager.UpdateCounter(counterId, data.Name, data.Color);
            SendJsonResponse(context, 200, new { success = true });
        }

        private void HandleDeleteCounter(HttpListenerContext context, string counterId)
        {
            _counterManager.RemoveCounter(counterId);
            SendJsonResponse(context, 200, new { success = true });
        }

        private void HandleIncrement(HttpListenerContext context, string counterId)
        {
            _counterManager.Increment(counterId);
            var counter = _counterManager.GetCounter(counterId);
            SendJsonResponse(context, 200, new { success = true, value = counter?.Value ?? 0 });
        }

        private void HandleDecrement(HttpListenerContext context, string counterId)
        {
            _counterManager.Decrement(counterId);
            var counter = _counterManager.GetCounter(counterId);
            SendJsonResponse(context, 200, new { success = true, value = counter?.Value ?? 0 });
        }

        private void HandleReset(HttpListenerContext context, string counterId)
        {
            _counterManager.Reset(counterId);
            var counter = _counterManager.GetCounter(counterId);
            SendJsonResponse(context, 200, new { success = true, value = counter?.Value ?? 0 });
        }

        private string ReadRequestBody(HttpListenerContext context)
        {
            using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
            return reader.ReadToEnd();
        }

        private void SendJsonResponse(HttpListenerContext context, int statusCode, object data)
        {
            string json = JsonSerializer.Serialize(data);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
    }
}