using Domain.ProductAgg;
using Domain.RoleAgg;
using Microsoft.AspNetCore.Identity;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UserAppAgg
{
    public class UserApp : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }
        public string? Password { get; set; }
        public string? JwtSecret { get; set; }
        public string? Role{ get; set; }
        public List<Product>? Products { get; set; }
    }
    
}
