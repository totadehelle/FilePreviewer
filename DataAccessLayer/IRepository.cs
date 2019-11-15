using System;
using System.IO;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface IRepository : IDisposable
    {
        Task<byte[]> GetContentAsync(Stream stream);
    }
}