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
        public IActionResult Register([FromBody] UserParams userParams)
        {
            var result = new General_Result();
            try
            {

                if (string.IsNullOrWhiteSpace(userParams.name))
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.FirstName), "Invisible Name? works fine... just not here!");
                }
                if (string.IsNullOrWhiteSpace(userParams.email) || !userParams.email.Contains("@") || !userParams.email.Contains(".") || userParams.email.Length < 5)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Email), "Invalid Email!");
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
                var accountExistance = _accountBO.GetUserBy(userParams.email);
                if (accountExistance.IsSuccess)
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
        [Route("GetAccess")]
        [AllowAnonymous]
        public IActionResult GetAccess(string email, string password)
        {
            var result = new General_ResultWithData();

            try
            {
                if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains(".") || email.Length < 5)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Email), "Wrong Email!");
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Password), "Wrong Password!");
                }

                if (result.HasError)
                    return Ok(result);

                var authentiateUserStatus = _accountBO.GetAccess(email, password);
                if (!authentiateUserStatus.IsSuccess)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(ErrorKeyTypes.AuthenticatingError.ToString(), authentiateUserStatus.ErrorMessage);
                    return Ok(result);
                }

                result.Data = authentiateUserStatus.Data;
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
        [AllowAnonymous]
        public IActionResult ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var result = new General_Result();
            try
            {
                var existingUser = _accountBO.GetUserBy("email from token", model.oldPassword);
                if (!existingUser.IsSuccess)
                {
                    var passwordIssues = "Invalid Old Password!";
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Password), passwordIssues);
                }
                if (string.IsNullOrWhiteSpace(model.newPassword) || model.newPassword.Length < 5 || !StaticHelpers.validateSpecialChars(model.newPassword))
                {
                    var passwordIssues = "Password must be 5 minimum length with specials characters!";
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Password), passwordIssues);
                }

                if (!result.HasError)
                {
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

        [HttpPost]
        [Route("DisableAccount")]
        [AllowAnonymous]
        public IActionResult DisableAccount([FromBody] string password)
        {
            var result = new General_Result();
            try
            {
                var userExistance = _accountBO.GetUserBy("email from token", password);
                if (!userExistance.IsSuccess)
                {
                    var passwordIssues = "Inconsistant Credintials!";
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Password), passwordIssues);
                }

                if (!result.HasError)
                {
                    var disableAccountStatus = _accountBO.DisableAccount("email from token");
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