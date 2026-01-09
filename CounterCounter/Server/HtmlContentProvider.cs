// CounterCounter/Server/HtmlContentProvider.cs
namespace CounterCounter.Server
{
    public class HtmlContentProvider
    {
        public string GenerateObsHtml(int wsPort, int intervalMs, string theme)
        {
            return $@"<!DOCTYPE html>
<html lang=""ja"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Counter Counter - OBS Display</title>
    <link rel=""stylesheet"" href=""/themes/{theme}/css/obs.css"">
</head>
<body data-ws-port=""{wsPort}"" data-slidein-interval=""{intervalMs}"">
    <div id=""counter-display""></div>
    <script src=""/themes/{theme}/js/obs.js""></script>
</body>
</html>";
        }

        public string GenerateRotationHtml(int wsPort, int intervalMs, string theme)
        {
            return $@"<!DOCTYPE html>
<html lang=""ja"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Counter Counter - Rotation Display</title>
    <link rel=""stylesheet"" href=""/themes/{theme}/css/rotation.css"">
</head>
<body data-ws-port=""{wsPort}"" data-rotation-interval=""{intervalMs}"">
    <div id=""counter-display""></div>
    <script src=""/themes/{theme}/js/rotation.js""></script>
</body>
</html>";
        }
    }
}