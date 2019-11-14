using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
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
        private readonly Dictionary<ContentTypes, Action<IContent>> _contentShowers;

        private CancellationTokenSource _previewCommandTokenSource;
        private CancellationTokenSource _nextCommandTokenSource;
        private CancellationTokenSource _previousCommandTokenSource;

        #region Binding_Properties

        private string _textSource;
        private BitmapImage _imageSource;
        private int _currentFileNumber;
        private int _totalFilesNumber;
        private string _filePath;
        private bool _nextEnabled;
        private bool _previousEnabled;
        private Visibility _imageContentIsVisible;
        private Visibility _textContentIsVisible;

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
        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                RaisePropertyChanged("FilePath");
            }
        }
        public bool NextEnabled
        {
            get => _nextEnabled;
            set
            {
                _nextEnabled = value;
                RaisePropertyChanged("NextEnabled");
            }
        }
        public bool PreviousEnabled
        {
            get => _previousEnabled;
            set
            {
                _previousEnabled = value;
                RaisePropertyChanged("PreviousEnabled");
            }
        }
        public Visibility ImageContentIsVisible
        {
            get => _imageContentIsVisible;
            set
            {
                _imageContentIsVisible = value;
                RaisePropertyChanged("ImageContentIsVisible");
            }
        }

        public Visibility TextContentIsVisible
        {
            get => _textContentIsVisible;
            set
            {
                _textContentIsVisible = value;
                RaisePropertyChanged("TextContentIsVisible");
            }
        }

        #endregion

        public MainPageViewModel(IMainPageModel model)
        {
            _model = model;
            _contentShowers = new Dictionary<ContentTypes, Action<IContent>>
            {
                {ContentTypes.Text, ShowText}, 
                {ContentTypes.Image, ShowImage}
            };
        }
        
        #region Commands

        public ICommand PreviewCommand => new CommandHandler(() => ShowNewFile(FilePath));
        public async void ShowNewFile(string path)
        {
            try
            {
                _previousCommandTokenSource?.Cancel();
                _nextCommandTokenSource?.Cancel();
                _previewCommandTokenSource = new CancellationTokenSource();
                if (!ValidatePath(path))
                {
                    await new MessageDialog($"{path} is not valid file path").ShowAsync();
                    return;
                }

                var content = await _model.GetNewFile(path, _previewCommandTokenSource.Token);
                ShowContent(content);
                SwitchButtons();
                RefreshPagination();
            }

            catch (Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
            finally
            {
                var oldTokenSource = _previewCommandTokenSource;
                _previewCommandTokenSource = null;
                oldTokenSource.Dispose();
            }
        }

        public ICommand PreviousCommand => new CommandHandler(ShowPreviousFile);
        public async void ShowPreviousFile()
        {
            try
            {
                _previewCommandTokenSource?.Cancel();
                _nextCommandTokenSource?.Cancel();
                _previousCommandTokenSource = new CancellationTokenSource();
                var content = await _model.GetPreviousFile(_previousCommandTokenSource.Token);
                ShowContent(content);
                SwitchButtons();
                RefreshPagination();
            }
            catch (Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
            finally
            {
                var oldTokenSource = _previousCommandTokenSource;
                _previousCommandTokenSource = null;
                oldTokenSource.Dispose();
            }
        }

        public ICommand NextCommand => new CommandHandler(ShowNextFile);
        public async void ShowNextFile()
        {
            try
            {
                _previewCommandTokenSource?.Cancel();
                _previousCommandTokenSource?.Cancel();
                _nextCommandTokenSource = new CancellationTokenSource();
                var content = await _model.GetNextFile(_nextCommandTokenSource.Token);
                ShowContent(content);
                SwitchButtons();
                RefreshPagination();
            }
            catch (Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
            finally
            {
                var oldTokenSource = _nextCommandTokenSource;
                _nextCommandTokenSource = null;
                oldTokenSource.Dispose();
            }
        }

        #endregion

        private void ShowContent(IContent content)
        {
           _contentShowers[content.Type].Invoke(content);
        }
        
        private void ShowText(IContent content)
        {
            ImageContentIsVisible = Visibility.Collapsed;
            var concreteContent = content as TextContent;
            TextSource = null;
            TextSource = concreteContent?.Text;
            TextContentIsVisible = Visibility.Visible;
        }

        private void ShowImage(IContent content)
        {
            TextContentIsVisible = Visibility.Collapsed;
            var concreteContent = content as ImageContent;
            ImageSource = null;
            ImageSource = concreteContent?.Image;
            ImageContentIsVisible = Visibility.Visible;
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

        private void SwitchButtons()
        {
            NextEnabled = _model.NextFileExists;
            PreviousEnabled = _model.PreviousFileExists;
        }

        private void RefreshPagination()
        {
            CurrentFileNumber = _model.CurrentFileNumber;
            TotalFilesNumber = _model.TotalFilesNumber;
        }
    }
}
