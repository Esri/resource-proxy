using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common.Authorization
{
    public class SignInModel
    {
        [Required, Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
