using System.ComponentModel;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace Computer_Science_Final_Task.ViewModels
{
    public interface IMainPageViewModel
    {
        string TextSource { get; set; }
        BitmapImage ImageSource { get; set; }
        int CurrentFileNumber { get; set; }
        int TotalFilesNumber { get; set; }
        string FilePath { get; set; }
        bool NextEnabled { get; set; }
        bool PreviousEnabled { get; set; }
        ICommand PreviewCommand { get; }
        ICommand PreviousCommand { get; }
        ICommand NextCommand { get; }
        void ShowNewFile(string path);
        void ShowPreviousFile();
        void ShowNextFile();
        void Cleanup();
    }
}