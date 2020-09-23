using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CryptoWallet.Models
{
    public class LoginViewModel
    {
        [Required]
        [MaxLength(64)]
        [Display(Name ="Email")]
        public string Username { get; set; }

        [Required]
        [MaxLength(32)]
        [Display(Name= "Parola")]
        public string Password { get; set; }
    }
}