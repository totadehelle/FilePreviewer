using System;
using System.IO;
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
        private readonly History _history;
        private bool NextFileExists => _history.NextFileExists;
        private bool PreviousFileExists => _history.PreviousFileExists;

        public MainPageModelWithCaching(IRepository repository, ICacheProvider cache, History history)
        {
            _history = history;
            _baseModel = new MainPageModel(repository, _history);
            _cache = cache;
            _cache.OnCacheOverflow += TrimCache;
        }

        public async Task<IContent> GetNewFile(string path, CancellationToken token)
        {
            if (_cache.Get(path) is IContent cachedContent)
            {
                var newFileIsAdded = _history.Add(path);
                if (_history.Count != 1 && newFileIsAdded)
                    _history.CurrentIndex++;
                return cachedContent;
            }

            var content = await _baseModel.GetNewFile(path, token);
            _cache.Add(path, content);
            return content;
        }
        
        public async Task<IContent> GetNextFile(CancellationToken token)
        {
            var path = _history.GetNext();
            _history.CurrentIndex++;
            if (!NextFileExists) return _cache.Get(path) as IContent;

            var newNextPath = _history.GetNext();
            if (_cache.Contains(newNextPath)) return _cache.Get(path) as IContent;

            bool cachingIsCompleted = false;
            while (!cachingIsCompleted)
            {
                try
                {
                    var newNextContent = await _baseModel.GetContent(newNextPath, token);
                    _cache.Add(newNextPath, newNextContent);
                    cachingIsCompleted = true;
                }
                catch (FileNotFoundException e)
                {
                    _history.Remove(newNextPath);
                    cachingIsCompleted = !NextFileExists;
                    if(!cachingIsCompleted)
                        newNextPath = _history.GetNext();
                }
            }
            return _cache.Get(path) as IContent;
        }

        public async Task<IContent> GetPreviousFile(CancellationToken token)
        {
            var path = _history.GetPrevious();
            _history.CurrentIndex--;
            if (!PreviousFileExists) return _cache.Get(path) as IContent;

            var newPreviousPath = _history.GetPrevious();
            if (_cache.Contains(newPreviousPath)) return _cache.Get(path) as IContent;

            bool cachingIsCompleted = false;
            while (!cachingIsCompleted)
            {
                try
                {
                    var newPreviousContent = await _baseModel.GetContent(newPreviousPath, token);
                    _cache.Add(newPreviousPath, newPreviousContent);
                    cachingIsCompleted = true;
                }
                catch (FileNotFoundException e)
                {
                    _history.Remove(newPreviousPath);
                    _history.CurrentIndex--;
                    cachingIsCompleted = !PreviousFileExists;
                    if (!cachingIsCompleted)
                        newPreviousPath = _history.GetNext();
                }
            }
            return _cache.Get(path) as IContent;
        }

        private string[] GetPreviousCurrentAndNextPaths()
        {
            if (!PreviousFileExists)
            {
                return !NextFileExists 
                    ? new[]{ _history.GetCurrent()} 
                    : new [] { _history.GetCurrent(), _history.GetNext() };
            }
            return !NextFileExists
                ? new[] { _history.GetCurrent(), _history.GetPrevious() }
                : new [] { _history.GetCurrent(), _history.GetNext(), _history.GetPrevious() };
        }

        private void TrimCache()
        {
            _cache.Trim(FilesToCache);
        }
    }
}