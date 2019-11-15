using System;
using System.IO;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class FileRepository : IRepository
    {
        private Stream _stream;
        public async Task<byte[]> GetContentAsync(Stream stream)
        {
            _stream = stream;
            byte[] contentBytes;
            using (stream)
            {
                contentBytes = new byte[stream.Length];
                await stream.ReadAsync(contentBytes, 0, (int)stream.Length);
            }

            _stream = null;
            return contentBytes;
        }

       private void ReleaseUnmanagedResources()
       {
           _stream?.Dispose();
       }

       public void Dispose()
       {
           ReleaseUnmanagedResources();
           GC.SuppressFinalize(this);
       }

       ~FileRepository()
       {
           ReleaseUnmanagedResources();
       }
    }
}