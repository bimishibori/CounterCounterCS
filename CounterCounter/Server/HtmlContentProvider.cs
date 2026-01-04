// CounterCounter/Server/HtmlContentProvider.cs
using System;

namespace CounterCounter.Server
{
    public class HtmlContentProvider
    {
        private readonly StaticFileProvider _fileProvider;

        public HtmlContentProvider()
        {
            _fileProvider = new StaticFileProvider();
        }

        public string GetIndexHtml()
        {
            string content = _fileProvider.ReadFile("index.html");

            if (string.IsNullOrEmpty(content))
            {
                return GetFallbackIndexHtml();
            }

            return content;
        }

        public string GetObsHtml()
        {
            string content = _fileProvider.ReadFile("obs.html");

            if (string.IsNullOrEmpty(content))
            {
                return GetFallbackObsHtml();
            }

            return content;
        }

        private string GetFallbackIndexHtml()
        {
            return @"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>カウンター・カウンター</title>
    <style>body { background: #1a1a1a; color: #fff; text-align: center; padding: 50px; }</style>
</head>
<body>
    <h1>カウンター・カウンター</h1>
    <p>wwwrootフォルダが見つかりません</p>
</body>
</html>";
        }

        private string GetFallbackObsHtml()
        {
            return @"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>カウンター表示</title>
    <style>body { background: transparent; display: flex; justify-content: center; align-items: center; height: 100vh; } .counter { font-size: 120px; color: #fff; }</style>
</head>
<body>
    <div class='counter'>0</div>
</body>
</html>";
        }
    }
}