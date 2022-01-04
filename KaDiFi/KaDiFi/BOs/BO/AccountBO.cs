using KaDiFi.BOs.IBO;
using KaDiFi.Entities;
using KaDiFi.Helpers;
using KaDiFi.Helpers.IHelper;
using System;
using System.Linq;

namespace KaDiFi.BOs
{
    public class AccountBO : IAccountBO
    {
        private KaDifiEntities _db;
        private IAuthenticationHelper _auth;

        public AccountBO(KaDifiEntities db, IAuthenticationHelper auth)
        {
            _db = db;
            _auth = auth;
        }

        public General_Status RegisterUser(User newUser)
        {
            var result = new General_Status();
            try
            {
                newUser.Id = Guid.NewGuid().ToString();
                newUser.Password = newUser.Password;
                newUser.RoleId = (int)UserRoles.User;
                _db.User.Add(newUser);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }
        public General_StatusWithData GetAccess(string email, string password)
        {
            var result = new General_StatusWithData();

            try
            {
                var loginUser = _db.User.FirstOrDefault(z => z.Email == email);
                //var userExist = _db.User.Any(z => z.Email == email && z.Password == password);
                if (loginUser == null)
                    result.ErrorMessage = "Invalid User Credintials!";

                if (loginUser.Password != password)
                    result.ErrorMessage = "Invalid User Credintials!";

                var token = _auth.GenerateJSONWebToken(email);
                if (string.IsNullOrWhiteSpace(token))
                {
                    result.ErrorMessage = General_Strings.APIIssueMessage;
                    return result;
                }

                result.Data = token;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = General_Strings.APIIssueMessage;
            }

            return result;
        }



        //public General_Status RegisterUser(User newUser)
        //{
        //    var result = new General_Status();
        //    try
        //    {
        //        newUser.Id = Guid.NewGuid().ToString();
        //        newUser.Password = newUser.Password.Mask();
        //        newUser.RoleId = (int)UserRoles.User;
        //        _db.User.Add(newUser);
        //        _db.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = false;
        //        result.ErrorMessage = "Temp Server Error.";
        //    }

        //    return result;
        //}
        //public General_StatusWithData GetAccess(string email, string password)
        //{
        //    var result = new General_StatusWithData();

        //    try
        //    {
        //        var loginUser = _db.User.FirstOrDefault(z => z.Email == email);
        //        //var userExist = _db.User.Any(z => z.Email == email && z.Password == password);
        //        if (loginUser == null)
        //            result.ErrorMessage = "Invalid User Credintials!";

        //        var passwordUnmasked = loginUser.Password.UnMask();
        //        if (passwordUnmasked != password)
        //            result.ErrorMessage = "Invalid User Credintials!";

        //        var token = _auth.GenerateJSONWebToken(email);
        //        if (string.IsNullOrWhiteSpace(token))
        //        {
        //            result.ErrorMessage = General_Strings.APIIssueMessage;
        //            return result;
        //        }

        //        result.Data = token;
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
