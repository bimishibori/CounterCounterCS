// CounterCounter/Server/HtmlContentProvider.cs
using System;

namespace CounterCounter.Server
{
    public class HtmlContentProvider
    {
        public string GetManagerHtml(int httpPort)
        {
            int wsPort = httpPort + 1;
            return $@"<!DOCTYPE html>
<html lang='ja'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Counter Counter - 管理画面</title>
    <link rel='stylesheet' href='/css/manager.css'>
</head>
<body data-ws-port='{wsPort}'>
    <div class='container'>
        <header>
            <h1>Counter Counter</h1>
            <div id='connection-status' class='status disconnected'>切断</div>
        </header>
        <div id='counters-container'></div>
    </div>
    <script src='/js/manager.js'></script>
</body>
</html>";
        }

        public string GetObsHtml(int httpPort)
        {
            int wsPort = httpPort + 1;
            return $@"<!DOCTYPE html>
<html lang='ja'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Counter Display</title>
    <link rel='stylesheet' href='/css/obs.css'>
</head>
<body data-ws-port='{wsPort}'>
    <div id='counter-display'></div>
    <script src='/js/obs.js'></script>
</body>
</html>";
        }
    }
}