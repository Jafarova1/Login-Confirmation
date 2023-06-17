using Microsoft.AspNetCore.Identity;

namespace FiorelloOneToMany.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}
