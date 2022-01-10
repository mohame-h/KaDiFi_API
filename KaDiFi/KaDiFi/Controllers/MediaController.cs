using KaDiFi.BOs.IBO;
using KaDiFi.Entities;
using KaDiFi.Helpers;
using KaDiFi.Helpers.IHelper;
using Microsoft.AspNetCore.Mvc;
using System;

namespace KaDiFi.Controllers
{
    [Route("api/[controller]")]
    public class MediaController : Controller
    {
        private IMediaBO _mediaBO;
        private IAuthenticationHelper _auth;

        public MediaController(IMediaBO mediaBO, IAuthenticationHelper auth)
        {
            _mediaBO = mediaBO;
            _auth = auth;
        }

        [HttpGet]
        [Route("HomeData")]
        public IActionResult GetHomeData()
        {

            var result = new General_ResultWithData();

            try
            {
                var claims = HttpContext.Request.HttpContext.User.Claims;
                var authenticationStatus = _auth.ValidateToken(claims);
                if (!authenticationStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.TokenError.ToString(), authenticationStatus.ErrorMessage);
                    return Ok(result);
                }

                var homeMediaDataStatus = _mediaBO.GetHomeMedia();
                if (!homeMediaDataStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.AuthenticatingError.ToString(), homeMediaDataStatus.ErrorMessage);
                    return Ok(result);
                }

                result.Data = homeMediaDataStatus.Data;

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetSpecificMedia")]
        public IActionResult GetSpecificMedia(GetMediaDTO model)
        {

            var result = new General_ResultWithData();

            try
            {
                var claims = HttpContext.Request.HttpContext.User.Claims;
                var authenticationStatus = _auth.ValidateToken(claims);
                if (!authenticationStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.TokenError.ToString(), authenticationStatus.ErrorMessage);
                    return Ok(result);
                }

                var specificMediaStatus = _mediaBO.GetSpecificMedia(model.mediaId, authenticationStatus.Data, model.commentsTotalCount, model.repliesTotalCount);
                if (!specificMediaStatus.IsSuccess || (specificMediaStatus.IsSuccess && specificMediaStatus.Data == null))
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.AuthenticatingError.ToString(), specificMediaStatus.ErrorMessage);
                    return Ok(result);
                }

