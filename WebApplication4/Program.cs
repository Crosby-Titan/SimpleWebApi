using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace WebApplication4
{
    public class Program
    {
        private static IDictionary<string, LinkedList<Image>> _pictures { get; set; }
        private static readonly string _defaultImagePath = @"C:\Users\CrosbyTitan\source\repos\WebApplication4\WebApplication4\files\Resources\Images\";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { WebRootPath = "files" });
            var app = builder.Build();

            app.UseStaticFiles();

            InitializeComponents();

            app.MapGet("/", async (context) =>
            {
                context.Response.ContentType = "text/html; charset=utf-8;";

                await context.Response.SendFileAsync(@"C:\Users\CrosbyTitan\source\repos\WebApplication4\WebApplication4\files\HTML\index.html");

            });

            app.MapPost("/search", async (context) =>
            {
                context.Request.ContentType = "application/json";

                var query = await context.Request.ReadFromJsonAsync<QueryString>();

                if (String.IsNullOrEmpty(query.Query))
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsJsonAsync(new { message = new[] { "Current query returned empty result." } });
                    return;
                }

                await SendPictureAsync(context, query?.Query);

            });

            app.Run();
        }

        private static void InitializeComponents()
        {
            _pictures = new Dictionary<string, LinkedList<Image>>()
            {
                {"samurai",new LinkedList<Image>()}
            };

            var img = new Image(Path.Combine(_defaultImagePath, "samurai.jpg"), "samurai");
            var img2 = new Image(Path.Combine(_defaultImagePath, "samurai2.jpg"), "samurai");

            _pictures["samurai"].AddLast(img);
            _pictures["samurai"].AddLast(img2);
        }

        private static async Task SendPictureAsync(HttpContext context, string? query)
        {

            context.Response.ContentType = "application/json; charset=utf-8;";

            var imgCollection = new List<string?>();

            await Task.Run(() => 
            {
                foreach (var picCollection in GetImages(query, _pictures))
                {
                    if (picCollection == null)
                        continue;

                    foreach (var pic in picCollection)
                    {
                        imgCollection.Add(pic.GetEncodedUrl);
                    }
                }
            });

            imgCollection.Add("END");

            await context.Response.WriteAsJsonAsync(new { message = imgCollection.ToArray() });

        }

        private static IEnumerable<LinkedList<Image>?> GetImages(string? query, IDictionary<string, LinkedList<Image>> pictures)
        {
            foreach (var picture in pictures)
            {
                if (picture.Key == query.ToLower())
                {
                    yield return picture.Value;
                }
            }

            yield break;
        }
    }

    [Serializable]
    internal class QueryString
    {
        [JsonPropertyName("QueryString")]
        public string? Query { get; set; }
    }

    [Serializable]
    internal class Image
    {
        private readonly string _imageTags;
        private readonly string _imageUrl;
        private string? _encodedURL;

        [JsonPropertyName("ImageURL")]
        public string? GetEncodedUrl { get { if (_encodedURL == null) SetEncodedImageBytes(); return _encodedURL; } }
        public string GetTags { get { return _imageTags; } }
        public string GetUrl { get { return _imageUrl; } }

        public Image(string imgSrc, string tags)
        {
            _imageUrl = imgSrc;
            _imageTags = tags;
        }

        private void SetEncodedImageBytes()
        {
            _encodedURL = Convert.ToBase64String(File.ReadAllBytes(_imageUrl));
        }
    }
}