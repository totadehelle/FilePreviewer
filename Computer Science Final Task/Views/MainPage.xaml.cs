using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Autofac;
using Computer_Science_Final_Task.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Computer_Science_Final_Task.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; private set; }
        
        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = App.Container.Resolve<MainPageViewModel>();
            this.DataContext = ViewModel;
        }
    }
}
