using KaDiFi.Entities;
using System.Collections.Generic;
using System.Security.Claims;

namespace KaDiFi.Helpers.IHelper
{
    public interface IAuthenticationHelper
    {
        string GenerateJSONWebToken(string email);
        General_StatusWithData ValidateToken(IEnumerable<Claim> claims);
    }
}
