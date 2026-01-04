// CounterCounter/Server/StaticFileProvider.cs
using System;
using System.IO;
using System.Reflection;

namespace CounterCounter.Server
{
    public class StaticFileProvider
    {
        private readonly string _wwwrootPath;

        public StaticFileProvider()
        {
            string? exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _wwwrootPath = Path.Combine(exeDir ?? "", "wwwroot");
        }

        public string ReadFile(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(_wwwrootPath, relativePath.TrimStart('/'));

                if (!File.Exists(fullPath))
                {
                    return string.Empty;
                }

                return File.ReadAllText(fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ファイル読み込みエラー: {relativePath} - {ex.Message}");
                return string.Empty;
            }
        }

        public bool FileExists(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(_wwwrootPath, relativePath.TrimStart('/'));
                return File.Exists(fullPath);
            }
            catch
            {
                return false;
            }
        }

        public string GetContentType(string path)
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();

            return extension switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".json" => "application/json",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".ico" => "image/x-icon",
                _ => "application/octet-stream"
            };
        }
    }
}