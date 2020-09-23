using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoWallet.Models
{
    public class SendViewModel
    {
      
        [MaxLength(64)]
        [Display(Name = "From Account:")]
        public string SenderAccountId { get; set; }

        [MaxLength(64)]
        [Display(Name = "To Account:")]
        public string ReceiverName { get; set; }
        [Required]
        [Range(0.0000001, 10000)]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        public List<SelectListItem> SenderAccounts { get; set; } = new List<SelectListItem>();
    }
}