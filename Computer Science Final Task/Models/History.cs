using System;
using System.Collections.Generic;

namespace Computer_Science_Final_Task.Models
{
    public class History
    {
        private readonly List<string> _paths;
        private int LastIndex => _paths.Count - 1;
        public int CurrentIndex { get; set; }

        public int Count => _paths.Count;
        public bool NextFileExists => CurrentIndex < LastIndex;
        public bool PreviousFileExists => CurrentIndex > 0;

        public History()
        {
            _paths = new List<string>();
        }

        public bool Add(string path)
        {
            if (NextFileExists)
            {
                _paths.RemoveRange(CurrentIndex + 1, LastIndex - CurrentIndex);
            }
            if (Count > 0 && _paths[LastIndex] == path)
                return false;
            _paths.Add(path);
            return true;
        }

        public void Remove(string path)
        {
            _paths.Remove(path);
        }

        public string GetCurrent()
        {
            if (Count <= 0)
                throw new Exception("History is empty, there is no current file");
            return _paths[CurrentIndex];
        }

        public string GetNext()
        {
            if (!NextFileExists)
                throw new Exception("There is no next file, but it was requested");
            return _paths[CurrentIndex+1];
        }

        public string GetPrevious()
        {
            if (!PreviousFileExists)
                throw new Exception("There is no previous file, but it was requested");
            return _paths[CurrentIndex-1];
        }
    }
}