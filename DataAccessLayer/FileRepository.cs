using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class FileRepository : IRepository
    {
       public async Task<byte[]> GetContentAsync(Stream stream)
        {
            byte[] contentBytes; // = await Task.Run(() => File.ReadAllBytes(path));
            using (stream)
            {
                contentBytes = new byte[stream.Length];
                await stream.ReadAsync(contentBytes, 0, (int)stream.Length);
            }
            
            return contentBytes;
        }
    }
}