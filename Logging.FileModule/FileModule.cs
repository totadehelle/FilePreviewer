using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Logging.FileOutput
{
	public class FileModule : IOutputModule, IDisposable
	{
		private readonly string _path;
        private readonly BlockingCollection<string> _messagesQueue;
		internal FileModule(string path)
		{
			_path = path;
            _messagesQueue = new BlockingCollection<string>();
            Task.Run(() =>
            {
                foreach (var message in _messagesQueue.GetConsumingEnumerable())
                {
                    try
                    {
                        ProcessQueueMessage(message);
                    }
                    catch (Exception e)
                    {
                        _messagesQueue.Dispose();
                        throw new LoggingFailedException("Logging to file failed. " + e.Message);
                    }
                }
            });
        }
		
        public void Write(string message)
		{
			_messagesQueue.Add(message);
		}

        private void ProcessQueueMessage(string message)
        {
            var actualPath = GetPathToLogfile(_path);
            var directory = Path.GetDirectoryName(actualPath);
            if (!(string.IsNullOrWhiteSpace(directory) || directory == ""))
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            using (var writer = File.AppendText(actualPath))
            {
                writer.WriteLine(message);
            };
        }

		private string GetPathToLogfile(string path)
		{
			var date = DateTime.UtcNow.Date.ToString("yyyyMMdd");
			var directory = Path.GetDirectoryName(path);
			var newFileName = Path.GetFileNameWithoutExtension(path) + date + Path.GetExtension(path);
			var newPath = Path.Combine(directory ?? "", newFileName);
			return newPath;
		}

        public void Dispose()
        {
            _messagesQueue.CompleteAdding();
            _messagesQueue.Dispose();
        }
    }
}
