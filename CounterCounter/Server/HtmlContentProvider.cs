// CounterCounter/Server/HtmlContentProvider.cs
namespace CounterCounter.Server
{
    public class HtmlContentProvider
    {
        public string GenerateObsHtml(int wsPort)
        {
            return $@"<!DOCTYPE html>
<html lang=""ja"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Counter Counter - OBS Display</title>
    <link rel=""stylesheet"" href=""/css/obs.css"">
</head>
<body data-ws-port=""{wsPort}"">
    <div id=""counter-display""></div>
    <script src=""/js/obs.js""></script>
</body>
</html>";
        }

        public string GenerateManagerHtml(int wsPort)
        {
            return $@"<!DOCTYPE html>
<html lang=""ja"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>カウンター・カウンター 管理画面</title>
    <link rel=""stylesheet"" href=""/css/manager.css"">
</head>
<body data-ws-port=""{wsPort}"">
    <div class=""container"">
        <header>
            <h1>カウンター・カウンター</h1>
            <div class=""status connected"" id=""connection-status"">接続中</div>
        </header>
        <div id=""counters-container""></div>
    </div>
    <script src=""/js/manager.js""></script>
</body>
</html>";
        }
    }
}