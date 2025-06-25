namespace cnd.Services
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(string username, string password);
        Task<string?> LoginAsync(string username, string password);
    }

}
