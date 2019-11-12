using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Caching;
using Computer_Science_Final_Task.Content;
using Newtonsoft.Json;

namespace Computer_Science_Final_Task.Models
{
    public class MainPageModel : IMainPageModel
    {
        readonly Dictionary<string, Func<string, Task<IContent>>> ContentGetters = 
        new Dictionary<string, Func<string, Task<IContent>>>();

        private ICacheProvider _cache;

        public MainPageModel(ICacheProvider cache)
        {
            ContentGetters.Add(".txt", GetPlainTextContent);
            ContentGetters.Add(".json", GetJsonFileContent);
            ContentGetters.Add(".jpeg", GetImageContent);
            _cache = cache;
        }

        public async Task<IContent> GetContent(string path)
        {
            var ext = Path.GetExtension(path);
            if(!ValidateFileType(ext))
                throw new Exception($"Files of type {ext} are not supported");
            
            return await ContentGetters[ext].Invoke(path);
        }

        private async Task<IContent> GetPlainTextContent(string path)
        {
            if(_cache.Get(path) is string cachedContent)
                return new TextContent() { Text = cachedContent };

            var textFile = await StorageFile.GetFileFromPathAsync(path);
            var text = await FileIO.ReadTextAsync(textFile);

            _cache.Add(path, text);

            return new TextContent(){Text = text};
        }

        private async Task<IContent> GetJsonFileContent(string path)
        {
            if (_cache.Get(path) is string cachedContent)
                return new TextContent() { Text = cachedContent };

            var textFileFromExternalPath = await StorageFile.GetFileFromPathAsync(path);
            var text = await FileIO.ReadTextAsync(textFileFromExternalPath);
            var deserializedText = JsonConvert.DeserializeObject(text);
            var indentedText = JsonConvert.SerializeObject(deserializedText, Formatting.Indented);

            _cache.Add(path, indentedText);

            return new TextContent(){Text = indentedText };
        }

        private async Task<IContent> GetImageContent(string path)
        {
            if (_cache.Get(path) is BitmapImage cachedContent)
                return new ImageContent() { Image = cachedContent };

            BitmapImage image = new BitmapImage();
            var storageFile = await StorageFile.GetFileFromPathAsync(path);
            using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read))
            {
                await image.SetSourceAsync(stream);
            }
            
            _cache.Add(path, image);

            return new ImageContent(){Image = image};
        }

        private bool ValidateFileType(string extension)
        {
            return ContentGetters.ContainsKey(extension);
        }
    }
}
