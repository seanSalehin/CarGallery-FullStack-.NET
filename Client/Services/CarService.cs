using Gateway_API_Client;

namespace Client.Services
{
    public class CarService : BaseService,ICarServices
    {
        private const string APIEndpoint = "/api/car";
        private readonly IHttpContextAccessor _httpContextAccessor; 

        public CarService(IHttpClientFactory httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public Task<T?> CreateAsync<T>(CreateCarsDTO dto) 
        {
            return SendAsync<T>(new Models.APIRequest
            {
                APIType = SD.APIType.POST,
                Data = dto,
                Url = $"{APIEndpoint}"
            });
        }


        public Task<T?> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new Models.APIRequest
            {
                APIType = SD.APIType.DELETE,
                Url = $"{APIEndpoint}/{id}"
            });
        }


        public Task<T?> GetAllAsync<T>() 
        {
            return SendAsync<T>(new Models.APIRequest
            {
                APIType = SD.APIType.GET,
                Url = $"{APIEndpoint}"
            });
        }



        public Task<T?> GetAsync<T>(int id) 
        {
            return SendAsync<T>(new Models.APIRequest
            {
                APIType = SD.APIType.GET,
                Url = $"{APIEndpoint}/{id}"
            });
        }


        public Task<T?> UpdateAsync<T>(UpdateDTO dto)
        {
            return SendAsync<T>(new Models.APIRequest
            {
                APIType = SD.APIType.PUT,
                Data=dto,
                Url = $"{APIEndpoint}/{dto.Id}"
            }); 
        }
    }
}
