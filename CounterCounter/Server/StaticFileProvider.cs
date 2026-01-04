// CounterCounter/Server/StaticFileProvider.cs
using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace CounterCounter.Server
{
    public class StaticFileProvider
    {
        private readonly string _wwwrootPath;

        public StaticFileProvider()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string exeDir = Path.GetDirectoryName(exePath) ?? Environment.CurrentDirectory;
            _wwwrootPath = Path.Combine(exeDir, "wwwroot");
        }

        public void ServeFile(HttpListenerContext context, string requestPath)
        {
            try
            {
                string filePath = Path.Combine(_wwwrootPath, requestPath.TrimStart('/'));

                if (!File.Exists(filePath))
                {
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                    return;
                }

                string contentType = GetContentType(filePath);
                byte[] fileBytes = File.ReadAllBytes(filePath);

                context.Response.ContentType = contentType;
                context.Response.ContentLength64 = fileBytes.Length;
                context.Response.OutputStream.Write(fileBytes, 0, fileBytes.Length);
                context.Response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error serving file: {ex.Message}");
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
        }

        private string GetContentType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".html" => "text/html; charset=utf-8",
                ".css" => "text/css; charset=utf-8",
                ".js" => "application/javascript; charset=utf-8",
                ".json" => "application/json; charset=utf-8",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".ico" => "image/x-icon",
                _ => "application/octet-stream"
            };
        }
    }
}