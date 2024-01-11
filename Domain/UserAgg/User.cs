using Domain.ProductAgg;
using Domain.RoleAgg;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UserAgg
{
    public class User
    {
        public User()
        {
            DeleteStatus = false;
        }
        public int? id { get;  set; }
        [Required]
        [MaxLength(100)]
        public string? Name { get;  set; }
        [Required]
        [MaxLength(200)]
        public string? UserName { get;  set; }
        public bool DeleteStatus { get;  set; }
        [Phone]
        [Required]
        [MaxLength(20)]
        public string? PhoneUser { get;  set; }
        [EmailAddress]
        [Required]
        [MaxLength(200)]
        public string? Email { get;  set; }
        [DataType(DataType.Password)]
        [Required]
        public string? Password { get;  set; }
        [DataType(DataType.Password)]
        public string? Password_Again { get;  set; }


        public void Update(string name, string userName, bool deleteStatus, string phoneUser, string email)
        {
            Name = name;
            UserName = userName;
            DeleteStatus = deleteStatus;
            PhoneUser = phoneUser;
            Email = email;
            DeleteStatus = false;
        }
    }
}
