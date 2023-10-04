using System.Text;
using WebApplication4.Extensions;

namespace WebApplication4
{
    public class ProjectPath
    {
        public  string? ApplicationPath { get; private set; }
        public  string? ImagesPath { get; private set; }
        public  string? HtmlPath { get; private set; }
        public  string? JsPath { get; private set; }
        public  string? CssPath { get; private set; }

        public void InitializePaths()
        {
            var sb = new StringBuilder(Environment.ProcessPath);

            ApplicationPath = sb.Replace("bin", "_")
                .Remove(sb.IndexOf('_'))
                .ToString();

            ImagesPath = Path.Combine(ApplicationPath, "files\\Resources\\Images");
            HtmlPath = Path.Combine(ApplicationPath, "files\\HTML");
            JsPath = Path.Combine(ApplicationPath, "files\\JS");
            CssPath = Path.Combine(ApplicationPath, "files\\CSS");
        }
    }
}
