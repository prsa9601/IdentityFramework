using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UserAgg
{
    public class Register
    {
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }
        [Required]
        [MaxLength(200)]
        public string? UserName { get; set; }
        [Phone]
        [Required]
        [MaxLength(11, ErrorMessage = "شماره تلفن نباید بیشتر از 11 حرف باشد.")]
        [MinLength(11, ErrorMessage = "شماره تلفن نباید کمتر از 11 حرف باشد.")]
        public string? PhoneUser { get; set; }
        [EmailAddress]
        [Required]
        [MaxLength(200)]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        public string? Password_Again { get; set; }
    }
}
