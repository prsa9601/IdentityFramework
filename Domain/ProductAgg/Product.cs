using Domain.Permission;
using Domain.RoleAgg;
using Domain.UserAgg;
using Domain.UserAppAgg;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProductAgg
{
    public class Product
    {
        public Product()
        {
            DeleteStatus = false;
        }

        public int? Id { get;  set; }
        public string? userId { get;  set; }
        [Required]
        [MaxLength(100)]
        public string? Name { get;  set; }
        public string? UserName { get;  set; }
        public DateTime ProduceDate { get;  set; }
        [MaxLength(100)]
        public string? Price { get;  set; }
        public bool DeleteStatus { get;  set; }
        [Phone]
        [MaxLength(11,ErrorMessage ="شماره تلفن نباید بیشتر از 11 حرف باشد.")]
        [MinLength(11,ErrorMessage ="شماره تلفن نباید کمتر از 11 حرف باشد.")]
        [Required]
        public string? PhoneUser { get;  set; }
        public bool Is_A_Valiable { get;  set; }
         public UserApp? UserApp { get;  set; }

    }
}
