using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.identityUser
{
    public class LoginConfirmation
    {
        [Required, StringLength(6)]
        public string SmsCode { get; set; }

        public string Phone { get; set; }
        public string Code { get; set; }
    }
}
