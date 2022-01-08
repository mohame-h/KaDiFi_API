using KaDiFi.BOs.IBO;
using KaDiFi.Entities;
using KaDiFi.Helpers;
using KaDiFi.Helpers.IHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace KaDiFi.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private IAccountBO _accountBO;
        private IAuthenticationHelper _auth;

        public AccountController(IAccountBO accountBO, IAuthenticationHelper auth)
        {
            _accountBO = accountBO;
            _auth = auth;
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] UserDTO userParams)
        {
            var result = new General_Result();
            try
            {

                if (string.IsNullOrWhiteSpace(userParams.name))
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.FirstName), "Invisible Name? works fine... just not here!");
                }
                if (userParams.age < 12 || userParams.age > 100)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Age), "Invalid Age!");
                }
                if (string.IsNullOrWhiteSpace(userParams.password) || userParams.password.Length < 5 || !StaticHelpers.validateSpecialChars(userParams.password))
                {
                    var passwordIssues = "Password must be 5 minimum length with specials characters!";
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Password), passwordIssues);
                }
                try
                {
                    var emailValidationInstance = new System.Net.Mail.MailAddress(userParams.email);
                }
                catch (Exception)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Email), "Invalid Email!");
                }

                var accountExistance = _accountBO.GetUserBy(userParams.email);
                if (accountExistance.IsSuccess && accountExistance.Data != null)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Email), "Account with same email exists, Sign in instead?");
                }

                if (!result.HasError)
                {
                    var user = new User()
                    {
                        Name = userParams.name,
                        Age = userParams.age,
                        Email = userParams.email,
                        Password = userParams.password,
                        TermsFlag = userParams.acceptTerms
                    };
                    var registeringStatus = _accountBO.RegisterUser(user);
                    if (!registeringStatus.IsSuccess)
                    {
                        result.HasError = true;
                        result.ErrorsDictionary.Add(ErrorKeyTypes.SavingError.ToString(), registeringStatus.ErrorMessage);
                    }
                }
                return Ok(result);
            }
            catch (Exception)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }
        }
        [HttpPost]
        [Route("VerifyAccount")]
        [AllowAnonymous]
        public IActionResult VerifyAccount(string verificationCode)
        {
            var result = new General_Result();
            try
            {
                if (string.IsNullOrEmpty(verificationCode))
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.VerificationCode), "Invalid Verification Code");
                }
                var verificationObjExistence = _accountBO.GetAccountVerificationBy(verificationCode);
                if (verificationObjExistence.IsSuccess && verificationObjExistence.Data == null)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.VerificationCode), "Invalid Verification Code");
                }

                if (!result.HasError)
                {
                    var registeringStatus = _accountBO.VerifyAccount(verificationCode);
                    if (!registeringStatus.IsSuccess)
                    {
                        result.HasError = true;
                        result.ErrorsDictionary.Add(ErrorKeyTypes.SavingError.ToString(), registeringStatus.ErrorMessage);
                    }
                }
                return Ok(result);
            }
            catch (Exception)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }
        }
        [HttpPost]
        [Route("ForgetPassword")]
        [AllowAnonymous]
        public IActionResult ForgetPassword(string email)
        {
            var result = new General_Result();
            try
            {
                try
                {
                    var emailValidationInstance = new System.Net.Mail.MailAddress(email);
                }
                catch (Exception)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Email), "Invalid Email!");
                }
                var userObj = _accountBO.GetUserBy(email);
                if (userObj.IsSuccess && userObj.Data == null)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.VerificationCode), "Invalid Email");
                }

                if (!result.HasError)
                {
                    var user = (User)userObj.Data;

                    var sendMailStatus= StaticHelpers.sendEmail(user.Email, "Forgot Password?", user.Password);
                    if (!sendMailStatus)
                    {
                        result.HasError = true;
                        result.ErrorsDictionary.Add(ErrorKeyTypes.SavingError.ToString(), General_Strings.APITempIssueMessage);
                    }
                }
                return Ok(result);
            }
            catch (Exception)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }
        }
        [HttpPost]
        [Route("GetAccess")]
        [AllowAnonymous]
        public IActionResult GetAccess([FromBody] LoginDTO model)
        {
            var result = new General_ResultWithData();

            try
            {
                try
                {
                    var emailValidationInstance = new System.Net.Mail.MailAddress(model.email);
                }
                catch (Exception)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Email), "Invalid Email!");
                }
                if (string.IsNullOrWhiteSpace(model.password))
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Password), "Invalid Credintials!");
                }

                if (!result.HasError)
                {
                    var authentiateUserStatus = _accountBO.GetAccess(model.email, model.password);
                    if ((authentiateUserStatus.IsSuccess && authentiateUserStatus.Data == null) || !authentiateUserStatus.IsSuccess)
                    {
                        result.HasError = true;
                        result.ErrorsDictionary.Add(ErrorKeyTypes.AuthenticatingError.ToString(), authentiateUserStatus.ErrorMessage);
                        return Ok(result);
                    }

                    result.Data = authentiateUserStatus.Data;
                }
            }
            catch (Exception)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
            }

            return Ok(result);
        }
        [HttpPost]
        [Route("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var result = new General_Result();
            try
            {
                if (string.IsNullOrWhiteSpace(model.newPassword) || model.newPassword.Length < 5 || !StaticHelpers.validateSpecialChars(model.newPassword))
                {
                    var passwordIssues = "Password must be 5 minimum length with specials characters!";
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Password), passwordIssues);
                }
                //TODO: Get token credintials
                var userObj = _accountBO.GetUserBy("email from token", model.oldPassword);
                if (userObj.IsSuccess && userObj.Data == null)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Password), "Invalid Credintials!");
                }

                if (!result.HasError)
                {
                    //TODO: 
                    var changePasswordStatus = _accountBO.ChangeUserPassword("email from token", model.newPassword);
                    if (!changePasswordStatus.IsSuccess)
                    {
                        result.HasError = true;
                        result.ErrorsDictionary.Add(ErrorKeyTypes.SavingError.ToString(), changePasswordStatus.ErrorMessage);
                    }
                }
                return Ok(result);
            }
            catch (Exception)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }
        }
        [HttpPost]
        [Route("FreezeAccount")]
        public IActionResult FreezeAccount([FromBody] FreezeAccountDTO model)
        {
            var result = new General_Result();
            try
            {
                if (string.IsNullOrWhiteSpace(model.password))
                {
                    var passwordIssues = "Invalid Credintials!";
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Password), passwordIssues);
                }
                if (model.daysCount < 1)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.FreezingPeriod), "Invalid Freezing period!");
                }

                //TODO: Get token credintials
                var userObj = _accountBO.GetUserBy("email from token", model.password);
                if (userObj.IsSuccess && userObj.Data == null)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Password), "Invalid Credintials!");
                }

                if (!result.HasError)
                {
                    var disableAccountStatus = _accountBO.DisableAccount("email from token", model.daysCount);
                    if (!disableAccountStatus.IsSuccess)
                    {
                        result.HasError = true;
                        result.ErrorsDictionary.Add(ErrorKeyTypes.SavingError.ToString(), disableAccountStatus.ErrorMessage);
                    }
                }
                return Ok(result);
            }
            catch (Exception)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }
        }
       
      

        [HttpPost]
        [Route("DisableNews")]
        [AllowAnonymous]
        public IActionResult DisableNews()
        {
            var result = new General_Result();
            try
            {
                var userExistance = _accountBO.GetUserBy("email from token");
                if (!userExistance.IsSuccess)
                {
                    var passwordIssues = "Inconsistant Credintials!";
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Email), passwordIssues);
                }

                if (!result.HasError)
                {
                    var disableNewsStatus = _accountBO.DisableNews("email from token");
                    if (!disableNewsStatus.IsSuccess)
                    {
                        result.HasError = true;
                        result.ErrorsDictionary.Add(ErrorKeyTypes.SavingError.ToString(), disableNewsStatus.ErrorMessage);
                    }
                }
                return Ok(result);
            }
            catch (Exception)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add(ErrorKeyTypes.ServerError.ToString(), General_Strings.APIIssueMessage);
                return Ok(result);
            }
        }

        


        //[HttpPost]
        //[Route("UpdateProfile")]
        //[AllowAnonymous]
        //public IActionResult UpdateAccount(string firstName, string lastName, int age, string email, string password)
        //{




        //}



        //[HttpGet]
        //[Route("GetAAA")]
        ////[Authorize]

        //public IActionResult AAA()
        //{
        //    var result = new General_Result();

        //    try
        //    {
        //        var validateTokenStatus = _auth.ValidateToken(HttpContext.User);
        //        if (!validateTokenStatus.IsSuccess)
        //        {
        //            result.HasError = true;
        //            result.ErrorsDictionary.Add(ErrorKeyTypes.TokenError.ToString(), validateTokenStatus.ErrorMessage);
        //            return Ok(result);
        //        }

        //        return Ok("BlaBlaaaa");


        //    }
        //    catch (Exception)
        //    {

        //    }


        //    return Ok();
        //}

    }
}