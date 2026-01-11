using Client.Models;
using Gateway_API_Client;

namespace Client.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private const string APIEndpoint = "/api/auth";
        private readonly IHttpContextAccessor _httpContextAccessor;  

        public AuthService(IHttpClientFactory httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        Task<T?> IAuthService.LoginAsync<T>(LoginRequestDTO loginRequestDTO) where T : default
        {
            return SendAsync<T>(new APIRequest
            {
                APIType = SD.APIType.POST,
                Data = loginRequestDTO,
                Url =APIEndpoint+ "/login",
            });
        }

        Task<T?> IAuthService.RegisterAsync<T>(RegisterationRequestDTO registerationRequestDTO) where T : default
        {
            return SendAsync<T>(new APIRequest
            {
                APIType = SD.APIType.POST,
                Data = registerationRequestDTO,
                Url = APIEndpoint+ "/register",
            });
        }
    }
}
