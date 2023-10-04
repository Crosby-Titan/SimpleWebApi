using System.Text.Json.Serialization;
using System.Text;

namespace WebApplication4
{
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
}
