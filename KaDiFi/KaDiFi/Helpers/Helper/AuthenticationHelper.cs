using KaDiFi.Entities;
using KaDiFi.Helpers.IHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace KaDiFi.Helpers
{
    public class AuthenticationHelper : IAuthenticationHelper
    {
        private KaDifiEntities _db;
        private IConfiguration _config;

        public AuthenticationHelper(KaDifiEntities db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public string GenerateJSONWebToken(string email)
        {
            var result = "";
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[] { new Claim(JwtRegisteredClaimNames.Email, email) };

                var token = new JwtSecurityToken(
                  _config["Jwt:Issuer"],
                  _config["Jwt:Issuer"],
                  claims,
                  expires: DateTime.Now.AddMinutes(120),
                  signingCredentials: credentials);

                result = new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception)
            {

            }
            return result;
        }

        public General_Status ValidateToken(ClaimsPrincipal userClaims)
        {
            var result = new General_Status();

            try
            {
                if (userClaims.HasClaim(z => z.Type == JwtRegisteredClaimNames.Email.ToString()))
                {
                    var userEmail = userClaims.Claims.FirstOrDefault(z => z.Type == JwtRegisteredClaimNames.Email.ToString()).Value;
                    var userExtistanceStatus = _db.User.FirstOrDefault(z => z.Email == userEmail);
                    if (userExtistanceStatus == null)
                    {
                        result.IsSuccess = false;
                        result.ErrorMessage = General_Strings.APIInvalidTokenMessage;
                        return result;
                    }

                }


            }
            catch (Exception)
            {
                result.IsSuccess = false;
                result.ErrorMessage = General_Strings.APIIssueMessage;
            }



            return result;
        }







    }
}
