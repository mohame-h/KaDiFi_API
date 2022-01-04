using KaDiFi.BOs.IBO;
using KaDiFi.Entities;
using KaDiFi.Helpers;
using KaDiFi.Helpers.IHelper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

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

        [HttpPost]
        [Route("AddMedia")]
        public async Task<IActionResult> AddMediaAsync(int mediaType, int mediaCategory, string mediaDescription)
        {

            var result = new General_Result();

            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var mediaDirectoryPath = _mediaBO.GetMediaDirectoryPath(mediaType);
                    var folderExistance = Directory.Exists(mediaDirectoryPath);
                    if (!folderExistance)
                        System.IO.Directory.CreateDirectory(mediaDirectoryPath);

                    for (int i = 0; i < Request.Form.Files.Count; i++)
                    {
                        var mediaobj = new Media();
                        mediaobj.Id = Guid.NewGuid().ToString();
                        mediaobj.CategoryId = mediaCategory;
                        mediaobj.Description = mediaDescription;

                        mediaobj.FriendlyName = Request.Form.Files[i].FileName;
                        string mediaExtension = Path.GetExtension(Request.Form.Files[i].FileName);
                        mediaobj.MediaName = string.Join(string.Empty, (string.Join("_", mediaobj.Id, mediaobj.CategoryId)), mediaExtension);
                        mediaobj.SourcePath = Path.Combine(mediaDirectoryPath, mediaobj.MediaName.Replace(" ", ""));

                        mediaobj.PublisherId = "00000000-0000-0000-0000-000000000000"; //TODO: From Token
                        mediaobj.Type = mediaType;

                        var mediaCreationStatus = _mediaBO.CreateMedia(mediaobj);
                        if (!mediaCreationStatus.IsSuccess)
                        {
                            result.HasError = true;
                            result.ErrorsDictionary.Add(ErrorKeyTypes.SavingError.ToString(), mediaCreationStatus.ErrorMessage);
                            return Ok(result);
                        }

                        //Request.Form.Files[i].SaveAs(Server.mappath(filepath + "/" + filename.replace(" ", ""))); //file will be saved in path

                        using (var inputStream = new FileStream(mediaobj.SourcePath, FileMode.Create))
                        {
                            await Request.Form.Files[i].CopyToAsync(inputStream);
                            // stream to byte array
                            byte[] array = new byte[inputStream.Length];
                            inputStream.Seek(0, SeekOrigin.Begin);
                            inputStream.Read(array, 0, array.Length);

                            string fName = Request.Form.Files[i].FileName;
                        }
                    }
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
        [Route("HomeData")]
        public IActionResult GetHomeData()
        {

            var result = new General_ResultWithData();

            try
            {
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
        [Route("MediaCategory")]
        public IActionResult GetSpecificMediaCategory(int mediaCategory, int periodType)
        {

            var result = new General_ResultWithData();

            try
            {
                var categoryMedia = _mediaBO.GetCategoryMedia(mediaCategory, periodType);
                if (!categoryMedia.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.AuthenticatingError.ToString(), categoryMedia.ErrorMessage);
                    return Ok(result);
                }

                result.Data = categoryMedia.Data;

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }

            return Ok(result);
        }


    }
}
