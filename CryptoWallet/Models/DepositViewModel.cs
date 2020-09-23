
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CryptoWallet.Models
{
    public class DepositViewModel
    {
        [Range(0.0000001, 10000)]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
    }
}