using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoWallet.Models
{
    public class ExchangeViewModel
    {
      
        [Display(Name = "From Currency")]
        public string CurrencyFrom { get; set; }
   
        [Display(Name = "To Currency")]
        public string CurrencyTo { get; set; }

        [Display(Name = "Amount")]
        [Range(0.0000001, 10000)]
        public decimal Amount { get; set; }
   
        public decimal Rate { get; set; }
        public List<SelectListItem> Accounts { get; set; } = new List<SelectListItem>();
    }
}