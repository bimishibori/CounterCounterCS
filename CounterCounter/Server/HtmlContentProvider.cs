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
<body>
    <div id=""counter-display""></div>
    <script>
        const WS_PORT = {wsPort};
    </script>
    <script src=""/js/obs.js""></script>
</body>
</html>";
        }
    }
}