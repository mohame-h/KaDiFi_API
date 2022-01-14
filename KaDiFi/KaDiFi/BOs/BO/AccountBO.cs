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
                    var nonActivatedUsers = (from tbld in _db.AccountDeactivation
                                             join tblu in _db.User
                                             on tbld.UserId equals tblu.Id
                                             select tblu.Id)
                                             .ToList();

                    var oldUserRegisterData = _db.User.Where(z => z.Email == newUser.Email && !z.IsActive && !nonActivatedUsers.Contains(z.Id)).ToList();
                    if (oldUserRegisterData.Count > 0)
                    {
                        var oldUserIds = oldUserRegisterData.Select(z => z.Id).ToList();
                        var oldUserActivations = _db.AccountVerification.Where(z => oldUserIds.Contains(z.UserId)).ToList();
                        oldUserRegisterData.ForEach(z =>
                        {
                            z.IsActive = false;
                            z.DeletedAt = DateTime.Now;
                        });

                        oldUserActivations.ForEach(z =>
                        {
                            z.IsActive = false;
                            z.DeletedAt = DateTime.Now;
                        });
                    }

                    newUser.Id = Guid.NewGuid().ToString();
                    newUser.Password = newUser.Password;
                    newUser.RoleId = (int)UserRoles.User;
                    newUser.CreatedAt = DateTime.Now;
                    newUser.IsActive = false;
                    _db.User.Add(newUser);

                    var verificationObj = new AccountVerification();
                    verificationObj.Id = Guid.NewGuid().ToString();
                    verificationObj.Code = Guid.NewGuid().ToString();
                    verificationObj.CreatedAt = DateTime.Now;
                    verificationObj.UserId = newUser.Id;
                    _db.AccountVerification.Add(verificationObj);

                    _db.SaveChanges();

                    var sendMailStatus = StaticHelpers.sendEmail(newUser.Email, "Account Verification", verificationObj.Code);
                    if (!sendMailStatus)
                        throw new Exception();

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
        public General_Status VerifyAccount(string verificationCode)
        {
            var result = new General_Status();
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var verificationObj = _db.AccountVerification.FirstOrDefault(z => z.IsActive && z.Code == verificationCode && z.DeletedAt == null);
                    verificationObj.IsActive = false;
                    verificationObj.DeletedAt = DateTime.Now;

                    var userObj = _db.User.FirstOrDefault(z => z.Id == verificationObj.UserId);
                    userObj.IsActive = true;
                    userObj.UpdatedAt = DateTime.Now;

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
        public General_Status DisableAccount(string email, int freezingDaysCount)
        {
            var result = new General_Status();
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var user = _db.User.FirstOrDefault(z => z.Email == email && z.IsActive);
                    user.IsActive = false;
                    user.DeletedAt = DateTime.Now;

                    var deactivationObj = new AccountDeactivation();
                    deactivationObj.Id = Guid.NewGuid().ToString();
                    deactivationObj.UserId = user.Id;
                    deactivationObj.CreatedAt = DateTime.Now;
                    deactivationObj.Until = DateTime.Now.AddDays(freezingDaysCount);
                    _db.AccountDeactivation.Add(deactivationObj);

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
                var loginUser = _db.User.FirstOrDefault(z => z.Email == email && z.Password == password); //&& z.IsActive
                if (loginUser == null)
                {
                    result.ErrorMessage = "Invalid Credintials!";
                    return result;
                }

                var loginUserDeactivatio = _db.AccountDeactivation.FirstOrDefault(z => z.UserId == loginUser.Id);
                if (loginUser != null && !loginUser.IsActive && loginUserDeactivatio != null && loginUserDeactivatio.Until > DateTime.Now)
                {
                    result.ErrorMessage = $"Deactivated Account, won't be active till {loginUserDeactivatio.Until.ToString("MM/dd/yyyy")}, for help contact administration";
                    return result;
                }

                var token = _auth.GenerateJSONWebToken(email);
                if (string.IsNullOrWhiteSpace(token))
                    throw new Exception();

                result.Data = new AccessResult() { UserName = loginUser.Name, Token = token };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = General_Strings.APIIssueMessage;
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
        public General_StatusWithData ActivateAccount(string activationCode)
        {
            var result = new General_StatusWithData();
            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    var activationObj = _db.AccountVerification.FirstOrDefault(z => z.Code == activationCode && z.IsActive && z.DeletedAt == null);
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




        #region GetBy region
        public General_StatusWithData GetAccountVerificationBy(string verificationCode)
        {
            var result = new General_StatusWithData();

            try
            {
                var verificationObj = _db.AccountVerification.FirstOrDefault(z => z.IsActive && z.Code == verificationCode && z.DeletedAt == null);
                if (verificationObj != null)
                    result.Data = verificationObj;
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
                if (user != null)
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
                var user = _db.User.FirstOrDefault(z => z.Email == email && z.Password == password && z.IsActive);
                if (user != null)
                    result.Data = user;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Temp Server Error.";
            }

            return result;
        }


        #endregion


    }
}
