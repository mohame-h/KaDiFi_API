using KaDiFi.Entities;
using KaDiFi.Helpers;
using System;


namespace KaDiFi.BOs
{
    public class AccountBO
    {
        private KaDifiEntities _db;

        public AccountBO(KaDifiEntities db)
        {
            _db = db;
        }

        public General_Status RegisterUser(User newUser)
        {
            var result = new General_Status() { };
            try
            {
                newUser.Id = new Guid().ToString();
                newUser.Password = newUser.Password.Mask();
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

    }
}
