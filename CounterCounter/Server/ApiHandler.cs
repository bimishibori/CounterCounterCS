// CounterCounter/Server/ApiHandler.cs
using System.Net;
using System.Text;
using System.Text.Json;
using CounterCounter.Core;

namespace CounterCounter.Server
{
    public class ApiHandler
    {
        private readonly CounterState _counterState;

        public ApiHandler(CounterState counterState)
        {
            _counterState = counterState;
        }

        public void HandleGetCounter(HttpListenerResponse response)
        {
            SendJson(response, new { value = _counterState.GetValue() });
        }

        public void HandleIncrement(HttpListenerResponse response)
        {
            _counterState.Increment();
            SendJson(response, new { value = _counterState.GetValue() });
        }

        public void HandleDecrement(HttpListenerResponse response)
        {
            _counterState.Decrement();
            SendJson(response, new { value = _counterState.GetValue() });
        }

        public void HandleReset(HttpListenerResponse response)
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
    }
}