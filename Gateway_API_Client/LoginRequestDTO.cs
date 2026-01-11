using System.ComponentModel.DataAnnotations;

namespace Gateway_API_Client
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

    }
}
