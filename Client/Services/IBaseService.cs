using Client.Models;
using Gateway_API_Client;

namespace Client.Services
{
    public interface IBaseService
    {
        //send request to server
        Task<T>SendAsync<T>(APIRequest apiRequest);

        //recive response from server
        ApiResponse<object>ResponseModel { get; set; }
    }
}
