using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Computer_Science_Final_Task.Content;
using DataAccessLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Computer_Science_Final_Task.Models
{
    public class MainPageModel : IMainPageModel
    {
        private readonly Dictionary<string, Func<byte[], IContent>> _contentFormatters;
        private readonly IRepository _repository;
        public History BrowsingHistory { get; }

        public int CurrentFileNumber => BrowsingHistory.CurrentIndex + 1;
        public int TotalFilesNumber => BrowsingHistory.Count;
        public bool NextFileExists => BrowsingHistory.NextFileExists;
        public bool PreviousFileExists => BrowsingHistory.PreviousFileExists;

        public MainPageModel(IRepository repository)
        {
            BrowsingHistory = new History();
            _contentFormatters = new Dictionary<string, Func<byte[], IContent>>
            {
                {".txt", FormatPlainTextContent}, 
                {".json", FormatJsonContent}, 
                {".jpeg", FormatImageContent}
            };
            _repository = repository;
        }

        public async Task<IContent> GetNewFile(string path)
        {
            var content = await GetContent(path);
            bool newFileIsAdded = BrowsingHistory.Add(path);
            if (BrowsingHistory.Count != 1 && newFileIsAdded)
               BrowsingHistory.CurrentIndex++;
            return content;
        }

        public async Task<IContent> GetNextFile()
        {
            var path = BrowsingHistory.GetNext();
            var content = await GetContent(path);
            BrowsingHistory.CurrentIndex++;
            return content;
        }

        public async Task<IContent> GetPreviousFile()
        {
            var path = BrowsingHistory.GetPrevious();
            var content = await GetContent(path);
            BrowsingHistory.CurrentIndex--;
            return content;
        }

        private async Task<IContent> GetContent(string path)
        {
            var ext = Path.GetExtension(path);
            if (!ValidateFileType(ext))
                throw new Exception($"Files of type {ext} are not supported");

            var file = await StorageFile.GetFileFromPathAsync(path);
            var stream = await file.OpenStreamForReadAsync();

            var content = await _repository.GetContentAsync(stream);
            return _contentFormatters[ext].Invoke(content);
        }

        private IContent FormatPlainTextContent(byte[] content)
        {
            var text = Encoding.UTF8.GetString(content);
            
            return new TextContent(){Text = text};
        }

        private IContent FormatJsonContent(byte[] content)
        {
            var token = JToken.Parse(Encoding.UTF8.GetString(content));
            var indentedText = token.ToString(Formatting.Indented);

            return new TextContent(){Text = indentedText };
        }

        private IContent FormatImageContent(byte[] content)
        {
            BitmapImage image = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                stream.WriteAsync(content.AsBuffer()).GetResults();
                stream.Seek(0);
                image.SetSource(stream);
            }
            return new ImageContent(){Image = image};
        }

        private bool ValidateFileType(string extension)
        {
            return _contentFormatters.ContainsKey(extension);
        }
    }
}
