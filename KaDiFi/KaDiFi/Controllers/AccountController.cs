using KaDiFi.BOs;
using KaDiFi.Entities;
using Microsoft.AspNetCore.Mvc;
using System;

namespace KaDiFi.Controllers
{
    public class AccountController : Controller
    {
        private AccountBO _accountBO;

        public AccountController(AccountBO accountBO)
        {
            _accountBO = accountBO;
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(User user)
        {
            var result = new General_Result();
            try
            {

                if (string.IsNullOrWhiteSpace(user.FirstName))
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add("firstName", "Invisible First Name? works fine... just not here!");
                }
                if (string.IsNullOrWhiteSpace(user.LastName))
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add("lastName", "Invisible Last Name? works fine... just not here!");
                }
                if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@") || !user.Email.Contains(".") || user.Email.Length < 5)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add("email", "Invalid Email!");
                }
                if (user.Age < 12 || user.Age > 100)
                {
                    result.HasError = true;
                    result.ErrorsDictionary.Add("age", "You are a mummy or a child!");
                }
                if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 5)
                {
                    var passwordIssues = "Invalid length";
                    var passwordChars = user.Password.ToCharArray();
                    int specialChars = 0;
                    foreach (var c in passwordChars)
                    {
                        if (char.IsLetterOrDigit(c))
                            specialChars++;
                    }

                    passwordIssues += specialChars == 0 ? "" : ", Need some special characters!";

                    result.HasError = true;
                    result.ErrorsDictionary.Add("password", passwordIssues);
                }

                if (!result.HasError)
                {
                    var registeringStatus = _accountBO.RegisterUser(user);
                    if (!registeringStatus.IsSuccess)
                    {
                        result.HasError = true;
                        result.ErrorsDictionary.Add("SavingError", registeringStatus.ErrorMessage);
                    }
                }
                return Ok(result);
            }
            catch (Exception)
            {
                result.HasError = true;
                result.ErrorsDictionary.Add("ServerError", "Sorry, An error Occured... Contact Admin for more help!");
                return Ok(result);
            }
        }


        //public IActionResult GetAccess()





    }
}