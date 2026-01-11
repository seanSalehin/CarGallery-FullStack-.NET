using System.ComponentModel.DataAnnotations;

namespace Gateway_API_Client
{
    public class UpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [MaxLength(30)]
        [Required]
        public required string Name { get; set; }
        public string? Details { get; set; }
        public double Rate { get; set; }
        public int price { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
