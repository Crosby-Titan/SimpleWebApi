namespace WebApplication4.Extensions
{
    internal static class ImageWorkerExtension
    {
        public static string GetShortFileName(this ImageWorker worker, string path)
        {
            string str = path.Substring(path.LastIndexOf('\\') + 1);

            return str.Remove(str.IndexOf('.'));
        }
        public static async Task<LinkedList<Image>?> GetImagesAsync(this ImageWorker worker, string? directory)
        {
            if (!Directory.Exists(directory) || directory == null)
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
                    var img = ImageWorker.CreateImage(file);

                    img.Tags.AppendLine(worker.GetShortFileName(file));

                    images.AddLast(img);
                });

            });

            return images;
        }
    }
}
