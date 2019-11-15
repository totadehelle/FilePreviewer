using System.Threading;
using System.Threading.Tasks;
using Computer_Science_Final_Task.Content;
using Computer_Science_Final_Task.ViewModels;

namespace Computer_Science_Final_Task.Models
{
    public interface IMainPageModel
    {
        Task<IContent> GetNewFile(string path, CancellationToken token);
        Task<IContent> GetNextFile(CancellationToken token);
        Task<IContent> GetPreviousFile(CancellationToken token);
    }
}