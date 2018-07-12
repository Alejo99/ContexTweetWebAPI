namespace ContexTweet.Configuration
{
    public class PagingOptions
    {
        public virtual int PageSize { get; set; } = 5;

        public PagingOptions()
        {
            PageSize = 5;
        }
    }
}
