using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoWallet.Models
{
    public class CurrencyRateViewModel
    {
        public string Currency { set; get; }
        public decimal Rate { get; set; }
    }
}