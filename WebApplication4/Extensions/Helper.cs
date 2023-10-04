using System.Text;

namespace WebApplication4.Extensions
{
    public interface IContentBuilder
    {
        public string Build();
        public IContentBuilder AddContentType(ContentType contentType);
        public IContentBuilder AddCharset(Charset charset);
    }

    public class ContentBuilder : IContentBuilder
    {
        private readonly StringBuilder _content;
        public ContentBuilder()
        {
            _content = new StringBuilder();
        }

        public string Build()
        {
            return _content.ToString();
        }

        public IContentBuilder AddContentType(ContentType contentType)
        {
            _content.Append(Helper.GetContentType(contentType));
            _content.Append(';');
            return this;
        }

        public IContentBuilder AddCharset(Charset charset)
        {
            _content.Append("charset=");
            _content.Append(Helper.GetCharset(charset));
            _content.Append(';');
            return this;
        }

        private bool IsBuilt()
        {
            return _content.Length > 0;
        }

        public void Clear()
        {
            if (!IsBuilt())
                return;

            _content.Clear();
        }
    }

    internal static class Helper
    {
        public static string GetContentType(ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.TextHtml:
                    return "text/html";
                case ContentType.TextPlain:
                    return "text/plain";
                case ContentType.ApplicationJson:
                    return "application/json";
                default:
                    return "text/html";
            }
        }
        public static string GetCharset(Charset charset)
        {
            switch (charset)
            {
                case Charset.Utf8:
                    return "utf-8";
                default:
                    return "utf-8";
            }
        }
    }

    public enum ContentType
    {
        TextHtml,
        TextPlain,
        ApplicationJson
    }

    public enum Charset
    {
        Utf8
    }

}
