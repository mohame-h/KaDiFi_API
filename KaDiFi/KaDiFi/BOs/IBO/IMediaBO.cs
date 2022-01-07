using KaDiFi.Entities;

namespace KaDiFi.BOs.IBO
{
    public interface IMediaBO
    {
        General_Status CreateMedia(Media mediaObj);

        General_StatusWithData GetHomeMedia();
        General_StatusWithData GetSpecificMedia(string mediaId);
        General_StatusWithData GetCategoryMedia(int mediaCategory, int periodType);
        General_StatusWithData SearchMedia(string mediaName);

        General_Status UpdateMedia();

        General_Status RemoveMedia(string mediaId);

        string GetMediaDirectoryPath(int mediaCategory);

        General_StatusWithData AddComment(MediaCommentDTO model);
    }
}
