using Windows.UI.Xaml.Media.Imaging;

namespace Computer_Science_Final_Task.Content
{
    public class ImageContent : IContent
    {
        public ContentTypes Type { get; } = ContentTypes.Image;
        public BitmapImage Image { get; set; }
    }
}