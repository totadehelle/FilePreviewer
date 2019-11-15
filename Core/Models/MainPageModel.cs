using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using DataAccessLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Models
{
    public class MainPageModel : IMainPageModel
    {
        private readonly Dictionary<string, Func<byte[], IContent>> _contentFormatters;
        private readonly IRepository _repository;
        private readonly History _history;

        public MainPageModel(IRepository repository, History history)
        {
            _history = history;
            _contentFormatters = new Dictionary<string, Func<byte[], IContent>>
            {
                {".txt", FormatPlainTextContent}, 
                {".json", FormatJsonContent}, 
                {".jpeg", FormatImageContent}
            };
            _repository = repository;
        }

        public async Task<IContent> GetNewFile(string path, CancellationToken token)
        {
            //Task need to be awaited here for adding to history only valid file paths
            token.ThrowIfCancellationRequested();
            var content = await GetContent(path, token);
            bool newFileIsAdded = _history.Add(path);
            if (_history.Count != 1 && newFileIsAdded)
               _history.CurrentIndex++;
            return content;
        }

        public async Task<IContent> GetNextFile(CancellationToken token)
        {
            var path = _history.GetNext();
            try
            {
                //Task need to be awaited here for adding to history only valid file paths
                var content = await GetContent(path, token);
                _history.CurrentIndex++;
                return content;
            }
            catch (FileNotFoundException e)
            {
                _history.Remove(path);
                throw new InvalidHistoryException("Next file is not found, probably it was deleted " +
                                                  "or its name was changed. History is refreshed");
            }
        }

        public async Task<IContent> GetPreviousFile(CancellationToken token)
        {
            var path = _history.GetPrevious();
            try
            {
                //Task need to be awaited here for adding to history only valid file paths
                var content = await GetContent(path, token);
                _history.CurrentIndex--;
                return content;
            }
            catch (FileNotFoundException e)
            {
                _history.Remove(path);
                _history.CurrentIndex--;
                throw new InvalidHistoryException("Previous file is not found, probably it was deleted " +
                                                  "or its name was changed. History is refreshed");
            }
        }

        public async Task<IContent> GetContent(string path, CancellationToken token)
        {
            var ext = Path.GetExtension(path);
            if (!ValidateFileType(ext))
                throw new NotSupportedException($"Files of type {ext} are not supported");

            token.ThrowIfCancellationRequested();
            var file = await StorageFile.GetFileFromPathAsync(path);
            var stream = await file.OpenStreamForReadAsync();

            token.ThrowIfCancellationRequested();
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
