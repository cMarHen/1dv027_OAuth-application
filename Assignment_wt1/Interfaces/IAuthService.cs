using Assignment_wt1.Models;

namespace Assignment_wt1.Interfaces
{
    public interface IAuthService
    {
        public string? GetAuthUri();
        public Task HandleLogin(string code);
        public Task HandleRefreshToken(string refreshToken);
    }
}
