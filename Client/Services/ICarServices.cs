using Gateway_API_Client;

namespace Client.Services
{
    public interface ICarServices
    {
        //since we perform crud
        Task<T?> GetAllAsync<T>();
        Task<T?> GetAsync<T>(int id);
        Task<T?> CreateAsync<T>(CreateCarsDTO dto);
        Task<T?> UpdateAsync<T>(UpdateDTO dto);
        Task<T?> DeleteAsync<T>(int id);
    }
}
