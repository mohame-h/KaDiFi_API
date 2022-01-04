using KaDiFi.Entities;
using System.Security.Claims;

namespace KaDiFi.Helpers.IHelper
{
    public interface IAuthenticationHelper
    {
        string GenerateJSONWebToken(string email);
        General_Status ValidateToken(ClaimsPrincipal userClaims);
    }
}
