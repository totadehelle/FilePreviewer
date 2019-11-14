using System.Threading;
using System.Threading.Tasks;
using Caching;
using Computer_Science_Final_Task.Content;
using DataAccessLayer;

namespace Computer_Science_Final_Task.Models
{
    public class MainPageModelWithCaching : IMainPageModel
    {
        private readonly MainPageModel _baseModel;
        private readonly ICacheProvider _cache;
        private string[] FilesToCache => GetPreviousCurrentAndNextPaths();

        public History BrowsingHistory => _baseModel.BrowsingHistory;
        public int CurrentFileNumber => _baseModel.CurrentFileNumber;
        public int TotalFilesNumber => _baseModel.TotalFilesNumber;
        public bool NextFileExists => _baseModel.NextFileExists;
        public bool PreviousFileExists => _baseModel.PreviousFileExists;

        public MainPageModelWithCaching(IRepository repository, ICacheProvider cache)
        {
            _baseModel = new MainPageModel(repository);
            _cache = cache;
            _cache.OnCacheOverflow += TrimCache;
        }

        public async Task<IContent> GetNewFile(string path, CancellationToken token)
        {
            if (_cache.Get(path) is IContent cachedContent)
            {
                var newFileIsAdded = BrowsingHistory.Add(path);
                if (BrowsingHistory.Count != 1 && newFileIsAdded)
                    BrowsingHistory.CurrentIndex++;
                return cachedContent;
            }

            var content = await _baseModel.GetNewFile(path, token);
            _cache.Add(path, content);
            return content;
        }
        
        public async Task<IContent> GetNextFile(CancellationToken token)
        {
            var path = BrowsingHistory.GetNext();
            BrowsingHistory.CurrentIndex++;
            if (!NextFileExists) return _cache.Get(path) as IContent;

            var newNextPath = BrowsingHistory.GetNext();
            if (_cache.Contains(newNextPath)) return _cache.Get(path) as IContent;

            var newNextContent = await _baseModel.GetNextFile(token);
            BrowsingHistory.CurrentIndex--;
            _cache.Add(newNextPath, newNextContent);
            return _cache.Get(path) as IContent;
        }

        public async Task<IContent> GetPreviousFile(CancellationToken token)
        {
            var path = BrowsingHistory.GetPrevious();
            BrowsingHistory.CurrentIndex--;
            if (!PreviousFileExists) return _cache.Get(path) as IContent;

            var newPreviousPath = BrowsingHistory.GetPrevious();
            if (_cache.Contains(newPreviousPath)) return _cache.Get(path) as IContent;

            var newNextContent = await _baseModel.GetPreviousFile(token);
            BrowsingHistory.CurrentIndex++;
            _cache.Add(newPreviousPath, newNextContent);
            return _cache.Get(path) as IContent;
        }

        private string[] GetPreviousCurrentAndNextPaths()
        {
            if (!PreviousFileExists)
            {
                return !NextFileExists 
                    ? new[]{BrowsingHistory.GetCurrent()} 
                    : new [] { BrowsingHistory.GetCurrent(), BrowsingHistory.GetNext() };
            }
            return !NextFileExists
                ? new[] { BrowsingHistory.GetCurrent(), BrowsingHistory.GetPrevious() }
                : new [] { BrowsingHistory.GetCurrent(), BrowsingHistory.GetNext(), BrowsingHistory.GetPrevious() };
        }

        private void TrimCache()
        {
            _cache.Trim(FilesToCache);
        }
    }
}