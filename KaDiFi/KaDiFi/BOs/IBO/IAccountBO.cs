using KaDiFi.Entities;

namespace KaDiFi.BOs.IBO
{
    public interface IAccountBO
    {
        General_Status RegisterUser(User newUser);
        General_StatusWithData GetAccess(string email, string password);


    }
}
