using System.ComponentModel.DataAnnotations;

namespace Gateway_API_Client
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; } = default!;
        public  string Name { get; set; } = default!;
        public required string Role { get; set; } = default!;
    }
}
