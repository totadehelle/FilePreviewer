using Windows.UI.Xaml.Media.Imaging;

namespace Core
{
    public class ImageContent : IContent
    {
        public ContentTypes Type { get; } = ContentTypes.Image;
        public BitmapImage Image { get; set; }
    }
}