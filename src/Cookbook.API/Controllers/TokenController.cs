using Cookbook.API.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Cookbook.API.Configuration;

namespace Cookbook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<CookbookUser> userManager;
        private readonly AuthenticationSettings authenticationSettings;

        public TokenController(UserManager<CookbookUser> userManager, AuthenticationSettings authenticationSettings)
        {
            this.userManager = userManager;
            this.authenticationSettings = authenticationSettings;
        }

        [HttpGet]
        public async Task<IActionResult> GetTokenGoogle([FromQuery] string t)
        {
            if (string.IsNullOrEmpty(t))
            {
                return BadRequest("Google token is not provided");
            }
            GoogleJsonWebSignature.Payload googlePayload;
            try
            {
                googlePayload = await GoogleJsonWebSignature.ValidateAsync(t, new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { authenticationSettings.Google.ClientId }
                });
            }
            catch
            {
                return BadRequest("Google token is not valid");
            }

            var user = await userManager.FindByEmailAsync(googlePayload.Email);

            if (user == null)
            {
                user = new CookbookUser
                {
                    Email = googlePayload.Email,
                    EmailConfirmed = googlePayload.EmailVerified,
                    UserName = googlePayload.Email,
                    Name = googlePayload.GivenName,
                    LastName = googlePayload.FamilyName,
                    FullName = googlePayload.Name
                };

                var result = await userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest("User could not be created");
                }
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
            };

            if (user.Roles.Count > 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, string.Join(',', user.Roles)));
            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(creds);
            var payload = new JwtPayload("yourdomain.com", "yourdomain.com", claims, DateTime.Now, DateTime.Now.AddMinutes(30), DateTime.Now);

            var token = new JwtSecurityToken(header, payload);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
