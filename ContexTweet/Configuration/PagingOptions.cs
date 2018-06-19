namespace ContexTweet.Configuration
{
    public class PagingOptions
    {
        public int PageSize { get; set; } = 5;

        public PagingOptions()
        {
            PageSize = 5;
        }
    }
}
