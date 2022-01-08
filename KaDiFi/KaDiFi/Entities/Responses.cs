namespace KaDiFi.Entities
{
    public class MediaResult
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string CoverSource { get; set; }
        public string Description { get; set; }
        public int ViewsCount { get; set; }
    }

    public class AccessResult
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }

    public class GetMediaResult
    {
        public dynamic mediaItem { get; set; }
        public dynamic suggestedMedia { get; set; }
    }



























}
