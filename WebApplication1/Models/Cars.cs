using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Cars
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public string? Details { get; set; } 
        public double Rate {  get; set; }
        public int price { get; set; }
        public  string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //now we have a foreign key with Features => we have one to many relationship with it
        public ICollection<Features>? Features { get; set; }
    }
}
