using Domain.ProductAgg;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Permission
{
    public class Permission
    {
        public Permission()
        {
            DeleteStatus = false;
        }
        public int? id { get; set; }
        [Required]
        public string? ProductName { get; set; }
        public string? RoleName { get; set; }
        public int? ProductId { get; set; }
        public int? RoleId { get; set; }
        public bool? DeleteStatus { get; set; }
        [Required]
        public bool PermissionUpdate { get; set; }
        [Required]
        public bool PermissionDelete { get; set; }
        [Required]
        public RoleAgg.Role? Roles { get; set; }
        [Required]
        public Product? Products { get; set; }
    }
}