                result.Data = specificMediaStatus.Data;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("AddOrRemoveMediaReact")]
        public IActionResult AddOrRemoveMediaReact([FromBody] AddOrRemoveReactDTO model)
        {

            var result = new General_Result();

            try
            {
                var claims = HttpContext.Request.HttpContext.User.Claims;
                var authenticationStatus = _auth.ValidateToken(claims);
                if (!authenticationStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.TokenError.ToString(), authenticationStatus.ErrorMessage);
                    return Ok(result);
                }

                var addOrRemoveStatus = _mediaBO.addOrRemoveMediaReact(model.mediaId, model.reactTypeId, authenticationStatus.Data);
                if (!addOrRemoveStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), addOrRemoveStatus.ErrorMessage);
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("AddMediaComment")]
        public IActionResult AddMediaComment([FromBody] AddMediaCommentDTO model)
        {
            var result = new General_Result();

            try
            {
                var claims = HttpContext.Request.HttpContext.User.Claims;
                var authenticationStatus = _auth.ValidateToken(claims);
                if (!authenticationStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.TokenError.ToString(), authenticationStatus.ErrorMessage);
                    return Ok(result);
                }

                var addCommentStatus = _mediaBO.AddComment(model.mediaId, model.commentText, authenticationStatus.Data);
                if (!addCommentStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), addCommentStatus.ErrorMessage);
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("EditMediaComment")]
        public IActionResult EditMediaComment([FromBody] EditMediaCommentDTO model)
        {
            var result = new General_Result();

            try
            {
                var claims = HttpContext.Request.HttpContext.User.Claims;
                var authenticationStatus = _auth.ValidateToken(claims);
                if (!authenticationStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.TokenError.ToString(), authenticationStatus.ErrorMessage);
                    return Ok(result);
                }

                var addOrRemoveStatus = _mediaBO.EditComment(model.commentId, model.commentText, authenticationStatus.Data);
                if (!addOrRemoveStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), addOrRemoveStatus.ErrorMessage);
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetMediaComments")]
        public IActionResult GetMediaComments([FromBody] GetMediaCommentsDTO model)
        {
            var result = new General_ResultWithData();

            try
            {
                var claims = HttpContext.Request.HttpContext.User.Claims;
                var authenticationStatus = _auth.ValidateToken(claims);
                if (!authenticationStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.TokenError.ToString(), authenticationStatus.ErrorMessage);
                    return Ok(result);
                }

                if (string.IsNullOrWhiteSpace(model.mediaId))
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.ParamError, FormFieldTypes.MediaId), "Invalid media, Please refresh!");
                }
                model.itemsCount = model.itemsCount == 0 ? 10 : model.itemsCount;
                model.pageNumber = model.pageNumber == 0 ? 1 : model.pageNumber;

                var getMediaCommentsStatus = _mediaBO.GetMediaComments(model.mediaId, model.itemsCount, model.pageNumber, authenticationStatus.Data);
                if (!getMediaCommentsStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), getMediaCommentsStatus.ErrorMessage);
                    return Ok(result);
                }

                result.Data = getMediaCommentsStatus.Data;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("AddMediaReply")]
        public IActionResult AddMediaReply([FromBody] AddMediaReplyDTO model)
        {
            var result = new General_Result();

            try
            {
                var claims = HttpContext.Request.HttpContext.User.Claims;
                var authenticationStatus = _auth.ValidateToken(claims);
                if (!authenticationStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.TokenError.ToString(), authenticationStatus.ErrorMessage);
                    return Ok(result);
                }

                var addReplyStatus = _mediaBO.AddReply(model.commentId, model.replyText, authenticationStatus.Data);
                if (!addReplyStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), addReplyStatus.ErrorMessage);
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("EditMediaReply")]
        public IActionResult EditMediaReply([FromBody] EditTextDTO model)
        {
            var result = new General_Result();

            try
            {
                var claims = HttpContext.Request.HttpContext.User.Claims;
                var authenticationStatus = _auth.ValidateToken(claims);
                if (!authenticationStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.TokenError.ToString(), authenticationStatus.ErrorMessage);
                    return Ok(result);
                }

                var editReplyStatus = _mediaBO.EditReply(model.id, model.text, authenticationStatus.Data);
                if (!editReplyStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), editReplyStatus.ErrorMessage);
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetMediaReplies")]
        public IActionResult GetMediaReplies([FromBody] GetMediaRepliesDTO model)
        {
            var result = new General_ResultWithData();

            try
            {
                var claims = HttpContext.Request.HttpContext.User.Claims;
                var authenticationStatus = _auth.ValidateToken(claims);
                if (!authenticationStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.TokenError.ToString(), authenticationStatus.ErrorMessage);
                    return Ok(result);
                }

                if (string.IsNullOrWhiteSpace(model.commentId))
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.ParamError, FormFieldTypes.MediaId), "issue with comment, Please refresh the page!");
                }
                model.itemsCount = model.itemsCount == 0 ? 10 : model.itemsCount;
                model.pageNumber = model.pageNumber == 0 ? 1 : model.pageNumber;

                var getMediaRepliesStatus = _mediaBO.GetMediaReplies(model.commentId, model.itemsCount, model.pageNumber, authenticationStatus.Data);
                if (!getMediaRepliesStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), getMediaRepliesStatus.ErrorMessage);
                    return Ok(result);
                }

                result.Data = getMediaRepliesStatus.Data;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }

            return Ok(result);
        }





        //[HttpGet]
        //[Route("MediaCategory")]
        //public IActionResult GetSpecificMediaCategory(int mediaCategory, int periodType)
        //{

        //    var result = new General_ResultWithData();

        //    try
        //    {
        //        var categoryMedia = _mediaBO.GetCategoryMedia(mediaCategory, periodType);
        //        if (!categoryMedia.IsSuccess)
        //        {
        //            result.HasError = true;
        //            result.ErrorsDictionary.Add(ErrorKeyTypes.AuthenticatingError.ToString(), categoryMedia.ErrorMessage);
        //            return Ok(result);
        //        }

        //        result.Data = categoryMedia.Data;

        //    }
        //    catch (Exception ex)
        //    {
        //        result.HasError = true;
        //        result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
        //        return Ok(result);
        //    }

        //    return Ok(result);
        //}







        //[HttpPost]
        //[Route("AddComment")]
        //public IActionResult AddComment(MediaCommentDTO model)
        //{

        //    var result = new General_ResultWithData();

        //    try
        //    {
        //        var mediaExistance = _mediaBO.GetSpecificMedia(model.mediaId);
        //        if (!mediaExistance.IsSuccess)
        //        {
        //            result.HasError = true;
        //            result.ErrorsDictionary.Add(ErrorKeyTypes.AuthenticatingError.ToString(), mediaExistance.ErrorMessage);
        //            return Ok(result);
        //        }

        //        //TODO: model.userId = UserId from token

        //        if (!result.HasError)
        //        {
        //            var AddCommentStatus = _mediaBO.AddComment(model);
        //            if (!AddCommentStatus.IsSuccess)
        //            {
        //                result.HasError = true;
        //                result.ErrorsDictionary.Add(ErrorKeyTypes.SavingError.ToString(), AddCommentStatus.ErrorMessage);
        //            }
        //            else
        //            {
        //                result.Data = AddCommentStatus.Data;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        result.HasError = true;
        //        result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
        //        return Ok(result);
        //    }

        //    return Ok(result);
        //}


        //[HttpPost]
        //[Route("AddMedia")]
        //public async Task<IActionResult> AddMediaAsync(int mediaType, int mediaCategory, string mediaDescription)
        //{

        //    var result = new General_Result();

        //    try
        //    {
        //        if (Request.Form.Files.Count > 0)
        //        {
        //            var mediaDirectoryPath = _mediaBO.GetMediaDirectoryPath(mediaType);
        //            var folderExistance = Directory.Exists(mediaDirectoryPath);
        //            if (!folderExistance)
        //                System.IO.Directory.CreateDirectory(mediaDirectoryPath);

        //            for (int i = 0; i < Request.Form.Files.Count; i++)
        //            {
        //                var mediaobj = new Media();
        //                mediaobj.Id = Guid.NewGuid().ToString();
        //                mediaobj.CategoryId = mediaCategory;
        //                mediaobj.Description = mediaDescription;

        //                mediaobj.FriendlyName = Request.Form.Files[i].FileName;
        //                string mediaExtension = Path.GetExtension(Request.Form.Files[i].FileName);
        //                mediaobj.MediaName = string.Join(string.Empty, (string.Join("_", mediaobj.Id, mediaobj.CategoryId)), mediaExtension);
        //                mediaobj.SourcePath = Path.Combine(mediaDirectoryPath, mediaobj.MediaName.Replace(" ", ""));

        //                mediaobj.PublisherId = "00000000-0000-0000-0000-000000000000"; //TODO: From Token
        //                mediaobj.Type = mediaType;

        //                var mediaCreationStatus = _mediaBO.CreateMedia(mediaobj);
        //                if (!mediaCreationStatus.IsSuccess)
        //                {
        //                    result.HasError = true;
        //                    result.ErrorsDictionary.Add(ErrorKeyTypes.SavingError.ToString(), mediaCreationStatus.ErrorMessage);
        //                    return Ok(result);
        //                }

        //                //Request.Form.Files[i].SaveAs(Server.mappath(filepath + "/" + filename.replace(" ", ""))); //file will be saved in path

        //                using (var inputStream = new FileStream(mediaobj.SourcePath, FileMode.Create))
        //                {
        //                    await Request.Form.Files[i].CopyToAsync(inputStream);
        //                    // stream to byte array
        //                    byte[] array = new byte[inputStream.Length];
        //                    inputStream.Seek(0, SeekOrigin.Begin);
        //                    inputStream.Read(array, 0, array.Length);

        //                    string fName = Request.Form.Files[i].FileName;
        //                }
        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        result.HasError = true;
        //        result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
        //        return Ok(result);
        //    }

        //    return Ok(result);
        //}



    }
}
