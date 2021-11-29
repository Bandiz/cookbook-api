using Cookbook.API.Models;

namespace Cookbook.API.Services.Interfaces
{
    public interface ITokenService
    {
        string GetUserToken(CookbookUser user);
    }
}
