using KaDiFi.Entities;
using KaDiFi.Helpers.IHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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

        //public string GenerateJSONWebToken(string email)
        //{
        //    var result = "";
        //    try
        //    {
        //        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //        var claims = new[] { new Claim(JwtRegisteredClaimNames.Email, email) };

        //        var token = new JwtSecurityToken(
        //          _config["Jwt:Issuer"],
        //          _config["Jwt:Issuer"],
        //          claims,
        //          expires: DateTime.Now.AddMinutes(120),
        //          signingCredentials: credentials);

        //        result = new JwtSecurityTokenHandler().WriteToken(token);
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return result;
        //}
        public string GenerateJSONWebToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] { new Claim(JwtRegisteredClaimNames.Email, email) };


            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public General_StatusWithData ValidateToken(IEnumerable<Claim> claims)
        {
            var result = new General_StatusWithData();

            try
            {
                var userClaims = claims.ToList();
                var expirationObj = userClaims.FirstOrDefault(z => z.Type == JwtRegisteredClaimNames.Exp)?.Value;
                if (string.IsNullOrWhiteSpace(expirationObj))
                    throw new Exception();

                var expirationDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                var exp = double.Parse(expirationObj);
                expirationDate = expirationDate.AddSeconds(exp).ToLocalTime();
                if (expirationDate <= DateTime.Now)
                    throw new Exception();

                var email = userClaims.FirstOrDefault(z => z.Type == ClaimTypes.Email)?.Value;
                if (email == null)
                    throw new Exception();

                result.Data = email;
            }
            catch (Exception)
            {
                result.IsSuccess = false;
                result.ErrorMessage = General_Strings.APIInvalidTokenMessage;
            }

            return result;
        }







    }
}






//if (userClaims.HasClaim(z => z.Type == JwtRegisteredClaimNames.Email.ToString()))
//{
//    var userEmail = userClaims.Claims.FirstOrDefault(z => z.Type == JwtRegisteredClaimNames.Email.ToString()).Value;
//    var userExtistanceStatus = _db.User.FirstOrDefault(z => z.Email == userEmail);
//    if (userExtistanceStatus == null)
//    {
//        result.IsSuccess = false;
//        result.ErrorMessage = General_Strings.APIInvalidTokenMessage;
//        return result;
//    }

//}
