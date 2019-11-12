using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Computer_Science_Final_Task.Content;
using Computer_Science_Final_Task.Models;
using Computer_Science_Final_Task.Utilities;
using GalaSoft.MvvmLight;

namespace Computer_Science_Final_Task.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IMainPageModel _model;
        readonly Dictionary<ContentTypes, Action<IContent>> ContentShowers =
            new Dictionary<ContentTypes, Action<IContent>>();
        #region Binding_Properties

        private string _textSource;
        private BitmapImage _imageSource;
        private int _currentFileNumber = 0;
        private int _totalFilesNumber = 0;
        private string _imageContentPath = null;
        private string _textContent = null;
        private string _filePath = null;

        public string TextSource {
            get => _textSource;
            set
            {
                _textSource = value;
                RaisePropertyChanged("TextSource");
            }
        }

        public BitmapImage ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                RaisePropertyChanged("ImageSource");
            }
        }
        
        public int CurrentFileNumber
        {
            get => _currentFileNumber;
            set
            {
                _currentFileNumber = value;
                RaisePropertyChanged("CurrentFileNumber");
            }
        }

        public int TotalFilesNumber
        {
            get => _totalFilesNumber;
            set
            {
                _totalFilesNumber = value;
                RaisePropertyChanged("TotalFilesNumber");
            }
        }

        public string ImageContentPath
        {
            get => _imageContentPath;
            set
            {
                _imageContentPath = value;
                RaisePropertyChanged("ImageContentPath");
            }
        }

        public string TextContent
        {
            get => _textContent;
            set
            {
                _textContent = value;
                RaisePropertyChanged("TextContent");
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                RaisePropertyChanged("FilePath");
            }
        }

        #endregion

        public MainPageViewModel(IMainPageModel model)
        {
            _model = model;
            ContentShowers.Add(ContentTypes.Text, ShowText);
            ContentShowers.Add(ContentTypes.Image, ShowImage);
        }

        private bool ValidatePath(string path)
        {
            try
            {
                Uri uri = new Uri(path);
                if (uri.Scheme == "file" && Path.HasExtension(path))
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #region Commands

        public ICommand PreviewCommand => new CommandHandler(() => ShowFileByPath(FilePath));
        public async void ShowFileByPath(string path)
        {
            try
            {
                if (!ValidatePath(path))
                {
                    await new MessageDialog($"{path} is not valid file path").ShowAsync();
                    return;
                }

                var content = await _model.GetContent(path);
                ContentShowers[content.Type].Invoke(content);
            }

            catch (Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
        }

        public ICommand PreviousCommand => new CommandHandler(ShowPreviousFile);
        public async void ShowPreviousFile()
        {
            try
            {
                
            }
            catch (Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
        }

        public ICommand NextCommand => new CommandHandler(ShowNextFile);
        public async void ShowNextFile()
        {
            try
            {

            }
            catch (Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
        }

        #endregion

        private void ShowText(IContent content)
        {
            var concreteContent = content as TextContent;
            TextSource = concreteContent?.Text;
        }

        private void ShowImage(IContent content)
        {
            var concreteContent = content as ImageContent;
            ImageSource = concreteContent?.Image;
        }
    }
}
