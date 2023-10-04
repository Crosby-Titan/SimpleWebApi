using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using WebApplication4.Extensions;

namespace WebApplication4
{
    public class Program
    {
        internal static IDictionary<string, LinkedList<Image>>? _pictures { get; set; }
        internal static ImageWorker _worker { get; set; }
        internal static ContentBuilder _contentBuilder { get; set; }
        internal static ProjectPath projectPath { get; set; }
        internal static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { WebRootPath = "files" });
            var app = builder.Build();

            app.UseStaticFiles();

            InitializeComponents();

            app.MapGet("/", async (context) =>
            {
                context.Response.ContentType = _contentBuilder
                .AddContentType(ContentType.TextHtml)
                .AddCharset(Charset.Utf8)
                .Build();

                _contentBuilder.Clear();

                await context.Response.SendFileAsync($"{projectPath.HtmlPath}\\index.html");
            });

            app.MapPost("/search", async (context) =>
            {
                context.Request.ContentType = _contentBuilder
                .AddContentType(ContentType.ApplicationJson).Build();

                _contentBuilder.Clear();

                var query = await context.Request.ReadFromJsonAsync<QueryString>();

                if (String.IsNullOrEmpty(query?.Query))
                {
                    await context.Response.WriteAsJsonAsync(new { message = new[] { "По вашему запросу ничего не найдено." } });
                    return;
                }

                await _worker.SendPictureAsync(context, query?.Query.ToLower(), _pictures);

            });

            app.Run();
        }

        private static void InitializeComponents()
        {
            _contentBuilder = new ContentBuilder();

            projectPath = new ProjectPath();
            projectPath.InitializePaths();

            _pictures = new Dictionary<string, LinkedList<Image>>();

            _worker = new ImageWorker();

            var images = _worker.GetImagesAsync(projectPath.ImagesPath).Result;

            if (images == null)
                throw new NullReferenceException(nameof(images));

            foreach (var image in images)
            {
                string tags = image.Tags.ToString();

                if (_pictures.ContainsKey(tags))
                {
                    _pictures[tags].AddLast(image);
                }
                else
                {
                    _pictures.Add(tags, new LinkedList<Image>());
                    _pictures[tags].AddLast(image);
                }
            }
        }
    }

    [Serializable]
    internal class QueryString
    {
        [JsonPropertyName("QueryString")]
        public string? Query { get; set; }
    }
}