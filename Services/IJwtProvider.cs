using JwtAuthentication.Models;

namespace JwtAuthentication.Services
{
    public interface IJwtProvider
    {
        string CreateToken(AppUser user);
    }
}
