using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class ApplicationUser : IdentityUser
    {
        //add custom properties on top of the default properties of IdentityUser
        public string Name { get; set; }
    }
}
