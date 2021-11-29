using Cookbook.API.Configuration;
using Cookbook.API.Models;
using Cookbook.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Cookbook.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly AuthenticationSettings authenticationSettings;

        public TokenService(AuthenticationSettings authenticationSettings)
        {
            this.authenticationSettings = authenticationSettings;
        }

        public string GetUserToken(CookbookUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
            };

            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(creds);
            var payload = new JwtPayload("cookbook-dev", "cookbook-dev", claims, DateTime.Now, DateTime.Now.AddMinutes(30), DateTime.Now);

            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
