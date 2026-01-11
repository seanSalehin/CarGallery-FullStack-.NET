using Gateway_API_Client;

namespace WebApplication1.Services
{
    public interface IAuthService
    {
        //we have 3 end points for user
        //since we have async, the best return type => Task<>
        //UserDTO? => nullable
        Task<UserDTO?> RegisterAsync(RegisterationRequestDTO registararionRequestDTO);
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<bool> IsEmailExistAsync(string email);
    }
}
