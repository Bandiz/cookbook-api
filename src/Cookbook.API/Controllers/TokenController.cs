using Cookbook.API.Configuration;
using Cookbook.API.Extensions;
using Cookbook.API.Models;
using Cookbook.API.Services;
using Cookbook.API.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cookbook.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly AuthenticationSettings authenticationSettings;
        private readonly ITokenService tokenService;
        private readonly UserManager<CookbookUser> userManager;

        public TokenController(AuthenticationSettings authenticationSettings, ITokenService tokenService, UserManager<CookbookUser> userManager)
        {
            this.authenticationSettings = authenticationSettings;
            this.tokenService = tokenService;
            this.userManager = userManager;
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
                user = await userManager.CreateUserAsync(googlePayload);

                if (user == null)
                {
                    return BadRequest("User could not be created");
                }
            }

            return Ok(new
            {
                token = tokenService.GetUserToken(user),
                user = new
                {
                    email = user.Email,
                    lastName = user.LastName,
                    name = user.Name,
                    isAdmin = user.IsAdmin
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> GetToken([FromForm] string userName, [FromForm] string password)
        {
            var user = await userManager.FindByNameAsync(userName);

            if (user == null || !await userManager.CheckPasswordAsync(user, password))
            {
                return BadRequest("Username or password wrong");
            }

            return Ok(new
            {
                token = tokenService.GetUserToken(user),
                user = new
                {
                    email = user.Email,
                    lastName = user.LastName,
                    name = user.Name,
                    isAdmin = user.IsAdmin
                }
            });
        }
    }
}


