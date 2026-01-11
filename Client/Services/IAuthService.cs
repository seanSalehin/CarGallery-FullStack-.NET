using Gateway_API_Client;

namespace Client.Services
{
    public interface IAuthService
    {
        Task<T?> LoginAsync<T>(LoginRequestDTO loginRequestDTO);
        Task<T?> RegisterAsync<T>(RegisterationRequestDTO registerationRequestDTO);
    }
}
