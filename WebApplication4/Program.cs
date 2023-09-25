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
                { "samurai", @"b79f9664257bf2836f7e7ca896f92244.jpg" }
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
                    var a = new BinaryWriter(context.Response.Body);

                    a.Write("Invalid request");

                    a.Close();
                }

                var path = @$"C:\Users\CrosbyTitan\source\repos\WebApplication4\WebApplication4\Resources\"
                        + @$"{_pictures[query.Query ?? "null"]}";

                using var binaryReader = new BinaryReader(
                    File.Open(path, FileMode.Open));

                await binaryReader.BaseStream.CopyToAsync(context.Response.Body);
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
}