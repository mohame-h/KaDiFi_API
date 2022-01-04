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

        public General_StatusWithData GetHomeMedia()
        {
            var result = new General_StatusWithData();

            try
            {
                var homeMedia = new Dictionary<string, List<MediaResult>>();

                var recent = (from tblm in _db.Media
                              join tblv in _db.MediaViews.OrderByDescending(z => z.ViewTime)
                              on tblm.Id equals tblv.MediaId
                              select new MediaResult
                              {
                                  Id = tblm.Id,
                                  Name = tblm.FriendlyName,
                                  Source = tblm.SourcePath
                              }
                               )
                              .Take(5)
                              .ToList();
                homeMedia.Add("Recent", recent);

                var recommended = _db.Media.Where(z => z.Likes > (z.DisLikes == 0 ? 0 : z.DisLikes / 3))
                                    .Take(5)
                                    .Select(z => new MediaResult { Id = z.Id, Name = z.FriendlyName, Source = z.SourcePath })
                                    .ToList();
                homeMedia.Add("Recommended", recommended);

                var cartoons = _db.Media.Where(z => z.CategoryId == (int)MediaCategories.Cartoons).Take(5).Select(z => new MediaResult { Id = z.Id, Name = z.FriendlyName, Source = z.SourcePath }).ToList();
                homeMedia.Add(MediaCategories.Cartoons.ToString(), cartoons);

                var sports = _db.Media.Where(z => z.CategoryId == (int)MediaCategories.Sports).Take(5).Select(z => new MediaResult { Id = z.Id, Name = z.FriendlyName, Source = z.SourcePath }).ToList();
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

        public General_StatusWithData GetSpecificMedia(string mediaId)
        {
            throw new NotImplementedException();
        }
        public General_StatusWithData GetCategoryMedia(int mediaCategory, int periodType)
        {
            var result = new General_StatusWithData();
            try
            {
                Func<MediaViews, bool> topDailyExpression = z => z.ViewTime > DateTime.Today && z.ViewTime < DateTime.Today.AddDays(1);
                Func<MediaViews, bool> topMonthlyExpression = z => z.ViewTime.Month == DateTime.Today.Month;
                Func<MediaViews, bool> topYesrlyExpression = z => z.ViewTime.Year == DateTime.Today.Year;
                Func<MediaViews, bool> periodExpression = z => true;
                switch (periodType)
                {
                    case ((int)PeriodTypes.Daily):
                        periodExpression = topDailyExpression;
                        break;
                    case ((int)PeriodTypes.Monthly):
                        periodExpression = topMonthlyExpression;
                        break;
                    case ((int)PeriodTypes.Yearly):
                        periodExpression = topYesrlyExpression;
                        break;
                }

                var mediaArrange = (from tblmedia in _db.Media.Where(z => z.CategoryId == mediaCategory)
                                    join tblviews in _db.MediaViews.Where(periodExpression)
                                    on tblmedia.Id equals tblviews.MediaId
                                    select new
                                    {
                                        MediaId = tblmedia.Id,
                                        MediaName = tblmedia.FriendlyName,
                                        ViewId = tblviews.Id
                                    })
                             .GroupBy(z => z.MediaId)
                             .Select(z => new { z.Key, ViewsCount = z.Count() })
                             //.Take(10)
                             .OrderByDescending(z => z.ViewsCount)
                             .Select(z => new
                             {
                                 Id = z.Key,
                                 MediaData = _db.Media.FirstOrDefault(x => x.Id == z.Key)
                             })
                             .Select(z => new MediaResult
                             {
                                 Id = z.Id,
                                 Name = z.MediaData == null ? "Error" : z.MediaData.FriendlyName,
                                 Source = z.MediaData == null ? "Error" : z.MediaData.SourcePath
                             })
                             .ToList();


                var categoryMedia = new Dictionary<string, List<MediaResult>>();


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
    }
}
