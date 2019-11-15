namespace Core
{
    public class TextContent : IContent
    {
        public ContentTypes Type { get; } = ContentTypes.Text;
        public string Text { get; set; }
    }
}