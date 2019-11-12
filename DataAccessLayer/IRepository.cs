using System.IO;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface IRepository
    {
        Task<byte[]> GetContentAsync(Stream stream);
    }
}