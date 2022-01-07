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
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var oldUserRegisterData = _db.User.Where(z => z.Email == newUser.Email && !z.IsActive).ToList();
                    var oldUserIds = oldUserRegisterData.Select(z => z.Id).ToList();
                    var oldUserActivations = _db.AccountActivation.Where(z => oldUserIds.Contains(z.UserId)).ToList();
                    if (oldUserRegisterData.Count > 0)
                    {
                        //_db.User.RemoveRange(oldUserRegisterData);
                        oldUserRegisterData.ForEach(z =>
                        {
                            z.IsActive = false;
                            z.DeletedAt = DateTime.Now;
                        });
                        if (oldUserActivations.Count > 0)
                        {
                            //_db.AccountActivation.RemoveRange(oldUserActivations);
                            oldUserActivations.ForEach(z =>
                            {
                                z.IsActive = false;
                                z.DeletedAt = DateTime.Now;
                            });
                        }
                        _db.SaveChanges();
                    }

                    newUser.Id = Guid.NewGuid().ToString();
                    newUser.Password = newUser.Password;
                    newUser.RoleId = (int)UserRoles.User;
                    newUser.CreatedAt = DateTime.Now;
                    newUser.IsActive = false;
                    _db.User.Add(newUser);

                    var activationObj = new AccountActivation();
                    activationObj.Id = Guid.NewGuid().ToString();
                    activationObj.Code = Guid.NewGuid().ToString();
                    activationObj.CreatedAt = DateTime.Now;
                    activationObj.UserId = newUser.Id;
                    _db.AccountActivation.Add(activationObj);
                    //TODO: Send Email staff;

                    _db.SaveChanges();
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.IsSuccess = false;
                    result.ErrorMessage = "Temp Server Error.";
                }
            }

            return result;
        }
        public General_StatusWithData GetAccess(string email, string password)
        {
            var result = new General_StatusWithData();

            try
            {
                var loginUser = _db.User.FirstOrDefault(z => z.Email == email && z.IsActive);
                if (loginUser == null)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "Wrong Email!";
                    return result;
                }
                else if (loginUser.Password != password)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "Invalid Credintials!";
                    return result;
                }

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
        public General_StatusWithData GetUserBy(string email)
        {
            var result = new General_StatusWithData();
            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == email && z.IsActive);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "No account with such Email exists!";
                    return result;
                }

                result.IsSuccess = true;
                result.Data = user;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }
        public General_StatusWithData GetUserBy(string email, string password)
        {
            var result = new General_StatusWithData();
            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == email && z.IsActive);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "No account with such Email exists!";
                    return result;
                }
                if (user.Password != password)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "Invalid Credintials!";
                    return result;
                }

                result.IsSuccess = true;
                result.Data = user;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }

        public General_Status ChangeUserPassword(string email, string newPassword)
        {
            var result = new General_Status();
            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == email && z.IsActive);
                user.Password = newPassword;
                user.UpdatedAt = DateTime.Now;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }

        public General_Status DisableNews(string email)
        {
            var result = new General_Status();
            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == email && z.IsActive);
                user.TermsFlag = false;
                user.UpdatedAt = DateTime.Now;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }
        public General_Status DisableAccount(string email)
        {
            var result = new General_Status();
            try
            {
                var user = _db.User.FirstOrDefault(z => z.Email == email && z.IsActive);
                user.IsActive = false;
                user.DeletedAt = DateTime.Now;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }

        public General_StatusWithData ActivateAccount(string activationCode)
        {
            var result = new General_StatusWithData();
            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    var activationObj = _db.AccountActivation.FirstOrDefault(z => z.Code == activationCode && z.IsActive && z.DeletedAt == null);
                    if (activationObj == null)
                    {
                        result.IsSuccess = false;
                        result.ErrorMessage = "Invalid Activation Code!";
                        return result;
                    }
                    activationObj.IsActive = false;
                    activationObj.UpdatedAt = DateTime.Now;

                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
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
