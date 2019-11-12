using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Computer_Science_Final_Task.Content;
using Newtonsoft.Json;

namespace Computer_Science_Final_Task.Models
{
    public class MainPageModel : IMainPageModel
    {
        readonly Dictionary<string, Func<string, Task<IContent>>> ContentGetters = 
        new Dictionary<string, Func<string, Task<IContent>>>();

        public MainPageModel()
        {
            ContentGetters.Add(".txt", GetPlainTextContent);
            ContentGetters.Add(".json", GetJsonFileContent);
            ContentGetters.Add(".jpeg", GetImageContent);
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
            var textFile = await StorageFile.GetFileFromPathAsync(path);
            var text = await FileIO.ReadTextAsync(textFile);
            return new TextContent(){Text = text};
        }

        private async Task<IContent> GetJsonFileContent(string path)
        {
            var textFileFromExternalPath = await StorageFile.GetFileFromPathAsync(path);
            var text = await FileIO.ReadTextAsync(textFileFromExternalPath);
            var deserializedText = JsonConvert.DeserializeObject(text);
            var indentedText = JsonConvert.SerializeObject(deserializedText, Formatting.Indented);
            return new TextContent(){Text = indentedText };
        }

        private async Task<IContent> GetImageContent(string path)
        {
            BitmapImage image = new BitmapImage();
            var storageFile = await StorageFile.GetFileFromPathAsync(path);
            using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read))
            {
                await image.SetSourceAsync(stream);
            }
            return new ImageContent(){Image = image};
        }

        private bool ValidateFileType(string extension)
        {
            return ContentGetters.ContainsKey(extension);
        }
    }
}
