using Application.Models.Identity;

namespace Application.Contracts
{
    public interface IAuthService
    {
        string GenerateToken(AuthResponse user);
        AuthResponse Authenticate(AuthRequest userLogin);
    }
}
