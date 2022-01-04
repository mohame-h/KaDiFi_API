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
        public IActionResult Register(string firstName, string lastName, int age, string email, string password)
        {
            var result = new General_Result();
            try
            {

                if (string.IsNullOrWhiteSpace(firstName))
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.FirstName), "Invisible First Name? works fine... just not here!");
                }
                if (string.IsNullOrWhiteSpace(lastName))
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.LastName), "Invisible Last Name? works fine... just not here!");
                }
                if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains(".") || email.Length < 5)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Email), "Invalid Email!");
                }
                if (age < 12 || age > 100)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Age), "You are a mummy or a child!");
                }


                if (string.IsNullOrWhiteSpace(password) || password.Length < 5 || !StaticHelpers.validateSpecialChars(password))
                {
                    var passwordIssues = "Password must be 5 minimum length with specials characters!";
                    result.HasError = true;
                    result.ErrorsDictionary.Add(string.Join("_", ErrorKeyTypes.FormValidationError, FormFieldTypes.Password), passwordIssues);
                }

                if (!result.HasError)
                {
                    var user = new User()
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Age = age,
                        Email = email,
                        Password = password
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