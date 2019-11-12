namespace Computer_Science_Final_Task.Content
{
    public class TextContent : IContent
    {
        public ContentTypes Type { get; } = ContentTypes.Text;
        public string Text { get; set; }
    }
}