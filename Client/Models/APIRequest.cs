using static Client.SD;

namespace Client.Models
{
    public class APIRequest
    {
        //for connecting CLient to API End points

        public APIType APIType { get; set; } = APIType.GET;
        public string? Url { get; set; }
        public object? Data { get; set; }
        public string? Token { get; set; }
    }
}
