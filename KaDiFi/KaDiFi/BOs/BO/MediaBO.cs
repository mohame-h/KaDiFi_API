using KaDiFi.BOs.IBO;
using KaDiFi.Entities;
using KaDiFi.Helpers;
using KaDiFi.Helpers.IHelper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KaDiFi.BOs
{
    public class MediaBO : IMediaBO
    {
        private KaDifiEntities _db;
        private IAuthenticationHelper _auth;
        private IConfiguration _configuration;

        public MediaBO(KaDifiEntities db, IAuthenticationHelper auth, IConfiguration configuration)
        {
            _db = db;
            _auth = auth;
            _configuration = configuration;
        }

        public General_StatusWithData GetHomeMedia()
        {
            var result = new General_StatusWithData();

            try
            {
                var homeMedia = new Dictionary<string, List<MediaResult>>();

                var recent = (from tblm in _db.Media
                              join tblv in _db.MediaViews
                              on tblm.Id equals tblv.MediaId

                              select new
                              {
                                  tblm.Id,
                                  tblm.Title,
                                  tblm.CoverSource,
                                  tblm.Description,
                                  ViewedAt = tblv.CreatedAt,
                              }
                             )
                             .OrderByDescending(z => z.ViewedAt)
                             .Take(8)
                             .Select(z => new MediaResult
                             {
                                 Id = z.Id,
                                 Title = z.Title,
                                 CoverSource = z.CoverSource,
                                 Description = string.Join(" ", z.Description.Split().Take(20)),
                                 ViewsCount = _db.MediaViews.Count(x => x.MediaId == z.Id)
                             })
                             .ToList();
                homeMedia.Add("Recent", recent);

                var recommendedMediaId = _db.Media.Select(z => new
                {
                    z.Id,
                    Likes = _db.MediaViews.Count(x => x.MediaId == z.Id && x.React == (int)MediaReactTypes.Like),
                    Dislikes = _db.MediaViews.Count(x => x.MediaId == z.Id && x.React == (int)MediaReactTypes.Dislike),
                })
                            .Where(z => z.Likes > 0 && z.Dislikes > 0 && ((z.Likes / z.Dislikes) > 3))
                            .Select(z => z.Id)
                            .ToList();
                var recommended = (
                                    from tblm in _db.Media
                                    join tblv in _db.MediaViews.Where(z => recommendedMediaId.Contains(z.Id))
                                    on tblm.Id equals tblv.MediaId

                                    select new
                                    {
                                        tblm.Id,
                                        tblm.Title,
                                        tblm.CoverSource,
                                        tblm.Description,
                                        ViewedAt = tblv.CreatedAt,
                                        ReactType = tblv.React
                                    }
                                )
                                .Take(8)
                                .Select(z => new MediaResult
                                {
                                    Id = z.Id,
                                    Title = z.Title,
                                    CoverSource = z.CoverSource,
                                    Description = z.Description,
                                    ViewsCount = _db.MediaViews.Count(x => x.MediaId == z.Id)
                                })
                                .ToList();
                homeMedia.Add("Recommended", recommended);

                var cartoons = _db.Media.Where(z => z.CategoryId == (int)MediaCategories.Cartoons)
                                        .Take(8)
                                        .Select(z => new MediaResult
                                        {
                                            Id = z.Id,
                                            Title = z.Title,
                                            CoverSource = z.CoverSource,
                                            Description = z.Description,
                                            ViewsCount = _db.MediaViews.Count(x => x.MediaId == z.Id)
                                        })
                                        .ToList();
                homeMedia.Add(MediaCategories.Cartoons.ToString(), cartoons);

                var sports = _db.Media.Where(z => z.CategoryId == (int)MediaCategories.Sports)
                                        .Take(8)
                                        .Select(z => new MediaResult
                                        {
                                            Id = z.Id,
                                            Title = z.Title,
                                            CoverSource = z.CoverSource,
                                            Description = z.Description,
                                            ViewsCount = _db.MediaViews.Count(x => x.MediaId == z.Id)
                                        })
                                        .ToList();
                homeMedia.Add(MediaCategories.Sports.ToString(), sports);

                result.Data = homeMedia;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = General_Strings.APIIssueMessage;
            }

            return result;
        }
        public General_StatusWithData GetSpecificMedia(string mediaId, string userEmail, int commentsCount, int repliesCount)
        {
            var result = new General_StatusWithData();

            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == userEmail);
                if (user == null)
                {
                    result.ErrorMessage = "Error while retreiving media, Please refresh the page!";
                    return result;
                }

                var media = _db.Media.FirstOrDefault(z => z.Id == mediaId);
                if (media == null)
                    throw new Exception();

                var mediaItem = _db.Media.Where(z => z.Id == mediaId)
                                        .Select(z => new
                                        {
                                            MediaId = z.Id,
                                            z.Title,
                                            z.CategoryId,
                                            z.Source,
                                            z.Description,
                                            z.CreatedAt,
                                            ViewsCount = _db.MediaViews.Count(x => x.MediaId == mediaId),
                                            UserReact = _db.MediaViews.FirstOrDefault(x => x.MediaId == mediaId && x.UserId == user.Id),
                                            Comments = (from tblc in _db.MediaComment.Where(c => c.MediaId == z.Id)
                                                        join tblu in _db.User
                                                        on tblc.CommenterId equals tblu.Id
                                                        select new
                                                        {
                                                            CommentId = tblc.Id,
                                                            CommenterName = tblu.Name,
                                                            CommentText = tblc.Body,
                                                            CommentCreationTime = tblc.CreatedAt,
                                                            IsCurrenctUser = (tblu.Id == user.Id)
                                                        })
                                                        .OrderByDescending(oo => oo.CommentCreationTime)
                                                        .Take(commentsCount)
                                                        .Select(c => new
                                                        {
                                                            c.CommentId,
                                                            c.CommenterName,
                                                            c.CommentText,
                                                            c.CommentCreationTime,
                                                            c.IsCurrenctUser,
                                                            TotalLikesCount = _db.MediaViews.Count(z => z.React == (int)MediaReactTypes.Like),
                                                            TotalDislikesCount = _db.MediaViews.Count(z => z.React == (int)MediaReactTypes.Dislike),
                                                            LastReply = _db.MediaCommentReply.Where(r => r.CommentId == c.CommentId)
                                                                        .OrderByDescending(o => o.DeletedAt == null ? o.UpdatedAt == null ? o.CreatedAt : o.UpdatedAt : o.DeletedAt)
                                                                        .Take(1)
                                                                        .Select(r => new
                                                                        {
                                                                            r.CommentId,
                                                                            ReplyId = r.Id,
                                                                            Replier = _db.User.FirstOrDefault(u => u.Id == user.Id),
                                                                            ReplyText = r.Body,
                                                                            ReplyCreationTime = r.CreatedAt,
                                                                        })
                                                                        .Select(r => new
                                                                        {
                                                                            r.CommentId,
                                                                            r.ReplyId,
                                                                            ReplierName = r.Replier == null ? "" : r.Replier.Name,
                                                                            r.ReplyText,
                                                                            r.ReplyCreationTime,
                                                                            IsCurrentUser = r.Replier == null ? false : (r.Replier.Id == user.Id)
                                                                        }).ToList(),
                                                        }).ToList(),

                                        })
                                        .Select(z => new
                                        {
                                            z.MediaId,
                                            z.Title,
                                            z.CategoryId,
                                            z.Source,
                                            z.Description,
                                            z.CreatedAt,
                                            z.ViewsCount,
                                            z.UserReact.React,
                                            z.Comments

                                        }).FirstOrDefault();

                if (mediaItem == null)
                {
                    result.ErrorMessage = "Error while retreiving media, Please refresh the page!";
                    return result;
                }

                var suggestedMedia = _db.Media.Where(z => z.CategoryId == mediaItem.CategoryId && z.Id != mediaId)
                                        .Select(z => new
                                        {
                                            z.Id,
                                            z.Title,
                                            z.CoverSource,
                                            z.Description,
                                            View = _db.MediaViews.Where(x => x.MediaId == z.Id).OrderByDescending(x => x.CreatedAt).FirstOrDefault(),
                                        })
                                        .Select(z => new
                                        {
                                            z.Id,
                                            z.Title,
                                            z.CoverSource,
                                            Description = string.Join(" ", z.Description.Split().Take(20)),
                                            ViewedAt = z.View == null ? DateTime.Now.AddYears(-20) : z.View.CreatedAt,
                                        })
                                        .OrderByDescending(z => z.ViewedAt)
                                        .Take(8)
                                        .Select(z => new MediaResult
                                        {
                                            Id = z.Id,
                                            Title = z.Title,
                                            CoverSource = z.CoverSource,
                                            Description = string.Join(" ", z.Description.Split().Take(20)),
                                            ViewsCount = _db.MediaViews.Count(x => x.MediaId == z.Id)
                                        })
                                        .ToList();

                var mediaView = new MediaViews();
                mediaView.Id = Guid.NewGuid().ToString();
                mediaView.UserId = user.Id;
                mediaView.MediaId = mediaId;
                mediaView.React = (int)MediaReactTypes.None;
                _db.MediaViews.Add(mediaView);
                _db.SaveChanges();

                result.Data = new GetMediaResult() { mediaItem = mediaItem, suggestedMedia = suggestedMedia };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = General_Strings.APIIssueMessage;
            }

            return result;
        }
        public General_Status addOrRemoveMediaReact(string mediaId, int reactTypeId, string userEmail)
        {
            var result = new General_Status();

            try
            {
                var userId = _db.User.FirstOrDefault(z => z.Email == userEmail)?.Id;
                var view = _db.MediaViews.Where(z => z.MediaId == mediaId && z.UserId == userId).OrderByDescending(z => z.CreatedAt).FirstOrDefault();
                if (view == null)
                    throw new Exception();

                view.React = reactTypeId;
                view.UpdatedAt = DateTime.Now;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = General_Strings.APIIssueMessage;
            }

            return result;
        }
        public General_Status AddComment(string mediaId, string commentText, string userEmail)
        {
            var result = new General_Status();
            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == userEmail);
                if (user == null)
                    throw new Exception();

                var newComment = new MediaComment();
                newComment.Id = Guid.NewGuid().ToString();
                newComment.CommenterId = user.Id;
                newComment.MediaId = mediaId;
                newComment.Body = commentText;
                newComment.CreatedAt = DateTime.Now;
                _db.MediaComment.Add(newComment);
                _db.SaveChanges();

                //var comments = (
                //            from tblcomment in _db.MediaComment.Where(z => z.MediaId == mediaId && z.IsActive)
                //            join tbluser in _db.User
                //            on tblcomment.CommenterId equals tbluser.Id
                //            select new
                //            {
                //                userName = tbluser.IsActive ? tbluser.Name : "",
                //                commentBody = tbluser.IsActive ? tblcomment.Body : "",
                //                commentDate = tblcomment.UpdatedAt == null ? tblcomment.CreatedAt : tblcomment.UpdatedAt,
                //                isCurrenctUser = tbluser.Id == user.Id ? true : false,
                //                isActiveUser = tbluser.IsActive
                //            })
                //            .OrderByDescending(z => z.commentDate)
                //            .ToList();

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }
        public General_Status EditComment(string commentId, string commentText, string userEmail)
        {
            var result = new General_Status();
            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == userEmail);
                if (user == null)
                    throw new Exception();

                var comment = _db.MediaComment.FirstOrDefault(z => z.Id == commentId);
                if (comment == null)
                    throw new Exception();

                comment.Body = commentText;
                comment.UpdatedAt = DateTime.Now;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }
        public General_StatusWithData GetMediaComments(string mediaId, int itemsCount, int pageNumber, string userEmail)
        {
            var result = new General_StatusWithData();
            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == userEmail);
                if (user == null)
                    throw new Exception();

                var comments = (from tblc in _db.MediaComment.Where(c => c.MediaId == mediaId)
                                join tblu in _db.User
                                on tblc.CommenterId equals tblu.Id
                                select new
                                {
                                    CommentId = tblc.Id,
                                    CommenterName = tblu.Name,
                                    CommentText = tblc.Body,
                                    CommentCreationTime = tblc.CreatedAt,
                                    IsCurrenctUser = (tblu.Id == user.Id)
                                })
                                .OrderByDescending(oo => oo.CommentCreationTime)
                                .Skip(pageNumber * itemsCount)
                                .Take(itemsCount)
                                .Select(c => new
                                {
                                    c.CommentId,
                                    c.CommenterName,
                                    c.CommentText,
                                    c.CommentCreationTime,
                                    c.IsCurrenctUser,
                                    TotalLikesCount = _db.MediaViews.Count(z => z.React == (int)MediaReactTypes.Like),
                                    TotalDislikesCount = _db.MediaViews.Count(z => z.React == (int)MediaReactTypes.Dislike),
                                    LastReply = _db.MediaCommentReply.Where(r => r.CommentId == c.CommentId)
                                                .OrderByDescending(o => o.DeletedAt == null ? o.UpdatedAt == null ? o.CreatedAt : o.UpdatedAt : o.DeletedAt)
                                                .Take(1)
                                                .Select(r => new
                                                {
                                                    r.CommentId,
                                                    ReplyId = r.Id,
                                                    Replier = _db.User.FirstOrDefault(u => u.Id == user.Id),
                                                    ReplyText = r.Body,
                                                    ReplyCreationTime = r.CreatedAt,
                                                })
                                                .Select(r => new
                                                {
                                                    r.CommentId,
                                                    r.ReplyId,
                                                    ReplierName = r.Replier == null ? "" : r.Replier.Name,
                                                    r.ReplyText,
                                                    r.ReplyCreationTime,
                                                    IsCurrentUser = r.Replier == null ? false : (r.Replier.Id == user.Id)
                                                }).ToList()
                                })
                                .ToList();

                result.Data = comments;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }
        public General_Status AddReply(string commentId, string replyText, string userEmail)
        {
            var result = new General_Status();
            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == userEmail);
                var comment = _db.MediaComment.FirstOrDefault(z => z.Id == commentId);
                if (user == null || comment == null)
                    throw new Exception();

                var newReply = new MediaCommentReply();
                newReply.Id = Guid.NewGuid().ToString();
                newReply.ReplierId = user.Id;
                newReply.CommentId = commentId;
                newReply.Body = replyText;
                newReply.CreatedAt = DateTime.Now;
                _db.MediaCommentReply.Add(newReply);
                _db.SaveChanges();

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }
        public General_Status EditReply(string replyId, string replyText, string userEmail)
        {
            var result = new General_Status();
            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == userEmail);
                var reply = _db.MediaCommentReply.FirstOrDefault(z => z.Id == replyId);
                if (user == null || reply == null)
                    throw new Exception();

                reply.Body = replyText;
                reply.UpdatedAt = DateTime.Now;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }
        public General_StatusWithData GetMediaReplies(string commentId, int itemsCount, int pageNumber, string userEmail)
        {
            var result = new General_StatusWithData();
            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == userEmail);
                if (user == null)
                    throw new Exception();

                var replies = (from tblr in _db.MediaCommentReply.Where(c => c.CommentId == commentId)
                               join tblu in _db.User
                               on tblr.ReplierId equals tblu.Id
                               select new
                               {
                                   CommentId = tblr.CommentId,
                                   ReplyId = tblr.Id,
                                   ReplierName = tblu.Name,
                                   ReplyText = tblr.Body,
                                   ReplyCreationTime = tblr.CreatedAt,
                                   IsCurrenctUser = (tblu.Id == user.Id)
                               })
                               .OrderByDescending(oo => oo.ReplyCreationTime)
                               .Skip(pageNumber * itemsCount)
                               .Take(itemsCount)
                               .ToList();

                result.Data = replies;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }


        public General_Status CreateMedia(Media mediaObj)
        {
            var result = new General_Status();
            try
            {
                _db.Media.Add(mediaObj);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = General_Strings.APIIssueMessage;
            }

            return result;

        }
        public General_Status RemoveMedia(string mediaId)
        {
            throw new NotImplementedException();
        }
        public General_StatusWithData SearchMedia(string mediaName)
        {
            throw new NotImplementedException();
        }
        public General_Status UpdateMedia()
        {
            throw new NotImplementedException();
        }
        public string GetMediaDirectoryPath(int mediaCategory)
        {
            var path = "";
            try
            {

                switch (mediaCategory)
                {
                    case (int)MediaCategories.Series:
                        path = _configuration.GetValue<string>("MediaPaths:SeriesPath");
                        break;
                    case (int)MediaCategories.Cartoons:
                        path = _configuration.GetValue<string>("MediaPaths:CartoonsPath");
                        break;
                    case (int)MediaCategories.Movies:
                        path = _configuration.GetValue<string>("MediaPaths:MoviesPath");
                        break;
                    case (int)MediaCategories.Sports:
                        path = _configuration.GetValue<string>("MediaPaths:SportsPath");
                        break;
                    case (int)MediaCategories.Songs:
                        path = _configuration.GetValue<string>("MediaPaths:SongsPath");
                        break;

                    default:
                        path = "";
                        break;
                }
            }
            catch (Exception)
            {

            }

            return path;
        }
        public General_StatusWithData GetCategoryMedia(int mediaCategory, int periodType)
        {
            throw new NotImplementedException();
        }


        public General_Status CreateDummyMedia(string title, string coverSource, string mediaSource, string description, MediaTypes typeId, MediaCategories categoryId, string userEmail)
        {
            var result = new General_Status();
            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == userEmail);
                if (user == null)
                    throw new Exception();

                var newMedia = new Media();
                newMedia.Id = Guid.NewGuid().ToString();
                newMedia.CategoryId = (int)categoryId;
                newMedia.CoverSource = coverSource;
                newMedia.CreatedAt = DateTime.Now;
                newMedia.Description = description;
                newMedia.IsActive = true;
                newMedia.PublisherId = user.Id;
                newMedia.Source = mediaSource;
                newMedia.Title = title;
                newMedia.Type = (int)typeId;

                _db.Media.Add(newMedia);
                _db.SaveChanges();

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }





        //public General_StatusWithData GetCategoryMedia(int mediaCategory, int periodType)
        //{
        //    var result = new General_StatusWithData();
        //    try
        //    {
        //        Func<MediaViews, bool> topDailyExpression = z => z.ViewTime > DateTime.Today && z.ViewTime < DateTime.Today.AddDays(1);
        //        Func<MediaViews, bool> topMonthlyExpression = z => z.ViewTime.Month == DateTime.Today.Month;
        //        Func<MediaViews, bool> topYesrlyExpression = z => z.ViewTime.Year == DateTime.Today.Year;
        //        Func<MediaViews, bool> periodExpression = z => true;
        //        switch (periodType)
        //        {
        //            case ((int)PeriodTypes.Daily):
        //                periodExpression = topDailyExpression;
        //                break;
        //            case ((int)PeriodTypes.Monthly):
        //                periodExpression = topMonthlyExpression;
        //                break;
        //            case ((int)PeriodTypes.Yearly):
        //                periodExpression = topYesrlyExpression;
        //                break;
        //        }

        //        var media = (from tblmedia in _db.Media.Where(z => z.CategoryId == mediaCategory).ToList()
        //                     join tblviews in _db.MediaViews.Where(periodExpression).ToList()
        //                     on tblmedia.Id equals tblviews.MediaId
        //                     into tblVout
        //                     from tblviews in tblVout.DefaultIfEmpty()
        //                     select new
        //                     {
        //                         MediaId = tblmedia.Id,
        //                         MediaName = tblmedia.FriendlyName,
        //                         ViewId = tblviews.Id
        //                     })
        //                     .GroupBy(z => z.MediaId)
        //                     .Select(z => new { z.Key, ViewsCount = z.Count() })
        //                     //.Take(10)
        //                     .OrderByDescending(z => z.ViewsCount)
        //                     .Select(z => new
        //                     {
        //                         Id = z.Key,
        //                         MediaData = _db.Media.FirstOrDefault(x => x.Id == z.Key)
        //                     })
        //                     .Select(z => new MediaResult
        //                     {
        //                         Id = z.Id,
        //                         Name = z.MediaData == null ? "Error" : z.MediaData.FriendlyName,
        //                         Source = z.MediaData == null ? "Error" : z.MediaData.SourcePath
        //                     })
        //                     .ToList();
        //        result.Data = media;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = false;
        //        result.ErrorMessage = General_Strings.APIIssueMessage;
        //    }

        //    return result;
        //}
    }
}










//(from tblm in _db.Media.Where(z => z.CategoryId == mediaItem.CategoryId)
// join tblv in _db.MediaViews
// on tblm.Id equals tblv.MediaId

// select new
// {
//     tblm.Id,
//     tblm.Title,
//     tblm.CoverSource,
//     tblm.Description,
//     ViewedAt = tblv.CreatedAt,
// }
//                             )