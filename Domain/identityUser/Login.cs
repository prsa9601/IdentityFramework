using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.identityUser
{
    public class Login
    {
        [Required]
        [Display(Name = "User Name")]
        [StringLength(200)]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
    }

    public class LoginWithPhone
    {
        [Required]
        [Phone]
        public string Phone { get; set; }

        public string ReturnUrl { get; set; }
        public List<AuthenticationScheme>? ExternalLogins { get; set; }
    }
}
