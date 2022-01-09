using KaDiFi.Entities;

namespace KaDiFi.BOs.IBO
{
    public interface IMediaBO
    {
        General_Status CreateMedia(Media mediaObj);

        General_StatusWithData GetHomeMedia();
        General_StatusWithData GetSpecificMedia(string mediaId, string userEmail, int commentsCount, int repliesCount);
        General_Status addOrRemoveMediaReact(string mediaId, int reactTypeId, string userEmail);
        General_Status AddComment(string mediaId, string commentText, string userEmail);
        General_Status EditComment(string commentId, string commentText, string userEmail);
        General_StatusWithData GetMediaComments(string mediaId, int itemsCount, int pageNumber, string userEmail);
        General_Status AddReply(string commentId, string replyText, string userEmail);
        General_Status EditReply(string replyId, string replyText, string userEmail);
        General_StatusWithData GetMediaReplies(string commentId, int itemsCount, int pageNumber, string userEmail);


        General_StatusWithData GetCategoryMedia(int mediaCategory, int periodType);
        General_StatusWithData SearchMedia(string mediaName);

        General_Status UpdateMedia();

        General_Status RemoveMedia(string mediaId);

        string GetMediaDirectoryPath(int mediaCategory);

    }
}
