using KaDiFi.Entities;

namespace KaDiFi.BOs.IBO
{
    public interface IAccountBO
    {
        General_Status RegisterUser(User newUser);
        General_StatusWithData GetAccountVerificationBy(string verificationCode);
        General_Status VerifyAccount(string verificationCode);

        General_StatusWithData GetAccess(string email, string password);
        General_StatusWithData GetUserBy(string email);
        General_StatusWithData GetUserBy(string email, string password);

        General_Status ChangeUserPassword(string email, string newPassword);
        General_Status DisableAccount(string email, int freezingDaysCount);

        General_Status DisableNews(string email);
        General_StatusWithData ActivateAccount(string activationCode);

    }
}
