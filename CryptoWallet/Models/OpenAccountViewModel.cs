using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoWallet.Models
{
    public class OpenAccountViewModel
    {
        [Display(Name = "Select a currency: ")]
        public string NewCurrency { get; set; }
        public List<SelectListItem> UnopenedAccounts { get; set; } = new List<SelectListItem>();
    }
}