using Domain.ProductAgg;
using Domain.UserAgg;
using Domain.UserAppAgg;
using Microsoft.AspNetCore.Identity;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RoleAgg
{
    public class Role : IdentityRole
    {
        public Role()
        {
            DeleteStatus = false;
        }
        [Required]
        public int roleId { get; set; }
        [Required]
        public int UserId { get; set; }
        public bool DeleteStatus { get; set; }
        public List<Permission.Permission>? Permissions{ get; set; }
    }
}
