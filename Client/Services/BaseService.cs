using System.Text.Json;
using Client.Models;
using Gateway_API_Client;
using System.Net.Http.Headers;

namespace Client.Services
{
    public class BaseService : IBaseService
    {
        //for connecting client to end points

        //when clients recived response, that should be serialized
        private static readonly JsonSerializerOptions JsonOption = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        private readonly IHttpContextAccessor _httpContextAccessor;  //to retriev JWT
        public IHttpClientFactory _httpClient { get; set; }
        public ApiResponse<object>ResponseModel { get; set; }
        public BaseService(IHttpClientFactory httpClient, IHttpContextAccessor httpContextAccessor  )
        {
            _httpClient = httpClient;
            this.ResponseModel = new();
            _httpContextAccessor = httpContextAccessor;
        }



        public async Task<T?> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = _httpClient.CreateClient("CarGallaryAPI");
                var message = new HttpRequestMessage
                {
                    //UriKind.Relative => not https://site.com/api/car, but /api/car
                    RequestUri = new Uri(apiRequest.Url, uriKind:UriKind.Relative),
                    Method = GetHttpMethod(apiRequest.APIType),
                };

                var token = _httpContextAccessor.HttpContext?.Session?.GetString(SD.SessionToken);
                if( !string.IsNullOrEmpty(token) )
                {
                    message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                if(apiRequest.Data!=null)
                {
                    message.Content=JsonContent.Create(apiRequest.Data, options: JsonOption);
                }
                var response = await client.SendAsync(message);
                return await response.Content.ReadFromJsonAsync<T>(JsonOption);

            }catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error : {ex.Message}");
                return default;
            }
        }



        //helper method 
        private static HttpMethod GetHttpMethod(SD.APIType apiType)
        {
            //convert our SD into http
            return apiType switch
            {
                SD.APIType.POST => HttpMethod.Post,
                SD.APIType.PUT => HttpMethod.Put,
                SD.APIType.DELETE => HttpMethod.Delete,
                _ => HttpMethod.Get, //for the default 
            };
        }
    }
}
