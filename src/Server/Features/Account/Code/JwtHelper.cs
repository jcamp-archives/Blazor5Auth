using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blazor5Auth.Server.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoFramework.AspNetCore.Identity;

namespace Features.Account
{
    public interface IJwtHelper
    {
        string GenerateJwt<TUser>(TUser user, IList<string> roles) where TUser : MongoIdentityUser;
    }

    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwt<TUser>(TUser user, IList<string> roles) where TUser : MongoIdentityUser
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, user.Email));

            //this is needed for identity system to retrieve full user object
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

            claims.Add(new Claim(ClaimTypes.Spn, user.GetType().Name));

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
