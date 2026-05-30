using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        //DbSet => is the gate for achiving access to Tables
        public DbSet<Cars> Cars { get; set; }
        public DbSet<Features> Features { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Cars>().HasData(
                    new Cars { Id = 1, Name = "Toyota Corolla", Details = "Compact sedan", Rate = 4.5, price = 20000, ImageUrl = "https://live.staticflickr.com/65535/49440475318_2d9d3c994d_o.jpg", CreatedDate = new DateTime(2025, 1, 1) },
                    new Cars { Id = 2, Name = "Honda Civic", Details = "Sporty compact", Rate = 4.7, price = 22000, ImageUrl = "https://live.staticflickr.com/65535/52051754815_b5de4bea83_b.jpg", CreatedDate = new DateTime(2025, 1, 1) },
                    new Cars { Id = 3, Name = "Ford Mustang", Details = "Muscle car", Rate = 4.9, price = 35000, ImageUrl = "https://live.staticflickr.com/65535/52045457604_fc0a27e506_b.jpg", CreatedDate = new DateTime(2025, 1, 1) },
                    new Cars { Id = 4, Name = "Chevrolet Camaro", Details = "Performance coupe", Rate = 4.8, price = 34000, ImageUrl = "https://live.staticflickr.com/65535/52821278553_d03072cf41_b.jpg", CreatedDate = new DateTime(2025, 1, 1) },
                    new Cars { Id = 5, Name = "BMW 3 Series", Details = "Luxury sedan", Rate = 4.6, price = 42000, ImageUrl = "https://live.staticflickr.com/65535/51016184159_d0fc8847be_b.jpg", CreatedDate = new DateTime(2025, 1, 1) },
                    new Cars { Id = 6, Name = "Audi A4", Details = "Comfortable sedan", Rate = 4.5, price = 41000, ImageUrl = "https://live.staticflickr.com/65535/50273443922_298b8bb6f8_b.jpg", CreatedDate = new DateTime(2025, 1, 1) },
                    new Cars { Id = 7, Name = "Mercedes C-Class", Details = "Premium sedan", Rate = 4.6, price = 43000, ImageUrl = "https://live.staticflickr.com/65535/51818320308_1769ae013c_b.jpg", CreatedDate = new DateTime(2025, 1, 1) },
                    new Cars { Id = 8, Name = "Tesla Model 3", Details = "Electric sedan", Rate = 4.9, price = 50000, ImageUrl = "https://live.staticflickr.com/65535/50195997318_00d4640d11_b.jpg", CreatedDate = new DateTime(2025, 1, 1) },
                    new Cars { Id = 9, Name = "Volkswagen Golf", Details = "Compact hatchback", Rate = 4.3, price = 23000, ImageUrl = "https://live.staticflickr.com/65535/50376941986_69e4f2056f_b.jpg", CreatedDate = new DateTime(2025, 1, 1) },
                    new Cars { Id = 10, Name = "Nissan Altima", Details = "Midsize sedan", Rate = 4.2, price = 25000, ImageUrl = "https://live.staticflickr.com/65535/50733651236_abc123abcd_b.jpg", CreatedDate = new DateTime(2025, 1, 1) }
                );
        }
    }
}
