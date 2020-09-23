using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CryptoWallet.Models
{
    public class RegisterViewModel
    {
        [Required]
        [MaxLength(64)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(32)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [MaxLength(32)]
        [Display(Name = "Name")]
        public string Name { get; set; }

    }
}