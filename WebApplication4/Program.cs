using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace WebApplication4
{
    public class Program
    {
        private static IDictionary<string, LinkedList<Image>>? _pictures { get; set; }
        private static ImageWorker _worker { get; set; }
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

                if (String.IsNullOrEmpty(query?.Query))
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsJsonAsync(new { message = new[] { "Current query returned empty result." } });
                    return;
                }

                await _worker.SendPictureAsync(context, query?.Query,_pictures);

            });

            app.Run();
        }

        private static void InitializeComponents()
        {

            _pictures = new Dictionary<string, LinkedList<Image>>();

            _worker = new ImageWorker();

            var images = _worker.GetImagesAsync(_defaultImagePath).Result;

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

    [Serializable]
    internal class Image
    {
        private readonly string _imageUrl;
        private string? _encodedURL;

        [JsonPropertyName("ImageURL")]
        public string? GetEncodedUrl { get { if (_encodedURL == null) SetEncodedImageBytes(); return _encodedURL; } }
        public StringBuilder Tags { get; set; }
        public string GetUrl { get { return _imageUrl; } }

        public Image(string imgSrc)
        {
            _imageUrl = imgSrc;
            Tags = new StringBuilder();
        }

        private void SetEncodedImageBytes()
        {
            _encodedURL = Convert.ToBase64String(File.ReadAllBytes(_imageUrl));
        }
    }

    internal static class ReadImagesFromDirectory
    {
        public static async Task<LinkedList<Image>?> GetImagesAsync(this ImageWorker worker, string directory)
        {
            if (!Directory.Exists(directory))
                return null;

            var images = new LinkedList<Image>();

            await Task.Run(() =>
            {
                var files = Directory.GetFiles(directory).Where(
                (file) =>
                {
                    if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".svg"))
                        return true;

                    return false;
                });


                Parallel.ForEach(files, (file) =>
                {
                    var img = worker.CreateImage(file);

                    img.Tags.AppendLine(worker.GetShortFileName(file));

                    images.AddLast(img);
                });

            });

            return images;
        }
    }

    internal static class FileExtension
    {
        public static string GetShortFileName(this ImageWorker worker, string path)
        {
            string str = path.Substring(path.LastIndexOf('\\') + 1);

            return str.Remove(str.IndexOf('.'));
        }
    }

    internal class ImageWorker
    {
        public Image? CreateImage(string path)
        {
            if (!Directory.Exists(path))
                return null;

            return new Image(path);
        }

        public IEnumerable<LinkedList<Image>?> GetImages(string? query, IDictionary<string, LinkedList<Image>>? pictures)
        {
            if (pictures == null)
                yield break;

            foreach (var picture in pictures)
            {
                if (picture.Key.Trim() == query?.ToLower())
                {
                    yield return picture.Value;
                }
            }

            yield break;
        }

        public async Task SendPictureAsync(HttpContext context, string? query, IDictionary<string, LinkedList<Image>>? pictures)
        {

            context.Response.ContentType = "application/json; charset=utf-8;";

            var imgCollection = new List<string?>();

            await Task.Run(() =>
            {
                foreach (var picCollection in this.GetImages(query, pictures))
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
    }
}