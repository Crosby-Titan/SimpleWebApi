using System.Text;
using WebApplication4.Extensions;

namespace WebApplication4
{
    internal class ImageWorker
    {
        public static Image CreateImage(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(nameof(path));

            return new Image(path);
        }

        public IEnumerable<LinkedList<Image>?> GetImages(string? query, IDictionary<string, LinkedList<Image>>? pictures)
        {
            if (pictures == null || query == null)
                yield break;

            foreach (var picture in pictures)
            {
                if (this.Contains(picture.Key, query))
                {
                    yield return picture.Value;
                }
            }

            yield break;
        }

        private bool Contains(string key, string query)
        {
            return key.Contains(new StringBuilder(query).Replace(' ', '_').ToString());
        }

        public async Task SendPictureAsync(HttpContext context, string? query, IDictionary<string, LinkedList<Image>>? pictures)
        {

            context.Response.ContentType = Program._contentBuilder
                .AddContentType(ContentType.ApplicationJson)
                .AddCharset(Charset.Utf8)
                .Build();

            Program._contentBuilder.Clear();

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
