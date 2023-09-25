namespace WebApplication4
{
    public class Program
    {
        private static IDictionary<string, string> _pictures { get; set; }  
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            _pictures = new Dictionary<string, string>
            {
                { "samurai", @"b79f9664257bf2836f7e7ca896f92244.jpg" }
            };

            app.MapGet("/", async (context) => 
            {
                context.Response.ContentType = "text/html; charset=utf-8;";

                await context.Response.SendFileAsync(@"../HTML/index.html");
            
            });

            app.MapGet("/search",async (context) =>
            {
                context.Request.ContentType = "application/json";

                var query = await context.Request.ReadFromJsonAsync<QueryString>();

                if (!_pictures.ContainsKey(query.Query))
                {
                    var a = new BinaryWriter(context.Response.Body);

                    a.Write("Invalid request");

                    a.Close();
                }

                using var binaryReader = new BinaryReader(
                    File.Open(
                        @$"C:\Users\CrosbyTitan\source\repos\WebApplication4\WebApplication4\Resources\
                        {_pictures[query.Query ?? "null"]}",FileMode.Open));

                await binaryReader.BaseStream.CopyToAsync(context.Response.Body);
            });

            app.Run();
        }
    }

    [Serializable]
    internal class QueryString
    {
        public string? Query { get; set; }
    }
}