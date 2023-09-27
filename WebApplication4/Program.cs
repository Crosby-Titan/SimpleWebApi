using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace WebApplication4
{
    public class Program
    {
        private static IDictionary<string, string> _pictures { get; set; }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { WebRootPath = "files" });
            var app = builder.Build();

            app.UseStaticFiles();

            _pictures = new Dictionary<string, string>
            {
                { "samurai", "samurai.jpg" }
            };

            app.MapGet("/",async (context) =>
            {
                context.Response.ContentType = "text/html; charset=utf-8;";

                await context.Response.SendFileAsync(@"C:\Users\CrosbyTitan\source\repos\WebApplication4\WebApplication4\files\HTML\index.html");
                
            });

            app.MapPost("/search", async (context) =>
            {
                context.Request.ContentType = "application/json";

                var query = await context.Request.ReadFromJsonAsync<QueryString>();

                if (!_pictures.ContainsKey(query.Query))
                {
                    context.Response.StatusCode = 404;
                    return;
                }
                   
                var path = @$"C:\Users\CrosbyTitan\source\repos\WebApplication4\WebApplication4\files\Resources\Images\"
                        + @$"{_pictures[query.Query ?? "null"]}";

                FileInfo info = new FileInfo(path);

                Stream? stream = null;

                if (info.Exists)
                    stream = info.OpenRead();
                else
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                byte[] bytes = new byte[info.Length];

                stream.Read(bytes,0,bytes.Length);

                stream.Close();

                context.Response.ContentType = "application/json; charset=utf-8;";

                await context.Response.WriteAsJsonAsync(new { imgStr = Convert.ToBase64String(bytes) });
                
            });

            app.Run();
        }
    }

    [Serializable]
    internal class QueryString
    {
        [JsonPropertyName("QueryString")]
        public string? Query { get; set; }
    }

    [Serializable]
    internal class EncodedImages
    {
        [JsonPropertyName("ImagesArray")]
        public List<string>? Images { get; set; }
    }
}