using System.Threading.Tasks;
using Computer_Science_Final_Task.Content;

namespace Computer_Science_Final_Task.Models
{
    public interface IMainPageModel
    {
        Task<IContent> GetContent(string path);
    }
}