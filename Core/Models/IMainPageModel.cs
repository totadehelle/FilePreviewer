using System.Threading;
using System.Threading.Tasks;

namespace Core.Models
{
    public interface IMainPageModel
    {
        Task<IContent> GetNewFile(string path, CancellationToken token);
        Task<IContent> GetNextFile(CancellationToken token);
        Task<IContent> GetPreviousFile(CancellationToken token);
    }
}