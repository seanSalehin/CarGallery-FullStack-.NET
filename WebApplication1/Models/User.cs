using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class User
    {
        //for authentication

        [Key]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Role { get; set; } = "customer";
        public DateTime CreatedDate { get; set; } 
        public DateTime UpdateDate { get; set; } 
    }
}
